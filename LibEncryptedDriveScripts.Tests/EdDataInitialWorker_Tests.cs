namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using Moq;
using LibEncryptedDriveScripts.EdData;
using LibEncryptedDriveScripts.Database;

public class EdDataInitialWorker_Tests
{
    public static string dbPath = "EdDataInitialWorker_Tests.db";

    public class Concrete_LogicFactory : IEdDataLogicFactory
    {
        public IEdDataCryptor CreateCryptor(IEdDataWorker worker)
        {
            return new EdDataCryptor();
        }
        public IDatabaseOperator CreateDatabaseOperator(IEdDataWorker worker)
        {
            return new EdDatabaseOperator(dbPath, true);
        }
        public IEdDataHashCalculator CreateHashCalculator(IEdDataWorker worker)
        {
            var mockObj = new Mock<IEdDataHashCalculator>();
            byte[] hash = new byte[512];
            mockObj.Setup(x => x.ComputeHash(It.IsAny<byte[]>(), It.IsAny<IMultipleKeyExchanger>())).Returns(hash);
            return mockObj.Object;
        }
        public IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker worker)
        {
            return new InitialMultipleKeyExchanger();
        }
        public IEdDataWorker CreateWorker()
        {
            throw new NotImplementedException();
        }
    }

    private IEdDataLogicFactory CreateFactory()
    {
        return new Concrete_LogicFactory();
    }

    private void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CreateObject_InitialMultiKeyIsExists()
    {
        DeleteFileIfExists(dbPath);
        var logicFactory = CreateFactory();
        var edWorker = new EdDataInitialWorker(logicFactory);
        bool IsInitialMultiKeyExists = edWorker.IsIndexExists("__InitialMultiKey");
        Assert.True(IsInitialMultiKeyExists);
    }
}
