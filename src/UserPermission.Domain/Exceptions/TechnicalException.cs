using System;
using System.Runtime.Serialization;

namespace UserPermission.Domain.Exceptions
{
    [Serializable]
    public class TechnicalException : Exception
    {
        public TechnicalException() { }
        public TechnicalException(string message) : base(message) { }
        public TechnicalException(string message, Exception inner) : base(message, inner) { }
        protected TechnicalException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
