namespace NetCleaner.Services;

internal class CleanerService
{
    public List<DeletingItem> DeletingItems { get; set; } = new();
    public long TotalSize = 0;

    public void Clean(CommandLineOptions o)
    {
        try
        {
            if (!o.Delete)
            {
                WriteWhatIfNote();
            }

            var root = o.Root ?? Environment.CurrentDirectory;
            if (!Directory.Exists(root))
            {
                throw new IOException("Root folder does not exist: " + o.Root);
            }

            this.TotalSize = 0;
            this.DeletingItems = new();
            this.ScanFolder(root, o);

            foreach (var item in this.DeletingItems)
            {
                this.ShouldDelete(item, o.Delete);
            }

            if (o.ReportSize)
            {
                Console.WriteLine();
                Console.WriteLine("Total deleting items size: " +
                    FormatSize(this.TotalSize));
            }

            if (!o.Delete)
            {
                WriteWhatIfNote();
            }
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
        }
    }

    void ScanFolder(string folder, CommandLineOptions o)
    {
        try
        {
            var folderName = Path.GetFileName(folder);
            if (o.IgnoredNames.Contains(folderName))
            {
                WriteWarning("Skipping ignored folder: " + folder);
                return;
            }

            var files = Directory.GetFiles(folder);
            var containsExt = o.RequiredExts.Count == 0;

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (!string.IsNullOrEmpty(ext) &&
                    o.RequiredExts.Contains(ext[1..]))
                {
                    containsExt = true;
                    break;
                }
            }

            if (containsExt)
            {
                foreach (var file in files)
                {
                    var name = Path.GetFileName(file);
                    if (o.FileNames.Contains(name))
                    {
                        this.DeletingItems.Add(
                            GetDeletingItem(file, false, o.ReportSize));
                    }
                }
            }

            var subFolders = Directory.GetDirectories(folder);
            foreach (var subFolder in subFolders)
            {
                var name = Path.GetFileName(subFolder);

                if (o.FileNames.Contains(name))
                {
                    // Don't merge this if
                    if (containsExt)
                    {
                        this.DeletingItems.Add(GetDeletingItem(
                        subFolder, true, o.ReportSize));
                    }
                }
                else
                {
                    this.ScanFolder(subFolder, o);
                }
            }
        }
        catch (Exception ex)
        {
            WriteWarning($"Failed to scan folder {folder}: {ex.Message}");
        }
    }

    void ShouldDelete(DeletingItem item, bool delete)
    {
        this.TotalSize += item.Size;

        var reportText = item.Path + " " +
            (item.Size > 0 ? $"({FormatSize(item.Size)})" : "");
        if (delete)
        {
            if (item.IsFolder)
            {
                Directory.Delete(item.Path, true);
            }
            else
            {
                File.Delete(item.Path);
            }

            reportText = "Deleted: " + reportText;
        }
        else
        {
            reportText = "Item would be deleted: " + reportText;
        }

        Console.WriteLine(reportText);
    }

    static DeletingItem GetDeletingItem(string path, bool isFolder, bool reportSize)
    {
        var size = 0L;

        if (reportSize)
        {
            if (isFolder)
            {
                var dirInfo = new DirectoryInfo(path);
                size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);
            }
            else
            {
                size = new FileInfo(path).Length;
            }
        }

        return new(path, isFolder, size);
    }

    static void WriteWhatIfNote()
    {
        WriteWarning("Running in WhatIf mode. To actually delete these files and folders, run with -d switch.");
    }

    static void WriteError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static void WriteWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static readonly string[] Units = { "B", "KB", "MB", "GB", "TB" };
    static string FormatSize(long bytes)
    {
        float value = bytes;
        var unit = 0;
        while (value > 1024 && unit < Units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return value.ToString("0.00") + Units[unit];
    }

}

record DeletingItem(string Path, bool IsFolder, long Size = 0);