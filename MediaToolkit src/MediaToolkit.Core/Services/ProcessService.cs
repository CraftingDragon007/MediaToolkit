using System.Diagnostics;
using MediaToolkit.Core.Events;
using MediaToolkit.Core.Infrastructure;
using MediaToolkit.Core.Utilities;
using Microsoft.Extensions.Logging;

namespace MediaToolkit.Core.Services;

public abstract class ProcessService : IProcessService
{
    private readonly IProcessServiceConfiguration _configuration;
    protected readonly ILogger? Logger;
    private readonly IoUtilities _utilities;

    protected ProcessService(IProcessServiceConfiguration configuration, ILogger? logger = null)
    {
        this._configuration = configuration;
        _utilities = new IoUtilities();
        Logger = logger;

        if (!this._configuration.EmbeddedResourceId.IsNullOrWhiteSpace())
        {
            // this.utilities.DecompressResourceStream(this.configuration.EmbeddedResourceId, this.configuration.ExePath);
        }
    }

    public abstract event EventHandler<RawDataReceivedEventArgs> OnRawDataReceivedEventHandler;

    public abstract Task ExecuteInstructionAsync(string instruction, CancellationToken token = default);
    public abstract Task ExecuteInstructionAsync(IInstructionBuilder instruction, CancellationToken token = default);

    #region Helpers

    protected ProcessStartInfo GetProcessStartInfo(string instruction)
    {
        var arguments = $"{_configuration.GlobalArguments} {instruction}";

        return new ProcessStartInfo
        {
            Arguments = arguments,
            FileName = _configuration.ExePath,
            CreateNoWindow = true,
            RedirectStandardInput = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };
    }

    /// <summary>
    ///     Creates a temporary copy of the executable to enable the client to
    ///     process jobs in parallel.
    /// </summary>
    /// <returns>Path of temporary copy</returns>
    protected async Task<string> GetTempExe()
    {
        var exeCopyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        await _utilities.CopyFileAsync(_configuration.ExePath, exeCopyPath);

        return exeCopyPath;
    }

    #endregion
}