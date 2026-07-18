namespace CommandRunner;

using System;
using System.Reflection;
using CommandLib;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: CommandRunner DirectorySize <path>");
            Console.WriteLine("       CommandRunner FindFiles <path> <mask>");
            return;
        }

        try
        {
            var assembly = Assembly.LoadFrom("FileSystemCommands.dll");
        
            ICommand command = args[0].ToLowerInvariant() switch
            {
                "directorysize" => (ICommand)Activator.CreateInstance(
                    assembly.GetType("FileSystemCommands.DirectorySizeCommand")!,
                    new object[] { args[1] })!,
        
                "findfiles" => (ICommand)Activator.CreateInstance(
                    assembly.GetType("FileSystemCommands.FindFilesCommand")!,
                    new object[] { args[1], args[2] })!,
        
                _ => throw new ArgumentException($"Unknown command: {args[0]}")
            };
        
            command.Execute();
        }
        catch (Exception ex) when (ex is FileNotFoundException or BadImageFormatException or FileLoadException)
        {
            Console.WriteLine($"Ошибка загрузки сборки: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения: {ex.Message}");
        }
    }
}
