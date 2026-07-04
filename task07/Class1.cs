using System.Reflection;

namespace task07;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    
    public DisplayNameAttribute(string name)
    {
        DisplayName = name;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[VersionAttribute(1, 0), DisplayNameAttribute("Пример класса")]
public class SampleClass
{
    [DisplayNameAttribute("Тестовый метод")]
    public void TestMethod() { }

    [DisplayNameAttribute("Числовое свойство")]
    public int Number { get; }
}

static class ReflectionHelper
{
    static void PrintTypeInfo(Type type)
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

