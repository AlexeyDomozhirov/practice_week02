using System.Reflection;
using CommandLib;

public static class PluginLoader
{
    public static void Main(string[] args)
    {
        try
        {
            string folder = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Путь к библиотекам: {folder}");
            LoadAndExecutePlugins(folder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
        }
    }

    public static void LoadAndExecutePlugins(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Папка '{folderPath}' не найдена.");

        var dllFiles = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);
        if (!dllFiles.Any())
        {
            Console.WriteLine("Библиотеки не найдены.");
            return;
        }

        var assemblies = new List<Assembly>();
        foreach (var file in dllFiles)
        {
            try
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine($"Пропущен файл (не .NET сборка): {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        if (!assemblies.Any())
        {
            Console.WriteLine("Не удалось загрузить ни одной сборки.");
            return;
        }

        var plugins = new List<(string Name, Type Type)>();
        foreach (var asm in assemblies)
        {
            foreach (var type in asm.GetExportedTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                    if (attr == null)
                        continue;

                    if (type.GetConstructor(Type.EmptyTypes) == null)
                    {
                        Console.WriteLine($"Пропущен {type.FullName}: отсутствует конструктор без параметров.");
                        continue;
                    }

                    if (!typeof(ICommand).IsAssignableFrom(type))
                    {
                        Console.WriteLine($"Пропущен {type.FullName}: не реализует ICommand.");
                        continue;
                    }

                    string pluginName = attr.PluginName;
                    plugins.Add((pluginName, type));
                }
            }
        }

        if (!plugins.Any())
        {
            Console.WriteLine("Плагины не найдены.");
            return;
        }

        var duplicateNames = plugins
            .GroupBy(p => p.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Any())
            throw new InvalidOperationException(
                $"Обнаружены дублирующиеся имена плагинов: {string.Join(", ", duplicateNames)}");

        var pluginByName = plugins.ToDictionary(p => p.Name, p => p.Type);

        var graph = new Dictionary<string, HashSet<string>>();
        foreach (var (name, type) in plugins)
        {
            var attr = type.GetCustomAttribute<PluginLoadAttribute>();
            var deps = new HashSet<string>();
            if (attr?.Dependencies != null)
            {
                foreach (var depName in attr.Dependencies)
                {
                    if (!pluginByName.ContainsKey(depName))
                        throw new InvalidOperationException(
                            $"Плагин '{name}' зависит от '{depName}', который не является зарегистрированным плагином.");
                    deps.Add(depName);
                }
            }
            graph[name] = deps;
        }

        var sortedNames = TopologicalSort(graph);

        foreach (var name in sortedNames)
        {
            var type = pluginByName[name];
            Console.WriteLine($"Загрузка плагина: {name} ({type.FullName})");
            var plugin = (ICommand)Activator.CreateInstance(type)!;
            if (plugin == null)
                throw new InvalidOperationException("Ошибка создания экземпляра плагина.");
            plugin.Execute();
        }

        Console.WriteLine("Все плагины выполнены.");
    }

    private static List<string> TopologicalSort(Dictionary<string, HashSet<string>> graph)
    {
        var sorted = new List<string>();
        var degree = new Dictionary<string, int>();
    
        foreach (var node in graph.Keys)
            degree[node] = graph[node].Count;
    
        var queue = new Queue<string>(degree.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));
    
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            sorted.Add(node);
    
            foreach (var dependent in graph.Keys.Where(k => graph[k].Contains(node)))
            {
                degree[dependent]--;
                if (degree[dependent] == 0)
                    queue.Enqueue(dependent);
            }
        }
    
        if (sorted.Count != graph.Count)
            throw new InvalidOperationException("Обнаружена циклическая зависимость между плагинами.");
    
        return sorted;
    }
}
