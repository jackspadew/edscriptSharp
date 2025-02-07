# Note about tests

# LibEd.Tests

## LibEd.Tests.RandomizedHashCalculator_Tests.GenerateHashAnotherSeed_ReturnAnotherHash

このテストはスキップされる。
利用可能なアルゴリズムが１つだけなので、ランダマイズできない（なのでテストも実行できない）ため。

# LibEd.PerfTests

パフォーマンステストです。
パフォーマンステストは既定では全てスキップされます。
MSBuildのプロパティ`PerformanceTests`に`enable`を設定することでパフォーマンステストが実行されます。

目標タイムをクリアした場合にテストをパスする。
計測時間を表示する場合にはオプション`--logger "console;verbosity=detailed"`を付けて実行すること。

パフォーマンステストの実行コマンドは次の通り。

```pwsh
dotnet test LibEd.PerfTests --logger "console;verbosity=detailed" -p:PerformanceTests="enable"
```

# PsEdScript.Tests

PsEdScriptの共通メソッドのテストのみを行う。

# PsEdScript

PsEdScriptはpsmoduleプロジェクトである。
testディレクトリにPesterスクリプトがある。
Pesterテストを実行する際には`HowToBuildAndInvokingPesterTest.md`を参照すること。
