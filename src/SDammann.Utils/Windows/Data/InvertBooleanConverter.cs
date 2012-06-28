namespace SDammann.Utils.Windows.Data {
    using System;
    using System.Globalization;
    using System.Windows.Data;


    /// <summary>
    /// Converts a <c>true</c> to <c>false</c> and vice versa
    /// </summary>
    public sealed class InvertBooleanConverter : IValueConverter {
        #region IValueConverter Members

        /// <summary/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool original = (bool) value;
            return !original;
        }
        /// <summary/>
        public object ConvertBack (object value, Type targetType, object parameter,
                                   CultureInfo culture) {
            bool original = (bool) value;
            return !original;
        }

        #endregion
    }
}