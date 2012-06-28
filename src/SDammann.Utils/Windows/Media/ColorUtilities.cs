namespace SDammann.Utils.Windows.Media {
    using System.Windows.Media;


    /// <summary>
    ///   Helper extensions and utilities for <see cref="Color" />
    /// </summary>
    public static class ColorUtilities {
        /// <summary>
        ///   Creates a complementary color that matches the given <paramref name="color" /> .
        /// </summary>
        /// <param name="color"> </param>
        /// <returns> </returns>
        public static Color AsComplementaryColor (this Color color) {
            return new Color {
                                     R = (byte) (255 - color.R),
                                     G = (byte) (255 - color.G),
                                     B = (byte) (255 - color.B),
                                     A = color.A
                             };
        }
    }
}