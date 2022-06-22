namespace MediaToolkit.Core.Infrastructure;

public class CustomInstructionBuilder : IInstructionBuilder
{
    public string Instruction { get; set; } = null!;

    public string BuildInstructions()
    {
        return Instruction;
    }
}