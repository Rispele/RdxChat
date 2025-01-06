namespace Rdx.Serialization.Attributes.Markup;

[AttributeUsage(AttributeTargets.Property)]
public class RdxPropertyAttribute : Attribute
{
    public RdxPropertyAttribute(string? propertyName = null)
    {
        PropertyName = propertyName;
    }

    public string? PropertyName { get; }
}