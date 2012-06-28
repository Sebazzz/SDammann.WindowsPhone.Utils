using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SDammann.Utils.Settings {
    /// <summary>
    /// Gets the settings for this application
    /// </summary>
    [NotifyPropertyWeaver.DoNotNotify]
    public sealed class Settings : SettingsBase {
        private static Settings _Instance;

        /// <summary>
        ///   Gets or sets the login credentials to use
        /// </summary>
        public LoginCredentials LoginCredentials {
            [DebuggerStepThrough]
            get { return this.Get("LoginCredentials", () => new LoginCredentials()); }
            [DebuggerStepThrough]
            set { this.Set("LoginCredentials", value); }
        }


        /// <summary>
        /// Gets or sets the simyo information of the user
        /// </summary>
        public SimyoInformation SimyoInfo {
            [DebuggerStepThrough]
            get { return this.Get("SimyoInfo", () => default(SimyoInformation)); }
            [DebuggerStepThrough]
            set { this.Set("SimyoInfo", value); }
        }

        /// <summary>
        /// Gets or sets whether or not live tile updates are enabled
        /// </summary>
        public bool IsLiveTileUpdateEnabled {
            [DebuggerStepThrough]
            get { return this.Get("IsLiveTileUpdateEnabled", false); }
            [DebuggerStepThrough]
            set { this.Set("IsLiveTileUpdateEnabled", value); }
        }
        /// <summary>
        ///   Gets the default settings object
        /// </summary>
        public static Settings Instance {
            [DebuggerStepThrough]
            get { return _Instance = (_Instance ?? new Settings()); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        private Settings() : base(true, false) {
        }
    }
}
