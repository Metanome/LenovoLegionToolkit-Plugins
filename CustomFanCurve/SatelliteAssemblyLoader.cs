using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve;

internal static class SatelliteAssemblyLoader
{
    private static readonly string _prefix = $"{typeof(SatelliteAssemblyLoader).Namespace}.satellites.";

    [ModuleInitializer]
    public static void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
    }

    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var an = new AssemblyName(args.Name);
        if (an.CultureInfo is null || an.CultureInfo.Equals(CultureInfo.InvariantCulture))
            return null;

        var asm = Assembly.GetExecutingAssembly();
        var resourceNames = asm.GetManifestResourceNames();

        var culture = an.CultureInfo;
        while (culture != null && culture != CultureInfo.InvariantCulture)
        {
            var suffix = $".{culture.Name}.{an.Name}.dll";
            var match = resourceNames.FirstOrDefault(n =>
                n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                using var stream = asm.GetManifestResourceStream(match);
                if (stream == null) break;

                var bytes = new byte[stream.Length];
                stream.ReadExactly(bytes);
                return Assembly.Load(bytes);
            }

            culture = culture.Parent;
        }

        return null;
    }
}
