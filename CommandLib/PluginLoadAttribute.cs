namespace CommandLib;

using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PluginLoadAttribute : Attribute
{
    public string[] Dependencies { get; }
    public string PluginName { get; }
    public PluginLoadAttribute(string PluginName, params string[] Dependencies)
    {
        this.PluginName = PluginName;
	this.Dependencies = Dependencies == null ? new string[0] : Dependencies;
    }
}
