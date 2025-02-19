# edscriptSharp

## Short Description

Provides data encryption functionality using the database as storage.

## Description

Provides a data encryption system that uses the database as storage.

This system uses encryption algorithms such as AES, etc., and further uses random combinations of encryption algorithms for multiple encryptions. Passwords (or more precisely, their hash values) are used in the creation of seed values for this purpose. This randomization of the encryption method makes decryption by an attacker difficult.

In addition, when the encrypted data is registered in database, the hash from specified name (IndexName) is registered as a index. This hash calculation method is also randomized in the same way.

## Usage

```pwsh
# Simple example of encryption/decryption.

Import-Module PsEdScript
$dbPath = "./path/of/database/file.db"

# Encryption

## Encrypt a text file
Get-Content -Path "./plaintext.txt" | Set-PsEdScript -IndexName "example name" -Path $dbPath
#### password: ********

## Encrypt a string
"plaintext" | Set-PsEdScript -IndexName "name for strnig data" -Path $dbPath
#### password: ********

## Encrypt a file as a binary data (byte[])
Get-Content -Path "./binary.zip" -AsByteStream | Set-PsEdScript -IndexName "name for binary data" -Path $dbPath
#### password: ********

# Decryption

# Decrypt a string data
$result = Get-PsEdScript -IndexName "name for strnig data" -Path $dbPath
#### password: ********
#### # $result is a string type variable

# Decrypt a byte[] data
$result = Get-PsEdScript -IndexName "name for binary data" -Path $dbPath -AsByteStream
#### > password: ********
#### # $result is a byte[] type variable
```

```C#
```
