namespace CalcConstructor;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Reflection;

using ICalculator;

public static class DynamicCalculator
{
    public static ICalc Create()
    {
       var source = @"
       using ICalculator;
       public class Calculator : ICalc
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
           MetadataReference.CreateFromFile(typeof(ICalc).Assembly.Location)
       };
       
       var compilation = CSharpCompilation.Create(
           "DynamicCalc",
           new[] { tree },
           references,
           new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
       
       using (var ms = new MemoryStream())
       {
           var emitResult = compilation.Emit(ms);
       
           if (!emitResult.Success)
               throw new Exception("Compilation error:\n" + 
                   string.Join("\n", emitResult.Diagnostics));
       
           ms.Position = 0;
           var assembly = Assembly.Load(ms.ToArray());
           
           var calculatorType = assembly.GetType("Calculator");
           return (ICalc)Activator.CreateInstance(calculatorType!);
       }
    }
}
