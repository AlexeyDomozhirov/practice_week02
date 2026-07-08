namespace FileSystemCommands;

using CommandLib;
using task07;

[DisplayName("Команда поиска файлов по маске")]
[Version(1, 0)]
public class FindFilesCommand : ICommand
{
    public string DirPath { get; set; }
    public string SearchPattern { get; set; }

    public FindFilesCommand(string DirectoryPath, string SearchPattern)
    {
        this.DirPath = DirectoryPath;
        this.SearchPattern = SearchPattern;
    }

    [DisplayName("Выполнить поиск файлов по маске")]
    public void Execute()
    {
        if (!Directory.Exists(DirPath))
        {
            throw new DirectoryNotFoundException();
        }

        try
        {
            var files = Directory.GetFiles(DirPath, SearchPattern, SearchOption.AllDirectories);
            Console.WriteLine($"Found {files.Length} files matching '{SearchPattern}' in '{DirPath}':");
            foreach (var file in files)
                Console.WriteLine(file);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error: {ex.Message}");
        }
    }
}
