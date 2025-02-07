# How to build PsEdScript and invoke Pester test

# Answer: Powershell code for invoking Pester test

ビルド実行から所定のコードを使用することでPesterテストを実行する。
以下のpowershellコードはWindows 64bit 環境であることが前提となる。
"win-x64"を指定しビルドを行い、その特定環境用のビルドの出力先のdllモジュールをインポートしている。

```pwsh
cd ./ # move to the solution dir.
dotnet build .\PsEdScript\ -c Debug -r win-x64
pwsh -c {
  Import-Module ".\PsEdScript\bin\Debug\net8.0\win-x64\PsEdScript.dll"
  Import-Module ".\PsEdScript\bin\Debug\net8.0\win-x64\LibEd.dll"
  Invoke-Pester ".\PsEdScript\test\CmdletTests.ps1"
}
```

# issue

アンマネージドDLL"e_sqlite3.dll"のアセンブリが決定されない事が原因で、Pesterテストを実行する時に問題がある。この問題はRuntimeIdentifier（RIDとも。"--runtime"オプションで指定する。）が未指定の場合に発生します。

# Solution 1

+ RIDを指定してビルドする
+ "e_sqlite3.dll"はビルド時にランタイム環境に合わせて自動的に選択され配置される。

- 成果物の出力先が事前に決定されない為、Pesterテスト内でモジュールの読み込みコードを書けない。

# Solution 2

+ RIDを未指定でビルドする。（UseCurrentRuntimeIdentifierもfalseにする。）
+ 成果物の出力先は固定なのでモジュール読み込みも"e_sqlite3.dll"をコピーする処理もスクリプトで組みやすい。

- 安全な解決法ではないだろう。
