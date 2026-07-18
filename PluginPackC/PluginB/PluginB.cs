namespace PluginB;
using CommandLib;

[PluginLoad("PluginB", "PluginC")]
public class PluginB : ICommand
{
    void ICommand.Execute()
    {
        Console.WriteLine("PluginB executed! depend on PluginC");
    }
}
