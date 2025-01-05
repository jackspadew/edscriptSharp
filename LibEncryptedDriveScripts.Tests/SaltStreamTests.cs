namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.SaltStream;

using System;
using System.IO;
using System.Text;

public class SaltStream_Tests
{
    public static string message = "text";
    public static string saltString = "SALT";

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
}
