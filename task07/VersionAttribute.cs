namespace task07;

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    
    public VersionAttribute(int Major, int Minor)
    {
        this.Major = Major;
        this.Minor = Minor;
    }
}

