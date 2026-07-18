namespace PluginC;
using CommandLib;

[PluginLoad("PluginC")]
public class PluginC : ICommand
{
    void ICommand.Execute()
    {
        Console.WriteLine("PluginC executed!");
    }
}
