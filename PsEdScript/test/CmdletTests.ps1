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
        Mock -CommandName "Read-Host" -MockWith { return $examplePassword }
        function StashHello {
            "world" | Set-PsEdScript -IndexName "hello" -Path $dbPath
        }
        $logic = [LibEd.EdData.BasicEdDataLogicFactory]::new($dbPath, $examplePassword)
    }

    BeforeEach {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
    }

    Context 'SetPsEdScript' {
        It 'Will not throw' {
            { StashHello } | Should -Not -Throw
        }
        It 'Execute with EdDataLogicFactory object then not throw.' {
            { "world" | Set-PsEdScript -IndexName "hello" -EdDataLogicObject $logic } | Should -Not -Throw
        }
    }

    Context 'GetPsEdScript' {
        It 'Will throw if argument IndexName is empty string.' {
            { Get-PsEdScript -IndexName "" -Path $dbPath } | Should -Throw
        }
        It 'Will return [string].' {
            StashHello
            Get-PsEdScript -IndexName "hello" -Path $dbPath | Should -BeOfType [string]
        }
        It 'Will return "world".' {
            StashHello
            Get-PsEdScript -IndexName "hello" -Path $dbPath | Should -Be "world"
        }
        It 'Execute with EdDataLogicFactory object then return correct string.' {
            "world" | Set-PsEdScript -IndexName "hello" -EdDataLogicObject $logic
            Get-PsEdScript -IndexName "hello" -EdDataLogicObject $logic | Should -Be "world"
        }
    }

    Context 'InvokePsEdScript' {
        It 'Will return correct string' {
            "#! pwsh`nWrite-Output ""world""" | Set-PsEdScript -IndexName "hello.ps1" -Path $dbPath
            $result = Invoke-PsEdScript -IndexName "hello.ps1" -Path $dbPath
            $result | Should -Be "world"
        }
        It 'Invoke python script will return correct string.' {
            "#! /usr/bin/python`nprint(""world"")" | Set-PsEdScript -IndexName "hello.py" -Path $dbPath
            $result = Invoke-PsEdScript -IndexName "hello.py" -Path $dbPath
            $result | Should -Be "world"
        }
        It 'Execute with EdDataLogicFactory object then return correct string.' {
            "#! pwsh`nWrite-Output ""world""" | Set-PsEdScript -IndexName "hello.ps1" -EdDataLogicObject $logic
            $result = Invoke-PsEdScript -IndexName "hello.ps1" -EdDataLogicObject $logic
            $result | Should -Be "world"
        }
    }
}
