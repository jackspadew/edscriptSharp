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

    [Theory]
    [InlineData(32)]
    public void UsingReadMethod_ReturnSaltedBytes(int bufferSize)
    {
        MemoryStream sourceStream = new MemoryStream(exampleBytes);
        SaltStream saltStream = new SaltStream(sourceStream, exampleSalt, new long[]{0}, false);
        int readCount;
        byte[] buffer = new byte[bufferSize];
        List<byte> resultList = new();
        int count = 0;
        while((readCount = saltStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            resultList.AddRange(buffer[0..readCount]);
        }
        byte[] expectedBytes = new byte[exampleBytes.Length + exampleSalt.Length];
        Array.Copy(exampleSalt, 0, expectedBytes, 0, exampleSalt.Length);
        Array.Copy(exampleBytes, 0, expectedBytes, exampleSalt.Length, exampleBytes.Length);
        Assert.Equal(expectedBytes, resultList.ToArray());
    }
}
