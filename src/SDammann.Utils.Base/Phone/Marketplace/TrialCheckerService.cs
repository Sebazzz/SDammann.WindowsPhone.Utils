namespace SDammann.Utils.Phone.Marketplace {
    using System.Windows;


    /// <summary>
    ///   An <see cref="IApplicationService" /> that calls the <see cref="TrialService"/> and refreshes the trial mode
    /// </summary>
    public sealed class TrialCheckerService : IApplicationService {
        /// <summary>
        /// Called by an application in order to initialize the application extension service.
        /// </summary>
        /// <param name="context">Provides information about the application state.</param>
        public void StartService(ApplicationServiceContext context) {
            TrialService.RefreshTrailMode();
        }

        /// <summary>
        /// Called by an application in order to stop the application extension service.
        /// </summary>
        public void StopService() {
            // nothing - no calls necessary here
        }
    }
}