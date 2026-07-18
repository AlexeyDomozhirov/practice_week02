namespace PluginC;
using CommandLib;

[PluginLoad("PluginC", "PluginA")]
public class PluginC : ICommand
{
    void ICommand.Execute()
    {
        Console.WriteLine("PluginC executed! depend on PluginA");
    }
}
