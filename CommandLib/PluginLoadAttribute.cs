namespace CommandLib;

using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PluginLoadAttribute : Attribute
{
    public string[] Dependencies { get; }
    public string PluginName { get; }
    public PluginLoadAttribute(string pluginName, params string[] dependencies)
    {
        PluginName = pluginName;
	Dependencies = dependencies == null ? new string[0] : dependencies;
    }
}
