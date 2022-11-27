using System;

namespace MediaToolkit.Core.Events
{

    public class RawDataReceivedEventArgs : EventArgs
    {
        public RawDataReceivedEventArgs(string data)
        {
            Data = data;
        }

        public string Data { get; }
    }
}