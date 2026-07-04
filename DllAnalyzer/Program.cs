using System.Reflection;
using task07;

if (args.Length == 0)
{
    Console.WriteLine("Укажите путь к сборке в качестве аргумента командной строки.");
    return;
}

string path = args[0];
try
{
    Assembly assembly = Assembly.LoadFrom(path);
    Type[] types = assembly.GetTypes();

    foreach (var type in types)
    {
        if (type.IsClass && !type.IsAbstract)
        {
            ReflectionHelper.PrintTypeInfo(type);
            Console.WriteLine(new string('-', 50));

        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при загрузке сборки: {ex.Message}");
}
