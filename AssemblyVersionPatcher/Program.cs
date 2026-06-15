using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: AssemblyVersionPatcher <assemblyPath> <targetAssemblyName> [targetAssemblyName ...]");
            Console.Error.WriteLine("Example: AssemblyVersionPatcher FanCurveExternsion.dll LenovoLegionToolkit.Lib UniversalFanControl.Lib");
            return 1;
        }

        try
        {
            var assemblyPath = args[0];
            var targetNames = args[1..];
            var anyChanged = false;

            foreach (var targetName in targetNames)
            {
                if (Patch(assemblyPath, targetName))
                {
                    Console.WriteLine($"  {targetName}: version set to 0.0.0.0");
                    anyChanged = true;
                }
                else
                {
                    Console.WriteLine($"  {targetName}: already at version 0.0.0.0 or not found (skipped)");
                }
            }

            if (anyChanged)
                Console.WriteLine($"Patched {assemblyPath}");
            else
                Console.WriteLine($"Skipped {assemblyPath}: all targets already at version 0.0.0.0 or not found");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    public static bool Patch(string assemblyPath, string targetAssemblyName)
    {
        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath)!);

        using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath,
            new ReaderParameters { AssemblyResolver = resolver, InMemory = true });

        var targetRef = assembly.MainModule.AssemblyReferences
            .FirstOrDefault(ar => string.Equals(ar.Name, targetAssemblyName, StringComparison.OrdinalIgnoreCase));

        if (targetRef == null || targetRef.Version.Major == 0)
            return false;

        var originalVersion = targetRef.Version;
        targetRef.Version = new Version(0, 0, 0, 0);

        assembly.Write(assemblyPath,
            new WriterParameters { WriteSymbols = false });

        Console.WriteLine($"  {targetAssemblyName}: {originalVersion} -> 0.0.0.0");
        return true;
    }
}
