namespace LibEncryptedDriveScripts.Database;

using System.IO;
using Microsoft.Data.Sqlite;

public abstract class DatabaseOperatorBase : IDatabaseOperator
{
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
        throw new NotImplementedException();
    }
    public void InsertData(byte[] index, byte[] data)
    {
        throw new NotImplementedException();
    }

    protected void InitDatabase()
    {
        throw new NotImplementedException();
    }
    private bool IsIndexExists(byte[] index)
    {
        throw new NotImplementedException();
    }
}
