using System;
using System.Windows;

namespace Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultViewAttribute : Attribute
    {
        public DefaultViewAttribute(Type viewType)
        {
            Guard.ArgumentNotNull(viewType, "viewType");
            Guard.ArgumentSubclassOf(viewType, "viewType", typeof(FrameworkElement));
            ViewType = viewType;
        }

        public Type ViewType { get; private set; }
    }
}