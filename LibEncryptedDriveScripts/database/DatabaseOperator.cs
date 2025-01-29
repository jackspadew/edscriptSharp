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
        try
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
        throw new InvalidOperationException($"There was no row with the specified index value.");
    }
    public byte[] GetDataBytes(byte[] index)
    {
        try
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
        throw new InvalidOperationException($"There was no row with the specified index value.");
    }
    public void InsertData(byte[] index, byte[] data)
    {
        try
        {
            _sqliteConnection.Open();
            string sqltext = $"INSERT INTO {_tableName} ({_indexName}, {_dataName}) VALUES (@{_indexName}, @{_dataName});";
            using (var command = new SqliteCommand(sqltext ,_sqliteConnection))
            {
                command.Parameters.AddWithValue($"@{_indexName}", index);
                command.Parameters.AddWithValue($"@{_dataName}", data);
                command.ExecuteNonQuery();
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

    public void UpdateData(byte[] index, byte[] data)
    {
        try
        {
            _sqliteConnection.Open();
            string sqltext = $"UPDATE {_tableName} SET {_dataName} = @{_dataName} WHERE {_indexName} = @{_indexName};";
            using (var command = new SqliteCommand(sqltext ,_sqliteConnection))
            {
                command.Parameters.AddWithValue($"@{_indexName}", index);
                command.Parameters.AddWithValue($"@{_dataName}", data);
                int affectedRowCount = command.ExecuteNonQuery();
                if( affectedRowCount == 0 )
                {
                    string indexBytesString = BitConverter.ToString(index);
                    throw new ArgumentException($"The Update command was successfully executed, but there was no updated row. (given index:\"{indexBytesString}\")");
                }
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

    protected void InitDatabase()
    {
        _sqliteConnection.Open();
        CreatedDataTable();
        _sqliteConnection.Close();
        SqliteConnection.ClearPool(_sqliteConnection);
    }
    public void InsertData(byte[] index, Stream readableStream)
    {
        try
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
    private void CreatedDataTable()
    {
        string sqltext = $"CREATE TABLE {_tableName} ({_indexName} BLOB PRIMARY KEY, {_dataName} BLOB)";
        using (var command = new SqliteCommand(sqltext, _sqliteConnection))
        {
            command.ExecuteNonQuery();
        }
    }

    public bool IsIndexExists(byte[] index)
    {
        try
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
        return false;
    }
}
