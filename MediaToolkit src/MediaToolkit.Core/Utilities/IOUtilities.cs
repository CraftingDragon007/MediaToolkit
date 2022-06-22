using System.IO.Compression;
using System.Reflection;

namespace MediaToolkit.Core.Utilities;

public class IoUtilities
{
    public async Task CopyFileAsync(string sourceFile, string destinationFile)
    {
        using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                   FileOptions.Asynchronous | FileOptions.SequentialScan))
        using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write,
                   FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
        {
            await sourceStream.CopyToAsync(destinationStream);
        }
    }

    public string ChangeFilePathName(string from, string to)
    {
        if (from.Length < 1 || from.IsNullOrWhiteSpace()) throw new ArgumentException("Path is empty", nameof(from));
        if (to.Length < 1 || to.IsNullOrWhiteSpace()) throw new ArgumentException("Path is empty", nameof(to));

        var fileName = Path.GetFileName(from);
        var fileExtension = Path.GetExtension(fileName);

        if (fileExtension.IsNullOrWhiteSpace()) return from.Replace(fileName, to);

        var index = fileName.LastIndexOf(fileExtension, StringComparison.Ordinal);
        fileName = fileName.Substring(0, index);

        return from.Replace(fileName, to);
    }

    public void DecompressResourceStream(string resourceId, string toPath)
    {
        //TODO: Make this cross-platform
        return;
        var currentAssembly = Assembly.GetExecutingAssembly();

        using var resourceStream = currentAssembly.GetManifestResourceStream(resourceId);
        if (resourceStream == null) throw new Exception("GZip stream is null");

        using var fileStream = new FileStream(toPath, FileMode.Create);
        using var compressedStream = new GZipStream(resourceStream, CompressionMode.Decompress);
        compressedStream.CopyTo(fileStream);
    }
}