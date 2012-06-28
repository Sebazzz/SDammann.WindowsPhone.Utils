namespace SDammann.Utils.Windows.Controls {
    using System;
    using System.Windows;
    using System.Windows.Media;


    /// <summary>
    ///   Represents an extra color resource
    /// </summary>
    public sealed class ExtraColorResources {
        private static readonly Lazy<Color> ComplementaryColorPv = new Lazy<Color>(CreateComplementaryColor);

        private static readonly Lazy<Brush> ComplementaryColorBrushPv =
                new Lazy<Brush>(() => new SolidColorBrush(ComplementaryColorPv.Value));

        public static Color ComplementaryColor {
            get { return ComplementaryColorPv.Value; }
        }

        public static Brush ComplementaryColorBrush {
            get { return ComplementaryColorBrushPv.Value; }
        }

        private static Color CreateComplementaryColor() {
            Color currentAccentColor = (Color) Application.Current.Resources ["PhoneAccentColor"];

            return Media.ColorUtilities.AsComplementaryColor(currentAccentColor);
        }
    }
}