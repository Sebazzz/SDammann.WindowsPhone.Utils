namespace SDammann.Utils.Phone.Marketplace {
    using System.Diagnostics;
    using Microsoft.Phone.Marketplace;


    /// <summary>
    ///   Helper class for determining if the application is in trial mode
    /// </summary>
    public static class TrialService {
        private static bool ApplicationIsTrialPrivate;

        /// <summary>
        ///   Gets if the application is in trial mode
        /// </summary>
        public static bool ApplicationIsTrial {
            [DebuggerStepThrough]
            get { return ApplicationIsTrialPrivate; }
        }

        /// <summary>
        ///   Refreshes the trail mode value - should be called after every app start or resume
        /// </summary>
        public static void RefreshTrailMode() {
#if DEBUG
            ApplicationIsTrialPrivate = false;
#else
            ApplicationIsTrialPrivate = new LicenseInformation().IsTrial();
#endif
        }
    }
}