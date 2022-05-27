using CommandLine;
using NetCleaner;
using NetCleaner.Services;

#if DEBUG

// Test non-existence
// args = new[] { "-r", @"D:\Temp\something-not-exists" };

// Test for WhatIf
args = new[] { "-r", @"D:\Temp\TestCleaner", "-s" };

//args = new[] { "-r", @"D:\Temp\TestCleaner", "-s", "-d" };

#endif

CommandLineOptions? o = null;
Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsed(parsed => o = parsed);

if (o is null) { return; }

new CleanerService().Clean(o);