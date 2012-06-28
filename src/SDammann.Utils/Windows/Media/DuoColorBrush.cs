namespace SDammann.Utils.Windows.Media {
    using System.Linq;
    using System.Windows.Media;
    using Linq;


    /// <summary>
    ///   A helper class that holds a brush and a complementary brush in a complementary color
    /// </summary>
    public sealed class DuoColorBrush {
        private Brush originalBrush;

        /// <summary>
        ///   Gets the complementary brush for <see cref="OriginalBrush"/>
        /// </summary>
        /// <value> The complementary brush. </value>
        public Brush ComplementaryBrush { get; set; }

        /// <summary>
        /// Gets or sets the original brush to create the complementary brush for
        /// </summary>
        public Brush OriginalBrush {
            get { return this.originalBrush; }
            set {
                this.originalBrush = value;

                this.ComplementaryBrush = GenerateComplementaryBrush(this.originalBrush);
            }
        }

        private Brush GenerateComplementaryBrush (Brush brush) {
            SolidColorBrush colorBrush = brush as SolidColorBrush;
            if (colorBrush != null) {
                SolidColorBrush newBrush = new SolidColorBrush();
                ObjectCopyHelper.Copy(colorBrush, newBrush);
                newBrush.Color = colorBrush.Color.AsComplementaryColor();
                return newBrush;
            }

            LinearGradientBrush linGradientBrush = brush as LinearGradientBrush;
            if (linGradientBrush != null) {
                LinearGradientBrush newBrush = new LinearGradientBrush();
                ObjectCopyHelper.Copy(linGradientBrush, newBrush, b => b.GradientStops);
                
                GradientStopCollection c = new GradientStopCollection();
                linGradientBrush.GradientStops.Select(g => new GradientStop() { Color = g.Color.AsComplementaryColor(), Offset = g.Offset }).ForEach(c.Add);
                newBrush.GradientStops = c;

                return newBrush;
            }

            RadialGradientBrush gradBrush = brush as RadialGradientBrush;
            if (gradBrush != null) {
                RadialGradientBrush newBrush = new RadialGradientBrush();
                ObjectCopyHelper.Copy(gradBrush, newBrush, b => b.GradientStops);

                GradientStopCollection c = new GradientStopCollection();
                gradBrush.GradientStops.Select(g => new GradientStop() { Color = g.Color.AsComplementaryColor(), Offset = g.Offset }).ForEach(c.Add);
                newBrush.GradientStops = c;

                return newBrush;
            }
            
            // we won't throw an exception, just return the original brush
            return brush;
        }
    }
}