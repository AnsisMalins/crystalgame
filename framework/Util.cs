using System;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Utilities
{
	public static class Util
	{
		public static string Because(Exception ex)
		{
			if (ex == null) return " for no reason";
			StringBuilder result = new StringBuilder();
			while (ex != null)
			{
				result.Append(" because ").Append(ex.Message);
				ex = ex.InnerException;
			}
			return result.ToString();
		}

        public static void Dispose<T>(ref T obj) where T : IDisposable
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = default(T);
            }
        }

        private static bool? _IsInDesignMode;
        public static bool IsInDesignMode
        {
            get
            {
                if (!_IsInDesignMode.HasValue)
                    _IsInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(
                        DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)
                        ).Metadata.DefaultValue;
                return _IsInDesignMode.Value;
            }
        }

        public static string Str(object value)
        {
            return value != null ? value.ToString() : null;
        }

        public static string Str(TimeSpan value)
        {
            return (value.TotalHours >= 1 ? value.TotalHours.ToString("0") + ":" : "")
                + (value.TotalHours >= 1 ? value.Minutes.ToString("00") : value.Minutes.ToString())
                + ":" + value.Seconds.ToString("00");
        }

        public static bool StrEquals(object objA, object objB)
		{
            if (objA is string || objB is string) return Str(objA) == Str(objB);
            else return object.Equals(objA, objB);
		}
	}
}