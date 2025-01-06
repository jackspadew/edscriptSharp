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
        string sqltext = $"SELECT {_dataName} FROM {_tableName} WHERE {_indexName} = @{_indexName};";
        using (var command = new SqliteCommand(sqltext, _sqliteConnection))
        {
            command.Parameters.AddWithValue($"@{_indexName}", index);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetStream(0);
                }
            }
        }
        throw new InvalidOperationException($"There was no row with the specified index value.");
    }
    public byte[] GetDataBytes(byte[] index)
    {
        throw new NotImplementedException();
    }
    public void InsertData(byte[] index, byte[] data)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
    private bool IsIndexExists(byte[] index)
    {
        throw new NotImplementedException();
    }
    private void CreatedDataTable()
    {
        using (var command = new SqliteCommand("CREATE TABLE data (b_index BLOB PRIMARY KEY, b_data BLOB)", _sqliteConnection))
        {
            command.ExecuteNonQuery();
        }
    }
}
