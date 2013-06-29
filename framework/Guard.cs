using System;

namespace Utilities
{
    public static class Guard
    {
        public static void ArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
        }

        public static void ArgumentSubclassOf(object argument, string argumentName, Type baseType)
        {
            ArgumentSubclassOf(argument.GetType(), argumentName, baseType);
        }

        public static void ArgumentSubclassOf(Type argumentType, string argumentName, Type baseType)
        {
            if (!argumentType.IsSubclassOf(baseType)) throw new ArgumentException(
                argumentName + " is " + argumentType.Name + ", but " + baseType.Name + " expected",
                argumentName);
        }
    }
}