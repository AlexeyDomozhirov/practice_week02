namespace PluginB;
using CommandLib;

[PluginLoad("PluginB", "PluginA", "PluginC")]
public class PluginB : ICommand
{
    void ICommand.Execute()
    {
        Console.WriteLine("PluginB executed! depend on PluginA and PluginC");
    }
}
