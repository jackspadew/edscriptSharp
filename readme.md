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

```pwsh
Import-Module PsEdScript

# Encrypt
## This line will prompt you for a password.
"Hello world!" | Set-PsEdScript -IndexName "hello" -Path "example.db"
## > password: ****

# Decrypt
## This line will prompt you for a password.
Get-PsEdScript -IndexName "hello" -Path "example.db"
## > password: ****
## > Hello world!
```

#### Example encryption script

```pwsh
# Example script to build the database of encrypted data.

Import-Module PsEdScript

$dbPath = "./example.db"

## Create the logic object: "New-PsEdScriptLogicObj"
##   The "-Password" is optional. If not specified, "Read-Host" will be called.
[string]$examplePassword = "password"
$logicObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword

## Set the logic object to $script:PsEdScriptLogic
##   Cmdlets use the value of $script:PsEdScriptLogic as default value.
$script:PsEdScriptLogic = $logicObj

## Encrypt: "Set-PsEdScript"
"example [string] type value" | Set-PsEdScript -IndexName "name for the [string] data"
Get-Content -Path "./plaintext.txt" | Set-PsEdScript -IndexName "name for the text file data"
Get-Content "./binaryfile.zip" -AsByteStream | Set-PsEdScript -IndexName "name for binary data"
```

#### Example decryption script

```pwsh
# Example script to read from the encrypted data database.

Import-Module PsEdScript

$dbPath = "./example.db"

## Create the logic object: "New-PsEdScriptLogicObj"
[string]$examplePassword = "password"
$logicObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword
$script:PsEdScriptLogic = $logicObj

## Decrypt: "Get-PsEdScript"
### decrypt as [string] type data
[string]$decryptedString = Get-PsEdScript -IndexName "name for the [string] data"
[string]$decryptedPlainText = Get-PsEdScript -IndexName "name for the text file data"
### decrypt as [byte[]] type data
[byte[]]$decryptedByteArray = Get-PsEdScript -IndexName "name for binary data" -AsByteStream
```

#### Example invoking encrypted script

Invoking encrypted script with arguments.

```pwsh
# Example scripts
## Need "Shebang" on the first line.
$ps1Text_HasParam = @"
#! pwsh
Param([string]`$message, [int]`$num)
if(`$num){
  Write-Output "`${message} with integer `${num}"
}else{
  Write-Output "`${message}"
}
"@

# Build encrypted database.
$ps1Text_HasParam | Set-PsEdScript -IndexName "ScriptThatHasParam"

# Invoke encrypted script: "Invoke-PsEdScript"
## Invoking "ScriptThatHasParam"
Invoke-PsEdScript -IndexName "ScriptThatHasParam" "This is Message"
## > This is Message
Invoke-PsEdScript -IndexName "ScriptThatHasParam" "This is Message" 3
## > This is Message with integer 3
```

Invoking encrypted script that using encrypted data.

```pwsh
# Example scripts
## Need "Shebang" on the first line.
$ps1Text_UsingEncryptedData = @"
#! pwsh
`$secretMessage = Get-PsEdScript -IndexName "SecretMessage"
Write-Output `$secretMessage
"@

# Build encrypted database.
"secret message" | Set-PsEdScript -IndexName "SecretMessage"
$ps1Text_UsingEncryptedData | Set-PsEdScript -IndexName "ScriptThatUsingEncryptedData"

# Invoke encrypted script: "Invoke-PsEdScript"
## Invoking
Invoke-PsEdScript -IndexName "ScriptThatUsingEncryptedData"
## > secret message
```

### C#

```C#
```
