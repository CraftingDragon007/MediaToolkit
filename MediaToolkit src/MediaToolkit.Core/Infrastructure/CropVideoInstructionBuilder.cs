using System.Globalization;
using System.Text;

namespace MediaToolkit.Core.Infrastructure
{

    public class CropVideoInstructionBuilder : IInstructionBuilder
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? InputFilePath { get; set; }
        public string? OutputFilePath { get; set; }


        public string BuildInstructions()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, " -i \"{0}\" ", InputFilePath);
            builder.AppendFormat(CultureInfo.InvariantCulture, " -filter:v \"crop={0}:{1}:{2}:{3}\" ", Width, Height, X,
                Y);
            builder.AppendFormat(CultureInfo.InvariantCulture, " \"{0}\" ", OutputFilePath);
            return builder.ToString();
        }
    }
}