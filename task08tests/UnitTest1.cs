namespace task08tests;
using FileSystemCommands;

using System;
using System.IO;
using Xunit;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldExecuteWithoutErrors()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test.txt"), "Hello");
        var command = new DirectorySizeCommand(testDir);

        var exception = Record.Exception(() => command.Execute());
        Assert.Null(exception);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldExecuteWithoutErrors()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file.txt"), "Text");
        var command = new FindFilesCommand(testDir, "*.txt");

        var exception = Record.Exception(() => command.Execute());
        Assert.Null(exception);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_ShouldOutputCorrectSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var filePath = Path.Combine(testDir, "test.txt");
        File.WriteAllText(filePath, "Hello"); // 5 байт

        var command = new DirectorySizeCommand(testDir);

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        command.Execute();

        Console.SetOut(Console.Out);

        var output = consoleOutput.ToString();
        Assert.Contains("5 bytes", output);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldOutputFoundFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var file1 = Path.Combine(testDir, "file1.txt");
        var file2 = Path.Combine(testDir, "file2.log");
        File.WriteAllText(file1, "Text1");
        File.WriteAllText(file2, "Log");

        var command = new FindFilesCommand(testDir, "*.txt");

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        command.Execute();

        Console.SetOut(Console.Out);

        var output = consoleOutput.ToString();
        Assert.Contains("Found 1 files", output);
        Assert.Contains(file1, output);
        Assert.DoesNotContain(file2, output);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_NonexistentDirectory_ShouldOutputNotFound()
    {
        var nonExistent = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var command = new DirectorySizeCommand(nonExistent);

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        command.Execute();

        Console.SetOut(Console.Out);

        var output = consoleOutput.ToString();
        Assert.Contains("Directory not found", output);
    }
}
