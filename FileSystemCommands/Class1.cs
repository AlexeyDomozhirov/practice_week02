namespace FileSystemCommands;

using System.IO;
using CommandLib;

public class DirectorySizeCommand : ICommand
{
    public string dir_path;

    public DirectorySizeCommand(string _dir_path)
    {
        dir_path = _dir_path;
    }

    public void Execute()
    {
        if (!Directory.Exists(dir_path))
        {
            Console.WriteLine($"Directory not found: {dir_path}");
            return;
        }

        Console.WriteLine($"Size of directory '{dir_path}': {CalculateDirectorySize(dir_path)} bytes");
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

public class FindFilesCommand : ICommand
{
    public string dir_path;
    public string search_pattern;

    public FindFilesCommand(string _directoryPath, string _searchPattern)
    {
        dir_path = _directoryPath;
        search_pattern = _searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(dir_path))
        {
            Console.WriteLine($"Directory not found: {dir_path}");
            return;
        }

        try
        {
            var files = Directory.GetFiles(dir_path, search_pattern, SearchOption.AllDirectories);
            Console.WriteLine($"Found {files.Length} files matching '{search_pattern}' in '{dir_path}':");
            foreach (var file in files)
                Console.WriteLine(file);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error: {ex.Message}");
        }
    }
}
