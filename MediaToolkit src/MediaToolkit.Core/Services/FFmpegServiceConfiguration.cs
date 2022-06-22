namespace MediaToolkit.Core.Services;

public class FFmpegServiceConfiguration : IProcessServiceConfiguration
{
    public FFmpegServiceConfiguration(string? exePath = null,
        string? globalArguments = null,
        string? embeddedResourceId = null)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        var pathItems = path?.Split(OperatingSystem.IsWindows() ? ';' : ':');
        if (pathItems != null && exePath == null)
            foreach (var pathItem in pathItems)
                if (OperatingSystem.IsWindows())
                {
                    if (!File.Exists(pathItem + "\\ffmpeg.exe")) continue;
                    ExePath = pathItem + "\\ffmpeg.exe";
                    break;
                }
                else if (OperatingSystem.IsLinux())
                {
                    if (!File.Exists(pathItem + "/ffmpeg")) continue;
                    ExePath = pathItem + "/ffmpeg";
                    break;
                }

        ExePath ??= exePath ?? Directory.GetCurrentDirectory() + @"/MediaToolkit/ffmpeg.exe";
        GlobalArguments = globalArguments ?? @"-nostdin -progress pipe:2 -y -loglevel warning ";
        EmbeddedResourceId = embeddedResourceId ?? "MediaToolkit.Core.Resources.FFmpeg.exe.gz";
    }

    public string ExePath { get; set; }
    public string GlobalArguments { get; set; }
    public string EmbeddedResourceId { get; set; }
}