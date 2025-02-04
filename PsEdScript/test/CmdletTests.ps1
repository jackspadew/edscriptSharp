using module Pester

$modulePath = Join-Path -Path (Split-Path -Parent $MyInvocation.MyCommand.Path) -ChildPath '../bin/Debug/net8.0/PsEdScript.dll'
Import-Module $modulePath

Describe 'PsEdScript_CmdletTests' {
    BeforeAll {
        $dbPath = "example.db";
        Remove-Item $dbPath
        Mock -CommandName "Read-Host" -MockWith { return "Mocked Input" }
    }

    Context 'SetPsEdScript' {
        It 'Will not throw' {
            { "world" | Set-PsEdScript -IndexName "hello" -Path $dbPath } | Should -Not -Throw
        }
    }

    Context 'GetPsEdScript' {
        It 'Will throw if argument IndexName is empty string.' {
            { Get-PsEdScript -IndexName "" -Path $dbPath } | Should -Throw
        }
        It 'Will return [string].' {
            Get-PsEdScript -IndexName "hello" -Path $dbPath | Should -BeOfType [string]
        }
        It 'Will return "world".' {
            Get-PsEdScript -IndexName "hello" -Path $dbPath | Should -Be "world"
        }
    }
}
