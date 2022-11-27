namespace MediaToolkit
{

    public class Resources
    {
        public static string ExceptionsFFmpegProcessNotRunning => "FFmpeg process is not running.";
        public static string ExceptionMediaInputFileNotFound => "Input file not found";
        public static string ExceptionsNullFFmpegGzipStream => "FFMpeg GZip stream is null";
        public static string? FFmpegProcessName { get; set; }
        public static string? FFmpegManifestResourceName { get; set; }
    }
}