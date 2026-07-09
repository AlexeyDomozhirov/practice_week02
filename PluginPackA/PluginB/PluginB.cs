namespace PluginB;
using CommandLib;

[PluginLoad("PluginB")]
public class PluginB : ICommand
{
    void ICommand.Execute()
    {
	Console.WriteLine("PluginB executed!");
    }
}
