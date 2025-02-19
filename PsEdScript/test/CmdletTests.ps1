using module Pester

Describe 'PsEdScript_CmdletTests' {
    BeforeAll {
        $env:PsEdScriptDatabasePath = $null
        $script:PsEdScriptLogic = $null
        $dbPath = "example.db";
        $examplePassword = "password"
        $exampleIndex = "hello"
        $exampleData = "world"
        $textPath = "hello.txt"
        $binaryFilePath = "example.binary"
        [byte[]]$exampleByteArray = 0,1,2,3
        [System.IO.File]::WriteAllBytes($binaryFilePath, $exampleByteArray)
        Mock -CommandName "Read-Host" -MockWith { return $examplePassword }
        function StashHello {
            $exampleData | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
        }
        function StashBytes {
            $exampleByteArray | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
        }
        function CreateTextFile {
            $multipleLineString = GenerateStringArray
            $multipleLineString | Set-Content $textPath
        }
        function GenerateStringArray {
            $multipleLineString = foreach($num in (0..10)){ "hello${num}" }
            return $multipleLineString
        }
        $logic = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword
        $script:PsEdScriptLogic = $null
    }

    BeforeEach {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
        Remove-Item $textPath -ErrorAction SilentlyContinue
    }

    AfterAll {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
        Remove-Item $textPath -ErrorAction SilentlyContinue
        Remove-Item $binaryFilePath -ErrorAction SilentlyContinue
    }

    Context 'SetPsEdScript' {
        It 'Will not throw' {
            { StashHello } | Should -Not -Throw
        }
        It 'Will not throw' {
            { StashBytes } | Should -Not -Throw
        }
        It 'Get-Content then execute, then it will not throw.' {
            CreateTextFile
            { Get-Content $textPath | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic } | Should -Not -Throw
        }
        It 'Get-Content with "-AsByteStream" then execute, then it will not throw.' {
            CreateTextFile
            { Get-Content $binaryFilePath -AsByteStream | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic } | Should -Not -Throw
        }
        It 'Execute with default EdDataLogicFactory object then not throw.' {
            { $exampleData | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath } | Should -Not -Throw
        }
        It 'Execute with EdDataLogicFactory object and byte array then not throw.' {
            { $exampleByteArray | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic } | Should -Not -Throw
        }
        It 'Execute with script scope EdDataLogicFactory object then not throw.' {
            $script:PsEdScriptLogic = $logic
            { $exampleData | Set-PsEdScript -IndexName $exampleIndex } | Should -Not -Throw
        }
        It 'Execute with script scope EdDataLogicFactory object and byte array then not throw.' {
            $script:PsEdScriptLogic = $logic
            { $exampleByteArray | Set-PsEdScript -IndexName $exampleIndex } | Should -Not -Throw
        }
    }

    Context 'GetPsEdScript' {
        It 'Will throw if argument IndexName is empty string.' {
            { Get-PsEdScript -IndexName "" -EdDataLogicObject $logic } | Should -Throw
        }
        It 'Will return [string].' {
            StashHello
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic | Should -BeOfType [string]
        }
        It 'Will return [byte[]].' {
            StashBytes
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic -AsByteStream | Should -BeOfType [byte[]]
        }
        It 'Will return correct string.' {
            StashHello
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic | Should -Be $exampleData
        }
        It 'Will return correct byte array.' {
            StashBytes
            $result = Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic -AsByteStream
            $result | Should -Be $exampleByteArray
        }
        It 'Execute with default EdDataLogicFactory object then return correct string.' {
            $exampleData | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            Get-PsEdScript -IndexName $exampleIndex -Path $dbPath | Should -Be $exampleData
        }
        It 'Execute with default EdDataLogicFactory object and byte array then return correct string.' {
            $exampleByteArray | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result = Get-PsEdScript -IndexName $exampleIndex -Path $dbPath -AsByteStream
            $result | Should -Be $exampleByteArray
        }
        It 'Execute with script scope EdDataLogicFactory object return correct string.' {
            $script:PsEdScriptLogic = $logic
            $exampleData | Set-PsEdScript -IndexName $exampleIndex
            Get-PsEdScript -IndexName $exampleIndex | Should -Be $exampleData
        }
        It 'Set text file then Get return [string].' {
            CreateTextFile
            Get-Content $textPath | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic | Should -BeOfType [string]
        }
        It 'Set text file then Get return correct data.' {
            $stringArray = (GenerateStringArray | Out-String) -replace '\r?\n$',''
            CreateTextFile
            Get-Content $textPath | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic | Should -Be $stringArray
        }
        It 'Set binary file then Get return correct data.' {
            Get-Content $binaryFilePath -AsByteStream | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic -AsByteStream
            $result | Should -Be $exampleByteArray
        }
    }

    Context 'InvokePsEdScript' {
        It 'Will return correct string' {
            "#! pwsh`nWrite-Output ""${exampleData}""" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result | Should -Be $exampleData
        }
        It 'Invoke python script will return correct string.' {
            "#! /usr/bin/python`nprint(""${exampleData}"")" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result | Should -Be $exampleData
        }
        It 'Execute with default EdDataLogicFactory object then return correct string.' {
            "#! pwsh`nWrite-Output ""${exampleData}""" | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result = Invoke-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result | Should -Be $exampleData
        }
        It 'Use "Get-PsEdScript" in encrypted script then return correct string' {
            $exampleData | Set-PsEdScript -IndexName "TextData" -EdDataLogicObject $logic
            "#! pwsh`nWrite-Output `$(Get-PsEdScript -IndexName ""TextData"")" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result | Should -Be $exampleData
        }
        It 'Execute with script scope EdDataLogicFactory object return correct string.' {
            $script:PsEdScriptLogic = $logic
            "#! pwsh`nWrite-Output ""${exampleData}""" | Set-PsEdScript -IndexName $exampleIndex
            $result = Invoke-PsEdScript -IndexName $exampleIndex
            $result | Should -Be $exampleData
        }
        It 'Execute Invoke-PsEdScript then script scope obj will back to its previous state.' {
            $script:PsEdScriptLogic = $null
            "#! pwsh`nWrite-Output ""This is valid script""" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $script:PsEdScriptLogic -eq $null | Should -BeTrue
        }
        It 'Execute Invoke-PsEdScript for invalid data, then after throw error the script scope obj will back to its previous state.' {
            $script:PsEdScriptLogic = $null
            "No Shebang to let Cmdlet throw." | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            try
            {
                $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            }catch {}
            $script:PsEdScriptLogic -eq $null | Should -BeTrue
        }
        It 'Execute Invoke-PsEdScript with undefined arguments then not throw.' -Tag "args" {
            $script:PsEdScriptLogic = $null
            "#! pwsh`nWrite-Output ""This is valid script""" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            { Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic "undefined args" 1 123 "undefined args" } | Should -Not -Throw
        }
        It 'Execute Invoke-PsEdScript with undefined arguments then invoked script can use a args.' -Tag "args" {
            $script:PsEdScriptLogic = $null
            $messageGivenToStashedScript = "The messages given to encrypted script"
            "#! pwsh`r`n Param([parameter(mandatory=`$true)][string]`$message)`r`n Write-Output `$message" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic $messageGivenToStashedScript
            $result | Should -Be $messageGivenToStashedScript
        }
        It 'Execute Invoke-PsEdScript with undefined arguments then invoked script can use two args.' -Tag "args" {
            $script:PsEdScriptLogic = $null
            $messageGivenToStashedScript_1 = "message1"
            $messageGivenToStashedScript_2 = "The messages given to encrypted script"
            "#! pwsh`r`n Param([parameter(mandatory=`$true)][string]`$arg1, [parameter(mandatory=`$true)][string]`$message)`r`n if(`$arg1 -eq ""message1""){ Write-Output `$message }" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic $messageGivenToStashedScript_1 $messageGivenToStashedScript_2
            $result | Should -Be $messageGivenToStashedScript_2
        }
        It 'Execute Invoke-PsEdScript with undefined arguments then invoked PYTHON script can use a args.' -Tag "args" {
            $script:PsEdScriptLogic = $null
            $messageGivenToStashedScript = "The messages given to encrypted script"
            "#! python`r`nimport sys`n`nif __name__ == ""__main__"":`n`tprint(sys.argv[1])" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic $messageGivenToStashedScript
            $result | Should -Be $messageGivenToStashedScript
        }
        It 'Execute Invoke-PsEdScript with undefined arguments then invoked PYTHON script can use two args.' -Tag "args" {
            $script:PsEdScriptLogic = $null
            $messageGivenToStashedScript_1 = "message1"
            $messageGivenToStashedScript_2 = "The messages given to encrypted script"
            "#! python`r`nimport sys`n`nif __name__ == ""__main__"":`n`tprint(sys.argv[2])" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic $messageGivenToStashedScript_1 $messageGivenToStashedScript_2
            $result | Should -Be $messageGivenToStashedScript_2
        }
    }

    Context 'NewPsEdScriptLogicObj with valid value' -ForEach @(
            @{ hashCount = 10; multipleEncryptionCount=10; fakeInsertionCount=10 },
            @{ hashCount = 1; multipleEncryptionCount=10; fakeInsertionCount=10 },
            @{ hashCount = 10; multipleEncryptionCount=1; fakeInsertionCount=10 },
            @{ hashCount = 10; multipleEncryptionCount=10; fakeInsertionCount=0 },
            @{ hashCount = 1; multipleEncryptionCount=1; fakeInsertionCount=0 },
            @{ hashCount = 1000000; multipleEncryptionCount=1000000; fakeInsertionCount=1000000 }
            ) {
        It "Create a logic object with specified HashStretchingCount value will not throw." -Tag "NewPsEdScriptLogicObj" {
            {
                $resultObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword -HashStretchingCount $hashCount
            } | Should -Not -Throw -Because "Values(`$hashCount=${hashCount}, `$multipleEncryptionCount=${multipleEncryptionCount}, `$fakeInsertionCount=${fakeInsertionCount})"
        }
        It "Create a logic object with specified MultipleEncryptionCount value will not throw." -Tag "NewPsEdScriptLogicObj" {
            {
                $resultObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword -MultipleEncryptionCount $multipleEncryptionCount
            } | Should -Not -Throw -Because "Values(`$hashCount=${hashCount}, `$multipleEncryptionCount=${multipleEncryptionCount}, `$fakeInsertionCount=${fakeInsertionCount})"
        }
        It "Create a logic object with specified FakeInsertionCount value will not throw." -Tag "NewPsEdScriptLogicObj" {
            {
                $resultObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword -FakeInsertionCount $fakeInsertionCount
            } | Should -Not -Throw -Because "Values(`$hashCount=${hashCount}, `$multipleEncryptionCount=${multipleEncryptionCount}, `$fakeInsertionCount=${fakeInsertionCount})"
        }
        It "Create a logic object with specified all arguments value will not throw." -Tag "NewPsEdScriptLogicObj" {
            {
                $resultObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword `
                    -HashStretchingCount $hashCount `
                    -MultipleEncryptionCount $multipleEncryptionCount `
                    -FakeInsertionCount $fakeInsertionCount
            } | Should -Not -Throw -Because "Values(`$hashCount=${hashCount}, `$multipleEncryptionCount=${multipleEncryptionCount}, `$fakeInsertionCount=${fakeInsertionCount})"
        }
    }
    Context 'NewPsEdScriptLogicObj with invalid value' -ForEach @(
            @{ hashCount = -10; multipleEncryptionCount=-10; fakeInsertionCount=-10 },
            @{ hashCount = 0; multipleEncryptionCount=10; fakeInsertionCount=10 },
            @{ hashCount = 10; multipleEncryptionCount=0; fakeInsertionCount=10 },
            @{ hashCount = 10; multipleEncryptionCount=10; fakeInsertionCount=-1 },
            @{ hashCount = -1000000; multipleEncryptionCount=-1000000; fakeInsertionCount=-1000000 }
            ) {
        It "Create a logic object with invalid all arguments value will throw." -Tag "NewPsEdScriptLogicObj" {
            {
                $resultObj = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword `
                    -HashStretchingCount $hashCount `
                    -MultipleEncryptionCount $multipleEncryptionCount `
                    -FakeInsertionCount $fakeInsertionCount
            } | Should -Throw -Because "Values(`$hashCount=${hashCount}, `$multipleEncryptionCount=${multipleEncryptionCount}, `$fakeInsertionCount=${fakeInsertionCount})"
        }
    }
    Context 'NewPsEdScriptLogicObj with lower limit value' -ForEach @(
            @{ hashCount = 10; multipleEncryptionCount=10; fakeInsertionCount=10 },
            @{ hashCount = 1; multipleEncryptionCount=10; fakeInsertionCount=10 },
            @{ hashCount = 10; multipleEncryptionCount=1; fakeInsertionCount=10 },
            @{ hashCount = 10; multipleEncryptionCount=10; fakeInsertionCount=0 },
            @{ hashCount = 1; multipleEncryptionCount=1; fakeInsertionCount=0 }
            ) {
        It "Encrypt then Decrypt and Invoke with logic object that created by NewPsEdScriptLogicObj will not throw." -Tag "NewPsEdScriptLogicObj" {
            $customLogic = New-PsEdScriptLogicObj -Path $dbPath -Password $examplePassword `
                    -HashStretchingCount $hashCount `
                    -MultipleEncryptionCount $multipleEncryptionCount `
                    -FakeInsertionCount $FakeInsertionCount
            $valuesInfo = "Values(`$hashCount=${hashCount}, `$multipleEncryptionCount=${multipleEncryptionCount}, `$fakeInsertionCount=${fakeInsertionCount})"
            $exampleContent = "message"
            $indexOfExampleContent = "content"
            $exampleScript = "#! pwsh`nWrite-Output `$(Get-PsEdScript -IndexName ""${indexOfExampleContent}"")"
            $indexOfScript = "script"
            $exampleContent | Set-PsEdScript -IndexName $indexOfExampleContent -EdDataLogicObject $customLogic
            $exampleScript | Set-PsEdScript -IndexName $indexOfScript -EdDataLogicObject $customLogic
            $resultGet = Get-PsEdScript -IndexName $indexOfExampleContent -EdDataLogicObject $customLogic
            $resultInvoke = Invoke-PsEdScript -IndexName $indexOfScript -EdDataLogicObject $customLogic
            $resultGet | Should -Be $exampleContent -Because $valuesInfo
            $resultInvoke | Should -Be $exampleContent -Because $valuesInfo
        }
    }
    Context 'NewPsEdScriptLogicObj interface tests' -Tag "NewPsEdScriptLogicObj Interface"{
        BeforeEach {
            $env:PsEdScriptDatabasePath = $null
            $script:PsEdScriptLogic = $null
        }
        It "Called without any arguments without env database Path then throw." {
            {
                New-PsEdScriptLogicObj
            } | Should -Throw
        }
        It "Called without any arguments but with env database Path then not throw." {
            $env:PsEdScriptDatabasePath = Join-Path $PSScriptRoot "example.db"
            {
                New-PsEdScriptLogicObj
            } | Should -Not -Throw
        }
        It "Called with Password then not throw." {
            $env:PsEdScriptDatabasePath = Join-Path $PSScriptRoot "example.db"
            {
                New-PsEdScriptLogicObj -Password $examplePassword
            } | Should -Not -Throw
        }
        It "Called with logic values then not throw." {
            $env:PsEdScriptDatabasePath = Join-Path $PSScriptRoot "example.db"
            {
                New-PsEdScriptLogicObj `
                    -HashStretchingCount 10 `
                    -MultipleEncryptionCount 10 `
                    -FakeInsertionCount 10
            } | Should -Not -Throw
        }
    }
}
