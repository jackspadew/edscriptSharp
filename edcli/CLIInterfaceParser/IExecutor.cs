namespace edcli.CommandLineParser;

public interface IExecutor
{
    void Stash(FileInfo dbFileInfo, string indexName, FileInfo stashTargetFileInfo, string? nullablePassword);
    void Extract(FileInfo dbFileInfo, string indexName, string? nullablePassword);
}
