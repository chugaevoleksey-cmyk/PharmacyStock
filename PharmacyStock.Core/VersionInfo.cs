using System.Reflection;

namespace PharmacyStock.Core;

/// <summary>Reads the semantic version defined for the assembly.</summary>
public static class VersionInfo
{
    /// <summary>Returns the informational version without the source revision.</summary>
    /// <returns>The semantic version of the Core assembly.</returns>
    public static string Current()
    {
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<
            AssemblyInformationalVersionAttribute>();
        var raw = attribute?.InformationalVersion ?? "0.0.0";
        var plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
