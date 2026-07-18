namespace FileSystemCommands;

using CommandLib;

public class DirectorySizeCommand : ICommand
{
    public string DirPath { get; set; }

    public DirectorySizeCommand(string dirPath)
    {
        DirPath = dirPath;
    }

    public void Execute()
    {
        if (!Directory.Exists(DirPath))
        {
            throw new DirectoryNotFoundException();
        }

        Console.WriteLine($"Size of directory '{DirPath}': {CalculateDirectorySize(DirPath)} bytes");
    }

    private long CalculateDirectorySize(string path)
    {
        long total = 0;

        foreach (var file in Directory.GetFiles(path))
        {
            total += new FileInfo(file).Length;
        }
        foreach (var dir in Directory.GetDirectories(path))
        {
            total += CalculateDirectorySize(dir);
        }

        return total;
    }
}
