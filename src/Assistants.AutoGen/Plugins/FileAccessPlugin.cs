// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Diagnostics;

namespace SemanticKernel.Assistants.AutoGen.Plugins;

public class FileAccessPlugin
{
    private readonly ILogger _logger;
    private readonly string _rootDirectoryPath;

    public FileAccessPlugin(string rootDirectoryPath, ILoggerFactory loggerFactory)
    {
        this._rootDirectoryPath = rootDirectoryPath;
        this._logger = loggerFactory.CreateLogger<FileAccessPlugin>();
    }

    [KernelFunction]
    [Description("Opens the specified file with the Windows Explorer.")]
    public void OpenFile([Description("The file path to open in the Windows Explorer")] string filePath)
    {
        var path = Path.Combine(this._rootDirectoryPath, filePath);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Cannot find {path}");
        }

        try
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = $"\"{Path.Combine(this._rootDirectoryPath, filePath)}\"";
            fileopener.Start();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unable to open the specified file.");
        }
    }

    [KernelFunction]
    [Description("Given a relative path, returns the file full path. Always use the relative path specified by the user, this function will return the full path.")]
    public string GetFullFilePath([Description("The relative file path")] string filePath)
    {
        var path = Path.GetFullPath(filePath);

        if (File.Exists(path))
        {
            return path;
        }

        throw new FileNotFoundException($"Cannot find file '{filePath}' in current directory.");
    }
}
