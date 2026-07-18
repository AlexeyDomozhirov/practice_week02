namespace FileSystemCommands;

using CommandLib;

public class FindFilesCommand : ICommand
{
    public string DirPath { get; set; }
    public string SearchPattern { get; set; }

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        DirPath = directoryPath;
        SearchPattern = searchPattern;
    }

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
