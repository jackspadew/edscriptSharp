namespace LibEd.EdData;

using System.IO;
using LibEd.Database;
using LibEd.Executor;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

public class EdDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    public EdDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {
    }
}

public class FakeInsertionDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    protected int InsertingFakeRowCount = 19;
    protected IActionExecutor RandomExecutor => new RandomizedExecutor([1,InsertingFakeRowCount]);
    public FakeInsertionDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {}
    public FakeInsertionDatabaseOperator(string dbPath, bool createFlag, int insertingFakeCount) : base(dbPath, createFlag)
    {
        InsertingFakeRowCount = insertingFakeCount;
    }

    public override void InsertData(byte[] index, byte[] data)
    {
        _sqliteConnection.Open();
        // Fake insertion variables
        byte[] randomIndexBuffer = new byte[index.Length];
        byte[] randomDataBuffer = new byte[data.Length];
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        // transaction
        try{
            using (var transaction = _sqliteConnection.BeginTransaction())
            {
                var fakeInsertionVariables = CreateFakeInsertionVariables(_sqliteConnection, transaction);
                // Execute commands
                RandomExecutor.Run([
                    () => {
                        ExecuteRealInsertionCommand(_sqliteConnection, transaction, index, data);
                    },
                    () => {
                        ExecuteFakeInsertionCommand(fakeInsertionVariables.command, fakeInsertionVariables.paramIndex, fakeInsertionVariables.paramData, randomIndexBuffer, randomDataBuffer, rng);
                    }
                ]);
                transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("", ex);
        }
        finally
        {
            _sqliteConnection.Close();
            SqliteConnection.ClearPool(_sqliteConnection);
        }
    }
    public override void InsertData(byte[] index, Stream readableStream)
    {
        RandomExecutor.Run([
            () => base.InsertData(index, readableStream),
            () => InsertFakeData(index, readableStream)
        ]);
    }
    protected virtual void InsertFakeData(byte[] imitatedIndex, byte[] imitatedData)
    {
        byte[] fakeIndex = GeneratedRandomBytes(imitatedIndex.Length);
        byte[] fakeData = GeneratedRandomBytes(imitatedData.Length);
        base.InsertData(fakeIndex, fakeData);
    }
    protected virtual void InsertFakeData(byte[] imitatedIndex, Stream readableStream)
    {
        byte[] fakeIndex = GeneratedRandomBytes(imitatedIndex.Length);
        byte[] fakeData = GeneratedRandomBytes(readableStream.Length);
        base.InsertData(fakeIndex, fakeData);
    }
    protected byte[] GeneratedRandomBytes(long length)
    {
        byte[] result = new byte[length];
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(result);
        return result;
    }
    protected virtual void ExecuteRealInsertionCommand(SqliteConnection connection, SqliteTransaction transaction, byte[] index, byte[] data)
    {
        string realInsertionText = $"INSERT INTO {_tableName} ({_indexName}, {_dataName}) VALUES (@{_indexName}, @{_dataName});";
        var command = new SqliteCommand(realInsertionText ,connection, transaction);
        command.Parameters.AddWithValue($"@{_indexName}", index);
        command.Parameters.AddWithValue($"@{_dataName}", data);
        command.ExecuteNonQuery();
    }
    protected virtual void ExecuteRealInsertionCommand(SqliteConnection connection, SqliteTransaction transaction, byte[] index, Stream readableStream)
    {
        long streamLength = readableStream.Length - readableStream.Position;
        string sqltext = $@"
            INSERT INTO {_tableName} ({_indexName}, {_dataName}) VALUES (@{_indexName}, zeroblob({streamLength}));
            SELECT last_insert_rowid();
        ";
        var command = new SqliteCommand(sqltext ,connection, transaction);
        command.Parameters.AddWithValue($"@{_indexName}", index);
        long rowid = (long)(command.ExecuteScalar() ??
                    throw new InvalidOperationException("Could not read the value of \"last_insert_rowid()\"."));
        using (var writeStream = new SqliteBlob(_sqliteConnection, _tableName, _dataName, rowid))
        {
            readableStream.CopyTo(writeStream);
        }
    }
    protected virtual void ExecuteFakeInsertionCommand(SqliteCommand fakeInsertionCommand, SqliteParameter paramIndex, SqliteParameter paramData, byte[] randomIndexBuffer, byte[] randomDataBuffer, RandomNumberGenerator rng)
    {
        rng.GetBytes(randomIndexBuffer);
        paramIndex.Value = randomIndexBuffer;
        rng.GetBytes(randomDataBuffer);
        paramData.Value = randomDataBuffer;
        fakeInsertionCommand.ExecuteNonQuery();
    }
    protected virtual (SqliteCommand command, SqliteParameter paramIndex, SqliteParameter paramData) CreateFakeInsertionVariables(SqliteConnection connection, SqliteTransaction transaction)
    {
        string fakeInsertionText = $"INSERT OR IGNORE INTO {_tableName} ({_indexName}, {_dataName}) VALUES (@{_indexName}, @{_dataName});";
        var fakeInsertionCommand = new SqliteCommand(fakeInsertionText ,_sqliteConnection, transaction);
        var paramIndex = fakeInsertionCommand.CreateParameter();
        paramIndex.ParameterName = $"@{_indexName}";
        var paramData = fakeInsertionCommand.CreateParameter();
        paramData.ParameterName = $"@{_dataName}";
        fakeInsertionCommand.Parameters.Add(paramIndex);
        fakeInsertionCommand.Parameters.Add(paramData);
        return (fakeInsertionCommand, paramIndex, paramData);
    }
}
