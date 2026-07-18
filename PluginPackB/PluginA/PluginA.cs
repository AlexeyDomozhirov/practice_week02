namespace PluginA;
using CommandLib;

[PluginLoad("PluginA")]
public class PluginA : ICommand
{
    void ICommand.Execute()
    {
        Console.WriteLine("PluginA executed!");
    }
}
