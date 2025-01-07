namespace LibEncryptedDriveScripts.Database;

using System.IO;
using Microsoft.Data.Sqlite;

public abstract class DatabaseOperatorBase : IDatabaseOperator
{
    private static string _tableName = "data";
    private static string _indexName = "b_index";
    private static string _dataName = "b_data";
    private SqliteConnection _sqliteConnection;

    public DatabaseOperatorBase(string dbPath, bool createFlag)
    {
        bool dbPathExists = File.Exists(dbPath);
        string connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
        if(!createFlag && !dbPathExists)
        {
            throw new FileNotFoundException($"'{dbPath}' does not exists.");
        }
        _sqliteConnection = new SqliteConnection(connectionString);
        if(createFlag && !dbPathExists)
        {
            InitDatabase();
        }
    }
    public DatabaseOperatorBase(string dbPath) : this(dbPath, false) {}

    public Stream GetDataStream(byte[] index)
    {
        _sqliteConnection.Open();
        string sqltext = $"SELECT {_dataName} FROM {_tableName} WHERE {_indexName} = @{_indexName};";
        Stream result;
        using (var command = new SqliteCommand(sqltext, _sqliteConnection))
        {
            command.Parameters.AddWithValue($"@{_indexName}", index);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = reader.GetStream(0);
                    _sqliteConnection.Close();
                    SqliteConnection.ClearPool(_sqliteConnection);
                    return result;
                }
            }
        }
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
        throw new InvalidOperationException($"There was no row with the specified index value.");
    }
    public byte[] GetDataBytes(byte[] index)
    {
        _sqliteConnection.Open();
        string sqltext = $"SELECT {_dataName} FROM {_tableName} WHERE {_indexName} = @{_indexName};";
        byte[] result;
        using (var command = new SqliteCommand(sqltext, _sqliteConnection))
        {
            command.Parameters.AddWithValue($"@{_indexName}", index);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = (byte[])reader[_dataName];
                    _sqliteConnection.Close();
                    SqliteConnection.ClearPool(_sqliteConnection);
                    return result;
                }
            }
        }
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
        throw new InvalidOperationException($"There was no row with the specified index value.");
    }
    public void InsertData(byte[] index, byte[] data)
    {
        _sqliteConnection.Open();
        string sqltext = $"INSERT INTO {_tableName} ({_indexName}, {_dataName}) VALUES (@{_indexName}, @{_dataName});";
        using (var command = new SqliteCommand(sqltext ,_sqliteConnection))
        {
            command.Parameters.AddWithValue($"@{_indexName}", index);
            command.Parameters.AddWithValue($"@{_dataName}", data);
            command.ExecuteNonQuery();
        }
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
    }

    protected void InitDatabase()
    {
        _sqliteConnection.Open();
        CreatedDataTable();
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
    }
    public void InsertData(byte[] index, Stream readableStream)
    {
        _sqliteConnection.Open();
        long streamLength = readableStream.Length - readableStream.Position;
        string sqltext = $@"
            INSERT INTO {_tableName} ({_indexName}, {_dataName}) VALUES (@{_indexName}, zeroblob({streamLength}));
            SELECT last_insert_rowid();
        ";
        using (var command = new SqliteCommand(sqltext ,_sqliteConnection))
        {
            command.Parameters.AddWithValue($"@{_indexName}", index);
            long rowid = (long)(command.ExecuteScalar() ??
                throw new InvalidOperationException("Could not read the value of \"last_insert_rowid()\"."));
            using (var writeStream = new SqliteBlob(_sqliteConnection, "data", "b_data", rowid))
            {
                readableStream.CopyTo(writeStream);
            }
        }
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
    }
    private bool IsIndexExists(byte[] index)
    {
        throw new NotImplementedException();
    }
    private void CreatedDataTable()
    {
        string sqltext = $"CREATE TABLE {_tableName} ({_indexName} BLOB PRIMARY KEY, {_dataName} BLOB)";
        using (var command = new SqliteCommand(sqltext, _sqliteConnection))
        {
            command.ExecuteNonQuery();
        }
    }

    bool IDatabaseOperator.IsIndexExists(byte[] index)
    {
        _sqliteConnection.Open();
        string sqltext = $"SELECT _ROWID_ FROM {_tableName} WHERE {_indexName} = @{_indexName};";
        using (var command = new SqliteCommand(sqltext, _sqliteConnection))
        {
            command.Parameters.AddWithValue($"@{_indexName}", index);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    _sqliteConnection.Close();
                    SqliteConnection.ClearPool(_sqliteConnection);
                    return true;
                }
            }
        }
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
        return false;
    }
}
