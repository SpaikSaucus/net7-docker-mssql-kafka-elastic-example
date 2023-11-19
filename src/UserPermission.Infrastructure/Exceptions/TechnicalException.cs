namespace UserPermission.Infrastructure.Exceptions
{
    using System;

    [Serializable]
    public class TechnicalException : Exception
    {
        public TechnicalException()
        {
        }

        public TechnicalException(string message) : base(message)
        {
        }

        public TechnicalException(string message, Exception innerException) : base(message, innerException)
        {
        }     
    }
}
