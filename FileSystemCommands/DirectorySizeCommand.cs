namespace FileSystemCommands;

using CommandLib;
using task07;

[DisplayName("Команда вычисления размера директории")]
[Version(1, 0)]
public class DirectorySizeCommand : ICommand
{
    public string DirPath { get; set; }

    public DirectorySizeCommand(string DirPath)
    {
        this.DirPath = DirPath;
    }

    [DisplayName("Вычислить размер директории")]
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
