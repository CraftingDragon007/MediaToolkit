namespace MediaToolkit;

public class Resources
{
    public static string Exceptions_FFmpeg_Process_Not_Running => "FFmpeg process is not running.";
    public static string Exception_Media_Input_File_Not_Found => "Input file not found";
    public static string Exceptions_Null_FFmpeg_Gzip_Stream => "FFMpeg GZip stream is null";
    public static string? FFmpegProcessName { get; set; }
    public static string? FFmpegManifestResourceName { get; set; }
}