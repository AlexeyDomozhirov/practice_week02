namespace task11;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Reflection;

public static class ClassGenerator
{
    public static ICalculator CreateCalculator()
    {
       var source = @"
       namespace task11;
       public class Calculator : ICalculator
       {
           public int Add(int a, int b) => a + b;
           public int Minus(int a, int b) => a - b;
           public int Mul(int a, int b) => a * b;
           public int Div(int a, int b) => a / b;
       }";
       
        var tree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
        };

        var compilation = CSharpCompilation.Create(
            "DynamicCalc",
            new[] { tree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
            var errors = string.Join("\n", emitResult.Diagnostics);
            throw new InvalidOperationException($"Compilation failed:\n{errors}");
        }

        ms.Position = 0;
        Assembly assembly;
        try
        {
            assembly = Assembly.Load(ms.ToArray());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load compiled assembly.", ex);
        }

        var calculatorType = assembly.GetType("task11.Calculator");
        if (calculatorType == null)
            throw new InvalidOperationException("Type 'task11.Calculator' not found in the compiled assembly.");

        if (!typeof(ICalculator).IsAssignableFrom(calculatorType))
            throw new InvalidOperationException("The generated type does not implement ICalculator.");

        object instance;
        try
        {
            instance = Activator.CreateInstance(calculatorType)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create an instance of Calculator.", ex);
        }

        if (instance == null)
            throw new InvalidOperationException("Activator.CreateInstance returned null.");

        return (ICalculator)instance;
    }
}
