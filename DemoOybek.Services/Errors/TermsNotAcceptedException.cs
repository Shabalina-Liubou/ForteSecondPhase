using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DemoOybek.Services.Errors
{
    [Serializable]
    public class TermsNotAcceptedException : Exception
    {
        public TermsNotAcceptedException() { }
        public TermsNotAcceptedException(string message) : base(message) { }
        public TermsNotAcceptedException(string message, Exception innerException) : base(message, innerException) { }
        protected TermsNotAcceptedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
