namespace SDammann.Utils.Windows {
    using System.Windows;


    /// <summary>
    ///   Helper extensions for visibility
    /// </summary>
    public static class VisibilityHelper {
        /// <summary>
        ///   Converts a <see cref="Visibility" /> value to a <see cref="bool" /> with <see cref="Visibility.Visible"/> being <c>true</c>
        /// </summary>
        /// <param name="v"> </param>
        /// <returns> </returns>
        public static bool ToBoolean (this Visibility v) {
            return v == Visibility.Visible;
        }

        /// <summary>
        /// Inverts the specified visibility: <see cref="Visibility.Visible"/> to <see cref="Visibility.Collapsed"/> and vice versa
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Visibility Inverse(this Visibility v) {
            return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a <see cref="bool"/> value to a <see cref="Visibility"/> value, with <c>true</c> being <see cref="Visibility.Visible"/>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Visibility ToVisibility(this bool b) {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}