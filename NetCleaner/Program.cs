using CommandLine;
using NetCleaner;
using NetCleaner.Services;

#if DEBUG

// Test non-existence
// args = new[] { "-r", @"D:\Temp\something-not-exists" };

// Test for WhatIf
// args = new[] { "-r", @"D:\Temp\TestCleaner", "-s" };

// Actually delete files, be careful
// args = new[] { "-r", @"D:\Temp\TestCleaner", "-s", "-d" };

// Show help
args = new[] { "-h" };

#endif

CommandLineOptions? o = null;
Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsed(parsed => o = parsed);

if (o is null) { return; }

new CleanerService().Clean(o);