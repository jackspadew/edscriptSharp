namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.SaltStream;

using System;
using System.IO;
using System.Text;

public class SaltStream_Tests
{
    public static string message = "text";
    public static string saltString = "SALT";
    public static byte[] exampleBytes = {0,0,0,0};
    public static byte[] exampleSalt = {1,1};

    [Fact]
    public void InsertSaltToTail_ReturnSaltedString()
    {
        byte[] sourceBytes = Encoding.UTF8.GetBytes(message);
        byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);
        MemoryStream sourceStream = new MemoryStream(sourceBytes);
        SaltStream saltStream = new SaltStream(sourceStream, saltBytes, new long[]{}, true);
        using (StreamReader reader = new StreamReader(saltStream, Encoding.UTF8))
        {
            string result = reader.ReadToEnd();
            string expected = message + saltString;
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void InsertSaltToHead_ReturnSaltedString()
    {
        byte[] sourceBytes = Encoding.UTF8.GetBytes(message);
        byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);
        MemoryStream sourceStream = new MemoryStream(sourceBytes);
        SaltStream saltStream = new SaltStream(sourceStream, saltBytes, new long[]{0});
        using (StreamReader reader = new StreamReader(saltStream, Encoding.UTF8))
        {
            string result = reader.ReadToEnd();
            string expected = saltString + message;
            Assert.Equal(expected, result);
        }
    }

    private byte[] GetInsertedBytes(byte[] source, byte[] insertData, int position)
    {
        byte[] result = new byte[source.Length + insertData.Length];
        Array.Copy(source, 0, result, 0, position);
        Array.Copy(insertData, 0, result, position, insertData.Length);
        Array.Copy(source, position, result, position+insertData.Length, source.Length-position);
        return result;
    }

    private byte[] GetSaltedBytesUsingReadMethod(byte[] source, byte[] salt, int bufferSize, long insertPos)
    {
        MemoryStream sourceStream = new MemoryStream(exampleBytes);
        SaltStream saltStream = new SaltStream(sourceStream, exampleSalt, new long[]{insertPos}, false);
        int readCount;
        byte[] buffer = new byte[bufferSize];
        List<byte> resultList = new();
        while((readCount = saltStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            resultList.AddRange(buffer[0..readCount]);
        }
        return resultList.ToArray();
    }

    [Theory]
    [InlineData(32,0)]
    [InlineData(32,1)]
    [InlineData(32,3)]
    [InlineData(32,4)]
    [InlineData(7,0)]
    [InlineData(7,4)]
    [InlineData(6,0)]
    [InlineData(6,4)]
    [InlineData(1,0)]
    [InlineData(1,4)]
    public void UsingReadMethod_ReturnSaltedBytes(int bufferSize, int insertPos)
    {
        byte[] result = GetSaltedBytesUsingReadMethod(exampleBytes, exampleSalt, bufferSize, insertPos);
        byte[] expected = GetInsertedBytes(exampleBytes, exampleSalt, insertPos);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(32,5)]
    [InlineData(32,100)]
    [InlineData(1,5)]
    [InlineData(6,5)]
    public void InsertSaltToInvalidPosByUsingReadMethod_ReturnSameBytesAtSource(int bufferSize, int insertPos)
    {
        byte[] result = GetSaltedBytesUsingReadMethod(exampleBytes, exampleSalt, bufferSize, insertPos);
        Assert.Equal(exampleBytes, result);
    }
}
