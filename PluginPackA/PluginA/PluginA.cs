namespace PluginA;
using CommandLib;

[PluginLoad("PluginA", "PluginB")]
public class PluginA : ICommand
{
    void ICommand.Execute()
    {
	Console.WriteLine("PluginA executed! depend on PluginB");
    }
}
