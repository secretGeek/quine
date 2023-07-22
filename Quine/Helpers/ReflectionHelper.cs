using CommandLine;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Quine.Helpers;

public static class ReflectionHelper
{
    public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit = true)
        where T : Attribute
        => provider.GetCustomAttributes(typeof(T), inherit).OfType<T>().ToArray();

    public static TAttribute? GetCustomAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        => value.GetType()
                .GetMember(value.ToString())?
                .FirstOrDefault()?
                .GetCustomAttributes(typeof(TAttribute), false)
                .FirstOrDefault()
                .Cast<TAttribute>() ?? null;

    /// <summary>
    /// If an Enum has a description attribute, we will find it.
    /// Otherwise, describe it as (for example) "Readonly (FileAttributes)"
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// 
    public static string GetDescription(this Enum value)
        => value.GetCustomAttribute<DescriptionAttribute>()?.Description ?? ($"{value} ({value.GetType().Name})");
}
