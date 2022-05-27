using CommandLine;

namespace NetCleaner;

internal class CommandLineOptions
{

    [Option('d', "Delete", HelpText = "Actually delete the folders. Default: false.")]
    public bool Delete { get; set; } = false;

    [Option('n', "Names", HelpText = "Folder and File names in comma-separated list that should be deleted. NOT case-sensitive. Default: bin,obj")]
    public string FileNamesText
    {
        set => this.FileNames = ParseList(value);
    }

    [Option('e', "Extensions", HelpText = "File extensions that is required to exist in order to delete folders. NOT case-sensitive. If none is specified, files and folders will be deleted. Default: sln,csproj,wapproj")]
    public string RequiredExtsText
    {
        set => this.RequiredExts = ParseList(value);
    }

    [Option('s', "Size", HelpText = "Report total file size of deleted folders")]
    public bool ReportSize { get; set; } = false;

    [Option('r', "Root", HelpText = "Start folder where the search is performed. Default to Current Directory.")]
    public string? Root { get; set; }

    [Option('i', "Ignores", HelpText = "Ignore these folder names in this comma-separated list. NOT case-sensitive. Default: (nothing)")]
    public string IgnoredNamesText
    {
        set => this.IgnoredNames = ParseList(value);
    }

    static HashSet<string> ParseList(string input) =>
        new(input.Split(',', StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase);

    internal HashSet<string> FileNames { get; set; } = new() { "bin", "obj" };
    internal HashSet<string> RequiredExts { get; set; } = new() { "sln", "csproj", "wapproj" };
    internal HashSet<string> IgnoredNames { get; set; } = new();

}
