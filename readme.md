# edscriptSharp

## Short Description

Provides data encryption functionality using the database as storage.

## Description

Provides a data encryption system that uses the database as storage.

This system uses encryption algorithms such as AES, etc., and further uses random combinations of encryption algorithms for multiple encryptions. Passwords (or more precisely, their hash values) are used in the creation of seed values for this purpose. This randomization of the encryption method makes decryption by an attacker difficult.

In addition, when the encrypted data is registered in database, the hash from specified name (IndexName) is registered as a index. This hash calculation method is also randomized in the same way.

## Usage

### PowerShell 7.2 and later

#### Simple encrypt/decrypt code

### C#

```C#
```
