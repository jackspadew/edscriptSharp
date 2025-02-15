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
    protected IActionExecutor RandomExecutor => new RandomizedExecutor(new int[]{1,InsertingFakeRowCount});
    public FakeInsertionDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {}
    public FakeInsertionDatabaseOperator(string dbPath, bool createFlag, int insertingFakeCount) : base(dbPath, createFlag)
    {
        InsertingFakeRowCount = insertingFakeCount;
    }

    public override void InsertData(byte[] index, byte[] data)
    {
        InsertWithFakeInsertion(
            (a,b) => {
                ExecuteRealInsertionCommand(a, b, index, data);
            },
            index.Length,
            data.Length
        );
    }
    public override void InsertData(byte[] index, Stream readableStream)
    {
        InsertWithFakeInsertion(
            (a,b) => {
                ExecuteRealInsertionCommand(a, b, index, readableStream);
            },
            index.Length,
            readableStream.Length
        );
    }
    protected virtual void InsertWithFakeInsertion(Action<SqliteConnection,SqliteTransaction> actionRealInsertion, long indexLength, long dataLength)
    {
        _sqliteConnection.Open();
        // Fake insertion variables
        byte[] randomIndexBuffer = new byte[indexLength];
        byte[] randomDataBuffer = new byte[dataLength];
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        // transaction
        try{
            using (var transaction = _sqliteConnection.BeginTransaction())
            {
                var fakeInsertionVariables = CreateFakeInsertionVariables(_sqliteConnection, transaction);
                // Execute commands
                RandomExecutor.Run(new Action[]{
                    () => {
                        actionRealInsertion(_sqliteConnection, transaction);
                    },
                    () => {
                        ExecuteFakeInsertionCommand(fakeInsertionVariables.command, fakeInsertionVariables.paramIndex, fakeInsertionVariables.paramData, randomIndexBuffer, randomDataBuffer, rng);
                    }
                });
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
