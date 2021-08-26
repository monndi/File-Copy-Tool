using System;

namespace File_Copy_Tool
{
    public class CopyingCanceledException : Exception
    {
        private string _message;
        public CopyingCanceledException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}
