namespace SDammann.Utils.Windows {
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Navigation;
    using Design;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Threading;


    /// <summary>
    /// Base class for applications for WP7 using
    /// </summary>
    public class GeneralApplication : Application {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralApplication"/> class.
        /// </summary>
        public GeneralApplication() {
            UserInterfaceThreadDispatcher.Initialize();
        }

        /// <summary>
        /// Enables debugging helpers. Make sure to call this only in DEBUG mode.
        /// </summary>
        protected static void EnableDebuggingHelpers() {
            // do some stuff for debugging
            if (Debugger.IsAttached) {
                // Display the current frame rate counters.
                Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                MetroGridHelper.IsVisible = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached) {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
#else
            MessageBox.Show("A unknown navigation error occurred. Please report this error. The application will shutdown.",
                                "Navigation Error",
                                MessageBoxButton.OK);
#endif
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized;

        /// <summary>
        /// Invoke in application constructor. Bloilerplate code.
        /// </summary>
        protected void InitializePhoneApplication() {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        /// <summary>
        /// Occurs when the phone application has been initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e) {
            // Set the root visual to allow the application to render
            PhoneApplicationFrame applicationFrame = this.RootFrame;
            if (applicationFrame != null && this.RootVisual != applicationFrame) {
                this.RootVisual = this.RootFrame;
            }

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}