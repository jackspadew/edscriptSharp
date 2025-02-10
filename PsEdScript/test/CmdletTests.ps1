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
        [byte[]]$exampleByteArray = 0,1,2,3
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
        $logic = [LibEd.EdData.BasicEdDataLogicFactory]::new($dbPath, $examplePassword, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1)
        $script:PsEdScriptLogic = $null
    }

    BeforeEach {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
        Remove-Item $textPath -ErrorAction SilentlyContinue
    }

    AfterAll {
        Remove-Item $dbPath -ErrorAction SilentlyContinue
        Remove-Item $textPath -ErrorAction SilentlyContinue
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
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic -Binary | Should -BeOfType [byte[]]
        }
        It 'Will return correct string.' {
            StashHello
            Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic | Should -Be $exampleData
        }
        It 'Will return correct byte array.' {
            StashBytes
            $result = Get-PsEdScript -IndexName $exampleIndex -EdDataLogicObject $logic -Binary
            $result | Should -Be $exampleByteArray
        }
        It 'Execute with default EdDataLogicFactory object then return correct string.' {
            $exampleData | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            Get-PsEdScript -IndexName $exampleIndex -Path $dbPath | Should -Be $exampleData
        }
        It 'Execute with default EdDataLogicFactory object and byte array then return correct string.' {
            $exampleByteArray | Set-PsEdScript -IndexName $exampleIndex -Path $dbPath
            $result = Get-PsEdScript -IndexName $exampleIndex -Path $dbPath -Binary
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
    }
}
