namespace task10tests;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

public class PluginLoaderTests
{
    private string GetSolutionRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            if (Directory.GetFiles(dir, "*.sln").Any())
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Не удалось найти файл .sln в родительских папках.");
    }

    private string GetPluginPackPath(string packName)
    {
        var root = GetSolutionRoot();
        var path = Path.Combine(root, packName);
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Папка '{packName}' не найдена по пути '{path}'.");
        return path;
    }

    private string RunPluginLoaderViaDotnetRun(string pluginPackPath)
    {
        var solutionRoot = GetSolutionRoot();
        var projectPath = Path.Combine(solutionRoot, "PluginLoader", "PluginLoader.csproj");
        if (!File.Exists(projectPath))
            throw new FileNotFoundException($"Проект PluginLoader не найден по пути '{projectPath}'.");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" -- \"{pluginPackPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
            throw new InvalidOperationException("Не удалось запустить процесс dotnet.");

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return output + (string.IsNullOrEmpty(error) ? "" : Environment.NewLine + error);
    }

    [Fact]
    public void TestPluginPackA_Order_ShouldBeBThenA()
    {
        string packPath = GetPluginPackPath("PluginPackA");
        string output = RunPluginLoaderViaDotnetRun(packPath);

        var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        int idxLoadB = Array.FindIndex(lines, l => l.Contains("Загрузка плагина: PluginB"));
        int idxExecB = Array.FindIndex(lines, l => l.Contains("PluginB executed!"));
        int idxLoadA = Array.FindIndex(lines, l => l.Contains("Загрузка плагина: PluginA"));
        int idxExecA = Array.FindIndex(lines, l => l.Contains("PluginA executed! depend on PluginB"));
        int idxDone = Array.FindIndex(lines, l => l.Contains("Все плагины выполнены."));

        Assert.True(idxLoadB >= 0, "Строка загрузки PluginB не найдена");
        Assert.True(idxExecB >= 0, "Строка выполнения PluginB не найдена");
        Assert.True(idxLoadA >= 0, "Строка загрузки PluginA не найдена");
        Assert.True(idxExecA >= 0, "Строка выполнения PluginA не найдена");
        Assert.True(idxDone >= 0, "Строка завершения не найдена");

        Assert.True(idxLoadB < idxExecB, "Порядок: загрузка PluginB должна быть до выполнения");
        Assert.True(idxExecB < idxLoadA, "Порядок: выполнение PluginB должно быть до загрузки PluginA");
        Assert.True(idxLoadA < idxExecA, "Порядок: загрузка PluginA должна быть до выполнения");
        Assert.True(idxExecA < idxDone, "Порядок: выполнение PluginA должно быть до завершения");

        Assert.DoesNotContain("Ошибка", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TestPluginPackB_Order_ShouldBeCThenAThenB()
    {
        string packPath = GetPluginPackPath("PluginPackB");
        string output = RunPluginLoaderViaDotnetRun(packPath);

        var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        int idxLoadC = Array.FindIndex(lines, l => l.Contains("Загрузка плагина: PluginC"));
        int idxExecC = Array.FindIndex(lines, l => l.Contains("PluginC executed!"));
        int idxLoadA = Array.FindIndex(lines, l => l.Contains("Загрузка плагина: PluginA"));
        int idxExecA = Array.FindIndex(lines, l => l.Contains("PluginA executed!"));
        int idxLoadB = Array.FindIndex(lines, l => l.Contains("Загрузка плагина: PluginB"));
        int idxExecB = Array.FindIndex(lines, l => l.Contains("PluginB executed! depend on PluginA and PluginC"));
        int idxDone = Array.FindIndex(lines, l => l.Contains("Все плагины выполнены."));

        Assert.True(idxLoadC >= 0);
        Assert.True(idxExecC >= 0);
        Assert.True(idxLoadA >= 0);
        Assert.True(idxExecA >= 0);
        Assert.True(idxLoadB >= 0);
        Assert.True(idxExecB >= 0);
        Assert.True(idxDone >= 0);

        Assert.True(idxLoadC < idxExecC);
        Assert.True(idxLoadA < idxExecA);
        Assert.True(idxExecA < idxLoadB);
	Assert.True(idxExecC < idxLoadB);
        Assert.True(idxLoadB < idxExecB);
        Assert.True(idxExecB < idxDone);

        Assert.DoesNotContain("Ошибка", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TestPluginPackC_ThrowsCyclicDependencyError()
    {
        string packPath = GetPluginPackPath("PluginPackC");
        string output = RunPluginLoaderViaDotnetRun(packPath);

        Assert.Contains("Обнаружена циклическая зависимость между плагинами.", output);
        Assert.DoesNotContain("Загрузка плагина:", output);
        Assert.DoesNotContain("Все плагины выполнены.", output);
    }

    [Fact]
    public void TestEmptyPluginPack_NoLibrariesFound()
    {
        string packPath = GetPluginPackPath("EmptyPluginPack");
        string output = RunPluginLoaderViaDotnetRun(packPath);

        Assert.Contains("Библиотеки не найдены.", output);
        Assert.DoesNotContain("Загрузка плагина:", output);
        Assert.DoesNotContain("Все плагины выполнены.", output);
    }
}
