using System.Reflection;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
	if(type == null) return;

        if (Attribute.GetCustomAttribute(type, typeof(DisplayNameAttribute)) is DisplayNameAttribute classDisplay)
        {
            Console.WriteLine($"Отображаемое имя: {classDisplay.DisplayName}");
        }

        if (Attribute.GetCustomAttribute(type, typeof(VersionAttribute)) is VersionAttribute versionAttr)
        {
            Console.WriteLine($"Версия: {versionAttr.Major}.{versionAttr.Minor}");
        }

        Console.WriteLine("--- Свойства ---");
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var prop in properties)
        {
            string propDisplay = Attribute.GetCustomAttribute(prop, typeof(DisplayNameAttribute)) is DisplayNameAttribute pDisplay 
                ? $" ({pDisplay.DisplayName})" 
                : string.Empty;
            
            Console.WriteLine($"- {prop.Name} {propDisplay}");
        }

        Console.WriteLine("\n--- Методы ---");
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var method in methods)
        {
            if (method.IsSpecialName) continue;

            string methodDisplay = Attribute.GetCustomAttribute(method, typeof(DisplayNameAttribute)) is DisplayNameAttribute mDisplay 
                ? $" ({mDisplay.DisplayName})" 
                : string.Empty;

            Console.WriteLine($"- {method.Name} {methodDisplay}");
        }
    }
}

