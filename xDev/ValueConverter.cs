using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace xDev
{
    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
        }
    }

    public class IsStringNotEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsStringNotEmpty can only be used OneWay.");
        }
    }

    public class ColorToShadowColorConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Only touch the shadow color if it's a solid color, do not mess up other fancy effects
            if (value is SolidColorBrush)
            {
                Color color = ((SolidColorBrush)value).Color;
                var r = Transform(color.R);
                var g = Transform(color.G);
                var b = Transform(color.B);

                // return with Color and not SolidColorBrush, otherwise it will not work
                // This means that most likely the Color -> SolidBrushColor conversion does the RBG -> sRBG conversion somewhere...
                return Color.FromArgb(color.A, r, g, b);
            }

            return value;
        }

        private byte Transform(byte source)
        {
            // see http://en.wikipedia.org/wiki/SRGB
            return (byte)(Math.Pow(source / 255d, 1 / 2.2d) * 255);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("ColorToShadowColorConverter is a OneWay converter.");
        }

        #endregion
    }
    public class TrimmedTextBlockVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            FrameworkElement textBlock = (FrameworkElement)value;

            textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (((FrameworkElement)value).ActualWidth < ((FrameworkElement)value).DesiredSize.Width)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
