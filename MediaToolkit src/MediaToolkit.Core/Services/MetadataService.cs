using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MediaToolkit.Core.Events;
using MediaToolkit.Core.Infrastructure;
using MediaToolkit.Core.Meta;
using MediaToolkit.Core.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MediaToolkit.Core.Services
{

    public class FFprobeServiceConfiguration : IProcessServiceConfiguration
    {
        public FFprobeServiceConfiguration(string? exePath = null,
            string? globalArguments = null,
            string? embeddedResourceId = null)
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            var pathItems = path?.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':');
            if (pathItems != null && exePath == null)
                foreach (var pathItem in pathItems)
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (File.Exists(pathItem + "\\ffprobe.exe"))
                        {
                            ExePath = pathItem + "\\ffprobe.exe";
                            break;
                        }
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        if (File.Exists(pathItem + "/ffprobe"))
                        {
                            ExePath = pathItem + "/ffprobe";
                            break;
                        }
                    }

            ExePath ??= exePath ?? Directory.GetCurrentDirectory() + @"/MediaToolkit/ffprobe.exe";
            GlobalArguments = globalArguments ?? @" -of json -show_format -show_streams ";
            EmbeddedResourceId = embeddedResourceId ?? "MediaToolkit.Core.Resources.FFprobe.exe.gz";
        }

        public string ExePath { get; set; }
        public string GlobalArguments { get; set; }
        public string EmbeddedResourceId { get; set; }
    }

    public class MetadataService : ProcessService
    {
        public MetadataService(IProcessServiceConfiguration? configuration = null, ILogger? logger = null)
            : base(configuration ?? new FFprobeServiceConfiguration(), logger)
        {
        }

        public MetadataService(ILogger? logger = null) : base(new FFprobeServiceConfiguration(), logger)
        {
        }

        public MetadataService() : this(null)
        {
        }
        //private readonly string resourceId = "MediaToolkit.Core.Resources.FFprobe.exe.gz";
        //private readonly string processPath = Directory.GetCurrentDirectory() + @"/MediaToolkit/ffprobe.exe";


        //exePath: Directory.GetCurrentDirectory() + @"/MediaToolkit/ffprobe.exe",
        //globalArguments: "-nostdin -progress pipe:2 -y -loglevel warning "

        public event EventHandler<MetadataEventArgs>? OnMetadataProcessedEventHandler;

        public override event EventHandler<RawDataReceivedEventArgs>? OnRawDataReceivedEventHandler;

        [SuppressMessage("ReSharper", "PositionalPropertyUsedProblem")]
        public override async Task ExecuteInstructionAsync(string instruction, CancellationToken token = default)
        {
            if (instruction == null) throw new ArgumentNullException(nameof(instruction));
            if (instruction.IsNullOrWhiteSpace())
                throw new ArgumentException("Instructions are empty", nameof(instruction));

            // We're creating a temporary copy of the the process to enable the client the option of processing 
            // multiple files concurrently, each thread owning their own exe process.
            // The copy is deleted once processing has completed or the application has faulted.
            var exeTempPath = await GetTempExe();


            var startInfo = GetProcessStartInfo(instruction);
            using (var process = new Process {StartInfo = startInfo})
            {
                var started = process.Start();

                Logger?.LogInformation("{1} process started? {0}", started, exeTempPath);

                var json = new List<string>();

                void DataReceived(object sender, DataReceivedEventArgs received)
                {
                    if (received.Data!.IsNullOrWhiteSpace()) return;


                    try
                    {
                        OnRawDataReceivedEventHandler?.Invoke(this, new RawDataReceivedEventArgs(received.Data!));
                        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                        Logger?.LogTrace(received.Data);
                        json.Add(received.Data!);
                        if (!token.IsCancellationRequested) return;

                        Logger?.LogInformation("Token has been cancelled, killing process");

                        try
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            process.Kill();
                        }
                        catch (InvalidOperationException)
                        {
                            // swallow exceptions that are thrown when killing the process, 
                            // one possible candidate is the application ending naturally before we get a chance to kill it
                        }
                    }
                    catch (Exception ex)
                    {
                        // catch the exception and kill the process since we're in a faulted state
                        //caughtException = ex;

                        try
                        {
                            Logger?.LogError(ex, "FFmpeg faulted, killing process");

                            // ReSharper disable once AccessToDisposedClosure
                            process.Kill();
                        }
                        catch (InvalidOperationException)
                        {
                            // swallow exceptions that are thrown when killing the process, 
                            // one possible candidate is the application ending naturally before we get a chance to kill it
                        }
                    }
                }

                process.OutputDataReceived += DataReceived;
                Logger?.LogInformation("Begin reading stderr from console");

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();

                var jsonData = string.Join("\n", json);
                var metadataDeserialized = JsonConvert.DeserializeObject<Metadata>(jsonData);
                metadataDeserialized!.RawMetaData = jsonData;

                OnMetadataProcessedEventHandler?.Invoke(this, new MetadataEventArgs(metadataDeserialized));
            }

            File.Delete(exeTempPath);
        }

        public override Task ExecuteInstructionAsync(IInstructionBuilder instruction, CancellationToken token = default)
        {
            return ExecuteInstructionAsync(instruction.BuildInstructions(), token);
        }
    }
}