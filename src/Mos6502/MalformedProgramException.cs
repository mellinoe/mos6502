using System;

namespace Mos6502
{
    internal class MalformedProgramException : Exception
    {
        public MalformedProgramException()
        {
        }

        public MalformedProgramException(string message) : base(message)
        {
        }

        public MalformedProgramException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}