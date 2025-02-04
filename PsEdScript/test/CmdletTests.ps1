using module Pester

$modulePath = Join-Path -Path (Split-Path -Parent $MyInvocation.MyCommand.Path) -ChildPath '../bin/Debug/net8.0/PsEdScript.dll'
Import-Module $modulePath

Describe 'PsEdScript_CmdletTests' {
    Context 'GetPsEdScript' {
        It 'Will throw if argument IndexName is empty string.' {
            { Get-PsEdScript -IndexName "" -Path "example.db" } | Should -Throw
        }
        It 'Will return [string].' {
            Get-PsEdScript -IndexName "abc" -Path "example.db" | Should -BeOfType [string]
        }
    }
}
