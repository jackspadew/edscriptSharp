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

----

Invoking encrypted script that using encrypted data.

```pwsh
Import-Module PsEdScript
$dbPath = "./example.db"
[string]$examplePassword = "password"
$logicObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword

$ps1Text_UsingEncryptedData = @"
#! pwsh
if(`$script:PsEdScriptLogic -ne `$null){ Write-Host "```$script:PsEdScriptLogic is not ```$null." }
`$secretMessage = Get-PsEdScript -IndexName "SecretMessage"
Write-Output `$secretMessage
"@

# Build encrypted database.
"secret message" | Set-PsEdScript -IndexName "SecretMessage" -EdDataLogicObject $logicObj
$ps1Text_UsingEncryptedData | Set-PsEdScript -IndexName "ScriptThatUsingEncryptedData" -EdDataLogicObject $logicObj

# Invoking
## This line is For example, clearly stated that $script:PsEdScriptLogic is null.
$script:PsEdScriptLogic = $null
## Invoke encrypted script
Invoke-PsEdScript -IndexName "ScriptThatUsingEncryptedData" -EdDataLogicObject $logicObj
## > $script:PsEdScriptLogic is not $null.
## > secret message
if($script:PsEdScriptLogic -eq $null){ Write-Host "`$script:PsEdScriptLogic is `$null." }
## > $script:PsEdScriptLogic is $null.
```

Explanation:
- $script:PsEdScriptLogic is set to the logic object that used for decryption by "Invoke-PsEdScript".
- Then "Invoke-PsEdScript" revert it back.
- This allows to access the encrypted data in the encrypted script process without creating a logic object (it means no need re-entering the password).

#### Advanced example: Invoking encrypted script

Invoking the encrypted script from the encrypted script with another logic object.

```pwsh
Import-Module PsEdScript

$env:PsEdScriptDatabasePath = "./example.db"
[string]$examplePassword = "password"
[string]$anotherPassword = "another password"
$mainLogicObj = New-PsEdScriptLogicObj -Password $examplePassword
$anotherLogicObj = New-PsEdScriptLogicObj -Password $anotherPassword

# <-- Setup code

## Encrypt script
$ps1Text_MainScript = @"
#! pwsh
`$anotherPassword = Get-PsEdScript -IndexName "AnotherPassword"
`$tmplogicObj = New-PsEdScriptLogicObj -Password `$anotherPassword

Write-Output `$(Invoke-PsEdScript -IndexName "AnotherScript" -EdDataLogicObject `$tmplogicObj)
"@
$ps1Text_MainScript | Set-PsEdScript -IndexName "ScriptThatInvokingScriptWithAnotherLogicObj" -EdDataLogicObject $mainLogicObj
## Encrypt the another password string
$anotherPassword | Set-PsEdScript -IndexName "AnotherPassword" -EdDataLogicObject $mainLogicObj

# Encrypt with $anotherLogicObj
## Encrypt another script
$ps1Text_AnotherScript = @"
#! pwsh
`$secretMessage = Get-PsEdScript -IndexName "SecretMessage"
Write-Output `$secretMessage
"@
$ps1Text_AnotherScript | Set-PsEdScript -IndexName "AnotherScript" -EdDataLogicObject $anotherLogicObj
## Encrypt the secret message
"Secret Message" | Set-PsEdScript -IndexName "SecretMessage" -EdDataLogicObject $anotherLogicObj

# Setup code -->

# Invoke code
$logicObj = New-PsEdScriptLogicObj -Password $examplePassword
Invoke-PsEdScript -IndexName "ScriptThatInvokingScriptWithAnotherLogicObj" -EdDataLogicObject $logicObj
## > Secret Message
```

### C#

```C#
```

## License

This repository is licensed under the MIT license.

And the notices for third-party components are listed in "NOTICES.md".
