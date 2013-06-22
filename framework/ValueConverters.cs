using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utilities
{
    public static class ValueConverters
    {
        public static readonly BooleanToVisibilityConverter BooleanToVisibility
            = new BooleanToVisibilityConverter();
        public new static readonly EqualsConverter Equals = new EqualsConverter();
    }

    public class ConverterGroup : Collection<IValueConverter>, IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = 0; i < Count; i++)
                value = this[i].Convert(value, targetType, parameter, culture);
            return value;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = Count - 1; i >= 0; i--)
                value = this[i].ConvertBack(value, targetType, parameter, culture);
            return value;
        }
    }

    public class EmptyStringToNullConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(Util.Str(value)) ? null : value;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(Util.Str(value)) ? "" : value;
        }
    }

    public class EqualsConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Util.StrEquals(value, parameter);
        }

        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return DependencyProperty.UnsetValue;
            for (int i = 1; i < values.Length; ++i)
                if (!Util.StrEquals(values[0], values[i])) return false;
            return true;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class FormatStringConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return DependencyProperty.UnsetValue;
            try { return string.Format(parameter.ToString(), value); }
            catch (FormatException) { return DependencyProperty.UnsetValue; }
        }

        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return DependencyProperty.UnsetValue;
            try { return string.Format(parameter.ToString(), values); }
            catch (FormatException) { return DependencyProperty.UnsetValue; }
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class MapConverter : Collection<Mapping>, IValueConverter
    {
        public MapConverter()
        {
            DefaultFrom = DependencyProperty.UnsetValue;
            DefaultTo = DependencyProperty.UnsetValue;
        }

        public object DefaultFrom { get; set; }

        public object DefaultTo { get; set; }

        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (Mapping i in this)
                if (Util.StrEquals(value, i.From)) return i.To;
            return DefaultTo;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (Mapping i in this)
                if ((i.From is string || i.To is string)
                    && Util.Str(i.From) == Util.Str(i.To)
                    || object.Equals(i.From, i.To)) return i.From;
            return DefaultFrom;
        }
    }

    public class Mapping
    {
        public Mapping()
        {
            From = DependencyProperty.UnsetValue;
            To = DependencyProperty.UnsetValue;
        }

        public object From { get; set; }

        public object To { get; set; }
    }

    public class MinConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = double.MaxValue;
            bool emptySet = true;
            foreach (object i in values)
            {
                double val;
                if (i is double || i is int) val = (double)i;
                else if (i != null || !double.TryParse(i.ToString(), out val)) continue;
                result = Math.Min(result, val);
                emptySet = false;
            }
            return !emptySet ? result : DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NotEqualsConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !Util.StrEquals(value, parameter);
        }

        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return DependencyProperty.UnsetValue;
            for (int i = 1; i < values.Length; ++i)
                if (Util.StrEquals(values[0], values[i])) return false;
            return true;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SubscriptConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList list = value as IList;
            if (list == null) return DependencyProperty.UnsetValue;
            int index;
            if (parameter is int) index = (int)parameter;
            else if (!int.TryParse(Util.Str(parameter), out index))
                return DependencyProperty.UnsetValue;
            return list[index];
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan)) return DependencyProperty.UnsetValue;
            TimeSpan time = (TimeSpan)value;
            StringBuilder result = new StringBuilder(8);
            if (time.TotalHours < 1) result.AppendFormat("{0}", time.Minutes);
            else result.AppendFormat("{0}:{1:00}", (int)time.TotalHours, time.Minutes);
            result.AppendFormat(":{0:00}", time.Seconds);
            return result.ToString();
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan result;
            return TimeSpan.TryParse(value as string, out result)
                ? result : DependencyProperty.UnsetValue;
        }
    }
}