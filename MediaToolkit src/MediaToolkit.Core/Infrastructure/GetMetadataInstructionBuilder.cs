using System.Globalization;
using System.Text;

namespace MediaToolkit.Core.Infrastructure
{

    public class GetMetadataInstructionBuilder : IInstructionBuilder
    {
        public string? InputFilePath { get; set; }

        public string BuildInstructions()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, " \"{0}\" ", InputFilePath);
            return builder.ToString();
        }
    }
}