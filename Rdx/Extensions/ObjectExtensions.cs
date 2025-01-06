using System.Collections.Concurrent;
using System.Reflection;
using Rdx.Serialization.Attributes;
using Rdx.Serialization.Attributes.Markup;

namespace Rdx.Extensions;

public static class ObjectExtensions
{
    public static void EnsureNotNull(this object? obj, string? name = null)
    {
        if (obj is null) throw new ArgumentNullException(name ?? nameof(obj));
    }

    public static RdxSerializerAttribute? FindRdxSerializerAttribute(
        this Type type,
        ConcurrentDictionary<Type, RdxSerializerAttribute>? knownSerializers = null)
    {
        if (knownSerializers?.TryGetValue(type, out var result) ?? false) return result;

        var attribute = type.GetCustomAttribute(typeof(RdxSerializerAttribute), true);
        if (attribute is not RdxSerializerAttribute serializerAttribute) return null;

        if (!knownSerializers?.ContainsKey(type) ?? false) knownSerializers[type] = serializerAttribute;

        return serializerAttribute;
    }

    public static (string name, PropertyInfo propertyInfo)[] GetObjectProperties(
        this Type objType,
        ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]>? knownTypes = null)
    {
        if (knownTypes?.TryGetValue(objType, out var result) ?? false) return result;

        var properties = objType
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(p => (
                attribute: p.GetCustomAttribute(typeof(RdxPropertyAttribute)) as RdxPropertyAttribute,
                property: p
            ))
            .Where(p => p.attribute is not null)
            .Select(t => (t.attribute!.PropertyName ?? t.property.Name, t.property))
            .ToArray();

        if (!knownTypes?.ContainsKey(objType) ?? false) knownTypes[objType] = properties;

        return properties;
    }

    public static IEnumerable<Type> GetParentTypes(this Type type)
    {
        foreach (var i in type.GetInterfaces()) yield return i;

        var currentBaseType = type.BaseType;
        while (currentBaseType != null)
        {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
    }
}