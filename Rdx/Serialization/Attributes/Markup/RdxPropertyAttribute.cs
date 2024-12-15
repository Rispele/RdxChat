namespace Rdx.Serialization.Attributes.Markup;

[AttributeUsage(AttributeTargets.Property)]
public class RdxPropertyAttribute : Attribute
{
    public string? PropertyName { get; }

    public RdxPropertyAttribute(string? propertyName = null)
    {
        PropertyName = propertyName;
    }
}