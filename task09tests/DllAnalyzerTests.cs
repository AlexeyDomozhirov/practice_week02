namespace task09tests;

using System;
using System.Diagnostics;
using Xunit;

public class DllAnalyzerTests
{
    private string RunAnalyzer(string targetDllPath)
    {
        string analyzerDll = typeof(DllAnalyzer.Program).Assembly.Location;
        var startInfo = new ProcessStartInfo("dotnet", $"\"{analyzerDll}\" \"{targetDllPath}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo)!;
        if (process == null)
        {
            throw new InvalidOperationException("Не удалось запустить процесс dotnet");
        }

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Ошибка: {error}");

        return output;
    }

    [Fact]
    public void AnalyzeDirectorySizeCommand_OutputContainsExpectedInfo()
    {
        string targetDll = typeof(FileSystemCommands.DirectorySizeCommand).Assembly.Location;
        string output = RunAnalyzer(targetDll);

        Assert.Contains("Тип: FileSystemCommands.DirectorySizeCommand", output);
        Assert.Contains("Отображаемое имя: Команда вычисления размера директории", output);
        Assert.Contains("Версия: 1.0", output);

        Assert.Contains("- Execute (Вычислить размер директории)", output);
        Assert.Contains("- CalculateDirectorySize", output);
        
        Assert.Contains("- .ctor", output);
        Assert.Contains("Параметры:", output);
        Assert.Contains("String dirPath", output);
    }

    [Fact]
    public void AnalyzeFindFilesCommand_OutputContainsExpectedInfo()
    {
        string targetDll = typeof(FileSystemCommands.FindFilesCommand).Assembly.Location;
        string output = RunAnalyzer(targetDll);

        Assert.Contains("Тип: FileSystemCommands.FindFilesCommand", output);
        Assert.Contains("Отображаемое имя: Команда поиска файлов по маске", output);
        Assert.Contains("Версия: 1.0", output);
        
        Assert.Contains("- Execute (Выполнить поиск файлов по маске)", output);
        
        Assert.Contains("- .ctor", output);
        Assert.Contains("Параметры:", output);
        Assert.Contains("String directoryPath", output);
        Assert.Contains("String searchPattern", output);
    }
}
