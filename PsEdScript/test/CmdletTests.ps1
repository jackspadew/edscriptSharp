using module Pester

$modulePath = Join-Path -Path (Split-Path -Parent $MyInvocation.MyCommand.Path) -ChildPath '../bin/Debug/net8.0/PsEdScript.dll'
Import-Module $modulePath
$LibEdPath = Join-Path -Path (Split-Path -Parent $MyInvocation.MyCommand.Path) -ChildPath '../bin/Debug/net8.0/LibEd.dll'
Import-Module $LibEdPath

Describe 'PsEdScript_CmdletTests' {
    BeforeAll {
        $dbPath = "example.db";
        $examplePassword = "password"
        $exampleIndex = "hello"
        $exampleData = "world"
        [byte[]]$exampleByteArray = 0,1,2,3
        Mock -CommandName "Read-Host" -MockWith { return $examplePassword }
        function StashHello {
            $exampleData | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
        }
        function StashBytes {
            $exampleByteArray | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
        }
        $logic = [LibEd.EdData.BasicEdDataLogicFactory]::new($dbPath, $examplePassword)
        $script:PsEdScriptLogic = $null
    }

    BeforeEach {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
    }

    AfterAll {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
    }

    Context 'SetPsEdScript' {
        It 'Will not throw' {
            { StashHello } | Should -Not -Throw
        }
        It 'Will not throw' {
            { StashBytes } | Should -Not -Throw
        }
        It 'Execute with EdDataLogicFactory object then not throw.' {
            { $exampleData | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic } | Should -Not -Throw
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
            { Get-PsEdScript -IndexName "" -Path $dbPath } | Should -Throw
        }
        It 'Will return [string].' {
            StashHello
            Get-PsEdScript -IndexName $exampleIndex -Path $dbPath | Should -BeOfType [string]
        }
        It 'Will return correct string.' {
            StashHello
            Get-PsEdScript -IndexName $exampleIndex -Path $dbPath | Should -Be $exampleData
        }
        It 'Execute with EdDataLogicFactory object then return correct string.' {
            $exampleData | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic | Should -Be $exampleData
        }
        It 'Execute with script scope EdDataLogicFactory object return correct string.' {
            $script:PsEdScriptLogic = $logic
            $exampleData | Set-PsEdScript -IndexName $exampleIndex
            Get-PsEdScript -IndexName $exampleIndex | Should -Be $exampleData
        }
    }

    Context 'InvokePsEdScript' {
        It 'Will return correct string' {
            "#! pwsh`nWrite-Output ""${exampleData}""" | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result = Invoke-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result | Should -Be $exampleData
        }
        It 'Invoke python script will return correct string.' {
            "#! /usr/bin/python`nprint(""${exampleData}"")" | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result = Invoke-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result | Should -Be $exampleData
        }
        It 'Execute with EdDataLogicFactory object then return correct string.' {
            "#! pwsh`nWrite-Output ""${exampleData}""" | Set-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic
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
    }
}
