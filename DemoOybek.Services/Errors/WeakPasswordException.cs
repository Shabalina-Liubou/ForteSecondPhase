using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DemoOybek.Services.Errors
{
    [Serializable]
    public class WeakPasswordException : Exception
    {
        public WeakPasswordException() { }
        public WeakPasswordException(string message) : base(message) { }
        public WeakPasswordException(string message, Exception innerException) : base(message, innerException) { }
        protected WeakPasswordException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
