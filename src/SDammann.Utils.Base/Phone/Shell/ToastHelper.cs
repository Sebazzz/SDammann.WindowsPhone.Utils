namespace SDammann.Utils.Phone.Shell {
    using System;
    using System.Collections.Generic;
    using Microsoft.Phone.Shell;


    /// <summary>
    ///   Provides a convenience method for showing a toast that won't annoy the user outside of their normal waking hours (as defined by the application)
    /// </summary>
    public static class ToastHelper {
        /// <summary>
        ///   The default time to use if (for some reason) a day-of-week can't be found. This default ensures that the user is never in "waking hours" (ie, no toasts will show)
        /// </summary>
        private static readonly TimeSegment DefaultTimeSegment = new TimeSegment(10, 10);

        /// <summary>
        ///   The waking hours as defined by the user, or the defaults if no custom times are specified
        /// </summary>
        /// <remarks>
        ///   The default waking hours are 8am - 10pm on weekdays and 10am - 10pm on weekends.
        /// </remarks>
        public static Dictionary<string, TimeSegment> WakingHours { get; private set; }

        /// <summary>
        ///   Sets up the default waking hours
        /// </summary>
        static ToastHelper() {
            var defaultWakingHours = new Dictionary<string, TimeSegment>();
            defaultWakingHours ["Monday"] = new TimeSegment(8, 22);
            defaultWakingHours ["Tuesday"] = new TimeSegment(8, 22);
            defaultWakingHours ["Wednesday"] = new TimeSegment(8, 22);
            defaultWakingHours ["Thursday"] = new TimeSegment(8, 22);
            defaultWakingHours ["Friday"] = new TimeSegment(8, 22);
            defaultWakingHours ["Saturday"] = new TimeSegment(10, 22);
            defaultWakingHours ["Sunday"] = new TimeSegment(10, 22);

            WakingHours = defaultWakingHours;
        }

        /// <summary>
        ///   Simple check to see if the user is likely to be awake at the given time
        /// </summary>
        /// <param name="time"> The time to check </param>
        /// <returns> True if the time represents normal "awake" hours; false otherwise </returns>
        private static bool IsWakingHours (DateTime time) {
            string day = time.DayOfWeek.ToString();
            TimeSegment segment = null;
            if (WakingHours == null || WakingHours.ContainsKey(day) != true) {
                segment = DefaultTimeSegment;
            } else {
                segment = WakingHours [day];
            }

            if (time.Hour >= segment.StartTime.Hours && time.Hour < segment.EndTime.Hours) {
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Shows a toast
        /// </summary>
        /// <param name="title"> The title of the toast </param>
        /// <param name="content"> The content of the toast </param>
        /// <param name="navigationUri"> The URI to navigate to if the toast is clicked </param>
        /// <param name="checkWakingHours"> </param>
        /// <remarks>
        ///   The toast won't show outside of normal waking hours (in case it wakes you up!)
        /// </remarks>
        public static void ShowToast (string title, string content, Uri navigationUri, bool checkWakingHours=false) {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content)) {
                return;
            }

            if (checkWakingHours && !IsWakingHours(DateTime.Now)) {
                return;
            }

            ShellToast toast = new ShellToast();
            toast.Title = title;
            toast.Content = content;
            if (navigationUri != null) {
                toast.NavigationUri = navigationUri;
            }

            toast.Show();
        }
    }
}