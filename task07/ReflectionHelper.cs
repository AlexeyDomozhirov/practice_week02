namespace task07;

using System;
using System.Linq;
using System.Reflection;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        if (type == null) return;

        Console.WriteLine($"Тип: {type.FullName}");

        var displayName = Attribute.GetCustomAttribute(type, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
        if (displayName != null)
            Console.WriteLine($"Отображаемое имя: {displayName.DisplayName}");

        var version = Attribute.GetCustomAttribute(type, typeof(VersionAttribute)) as VersionAttribute;
        if (version != null)
            Console.WriteLine($"Версия: {version.Major}.{version.Minor}");

        var otherAttributes = Attribute.GetCustomAttributes(type)
            .Where(a => a.GetType() != typeof(DisplayNameAttribute) 
                     && a.GetType() != typeof(VersionAttribute))
            .ToList();
        if (otherAttributes.Any())
        {
            Console.WriteLine("Атрибуты класса:");
            foreach (var attr in otherAttributes)
                Console.WriteLine($"  {attr.GetType().FullName}");
        }

        Console.WriteLine("\n--- Методы ---");
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic 
                                    | BindingFlags.Instance | BindingFlags.Static);
        foreach (var method in methods)
        {
            if (method.IsSpecialName) continue;

            var methodDisplay = Attribute.GetCustomAttribute(method, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            Console.Write($"- {method.Name}");
            if (methodDisplay != null)
                Console.Write($" ({methodDisplay.DisplayName})");
            Console.WriteLine();

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                Console.WriteLine("  Параметры:");
                foreach (var p in parameters)
                    Console.WriteLine($"    {p.ParameterType.Name} {p.Name}");
            }
            else
            {
                Console.WriteLine("  (без параметров)");
            }
        }

        Console.WriteLine("\n--- Конструкторы ---");
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic 
                                              | BindingFlags.Instance | BindingFlags.Static);
        foreach (var ctor in constructors)
        {
            Console.Write($"- {ctor.Name}");
            var parameters = ctor.GetParameters();
            if (parameters.Length > 0)
            {
                Console.WriteLine();
                Console.WriteLine("  Параметры:");
                foreach (var p in parameters)
                    Console.WriteLine($"    {p.ParameterType.Name} {p.Name}");
            }
            else
            {
                Console.WriteLine(" (без параметров)");
            }
        }
    }
}
