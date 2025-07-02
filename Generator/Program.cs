using Generator.Core;
using Generator.Core.Generations;

class Program
{
    static void Main(string[] args)
    {
        string? versionStr = GetArg(args, "version");
        int version = int.TryParse(versionStr, out var v) ? v : 9;

        IGenerator generator = version switch
        {
            9 => new PK9Generator(args),
            _ => throw new NotSupportedException($"Unsupported version: {version}")
        };

        generator.Run();
        generator.Export();
    }

    static string? GetArg(string[] args, string key)
    {
        string prefix = $"--{key}=";
        string? match = args.FirstOrDefault(arg => arg.StartsWith(prefix));
        if (match == null)
            return null;

        string[] parts = match.Split('=', 2);
        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            return null;

        return parts[1];
    }
}
