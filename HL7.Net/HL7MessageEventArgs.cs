using System;

namespace HL7.Net
{
    public class HL7MessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public string AckMessage { get; set; }
    }
}
