using System;

namespace Utilities
{
    public class AlreadyInitializedException : InvalidOperationException
    {
        public AlreadyInitializedException(string objectName) :
            base(objectName + " is already initialized")
        {
        }
    }

    public class NotInitializedException : InvalidOperationException
    {
        public NotInitializedException(string objectName) :
            base(objectName + " is not initialized")
        {
        }
    }
}