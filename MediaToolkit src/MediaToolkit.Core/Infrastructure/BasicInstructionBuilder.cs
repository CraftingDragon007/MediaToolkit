using System.Globalization;
using System.Text;

namespace MediaToolkit.Core.Infrastructure;

public class BasicInstructionBuilder : IInstructionBuilder
{
    public string InputFilePath { get; set; } = null!;
    public string OutputFilePath { get; set; } = null!;

    public string BuildInstructions()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(CultureInfo.InvariantCulture, " -i \"{0}\" ", InputFilePath);
        builder.AppendFormat(CultureInfo.InvariantCulture, " \"{0}\" ", OutputFilePath);
        return builder.ToString();
    }
}