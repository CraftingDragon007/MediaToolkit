using MediaToolkit.Core.Events;
using MediaToolkit.Core.Infrastructure;

namespace MediaToolkit.Core.Services;

public interface IProcessService
{
    event EventHandler<RawDataReceivedEventArgs> OnRawDataReceivedEventHandler;
    Task ExecuteInstructionAsync(string instruction, CancellationToken token = default);
    Task ExecuteInstructionAsync(IInstructionBuilder instruction, CancellationToken token = default);
}