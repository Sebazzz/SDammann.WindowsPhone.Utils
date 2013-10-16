namespace SDammann.Utils {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Threading;
    using Microsoft.Phone.Info;

    /// <summary>
    /// Static helper class for enabling diagnostic logging
    /// </summary>
    public static class DiagnosticLogger {
        private static IsolatedStorageFile _IsolatedStorage;
        private static string _FileName;

        /// <summary>
        /// Gets or sets if the log is enabled
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Enables the app log
        /// </summary>
        public static void Enable() {
            IsEnabled = true;
        }

        /// <summary>
        /// Disables the app log
        /// </summary>
        public static void Disable() {
            IsEnabled = false;

            try {
                _IsolatedStorage.DeleteFile(_FileName);
            } catch (Exception ex) {
                Debug.WriteLine("Couldn't delete file for diag: {0}", ex);
            }
        }

        /// <summary>
        /// Shuts down the logger
        /// </summary>
        public static void Shutdown() {
            _IsolatedStorage.Dispose();
            _IsolatedStorage = null;
        }

        /// <summary>
        /// Configures the diagnostic log
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="enable"> </param>
        public static void Configure(string fileName, bool enable) {
            try {
                _IsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

                _FileName = fileName;

                IsEnabled = enable;
                if (enable) {
                    WriteLogFileHeader();
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Writes an log file header
        /// </summary>
        private static void WriteLogFileHeader() {
            using (StreamWriter writer = new StreamWriter(_IsolatedStorage.OpenFile(_FileName, FileMode.OpenOrCreate))) {
                writer.WriteLine();
                writer.WriteLine("============ App Diagnostic Log");
                writer.WriteLine(" Device name: {0} ", DeviceStatus.DeviceName);
                writer.WriteLine(" Device mem : {0} ", DeviceStatus.DeviceTotalMemory);
                writer.WriteLine(" Culture    : {0}", Thread.CurrentThread.CurrentCulture);
                writer.WriteLine(" UICulture  : {0}", Thread.CurrentThread.CurrentUICulture);
                writer.WriteLine(" Timestamp  : {0}", DateTime.UtcNow);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Gets the logged text in the diagnostic log
        /// </summary>
        /// <returns></returns>
        public static string GetLoggedText() {
            using (IsolatedStorageFileStream stream = _IsolatedStorage.OpenFile(_FileName, FileMode.Open, FileAccess.Read)) {
                using (StreamReader streamReader = new StreamReader(stream)) {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Writes a simple string to the log
        /// </summary>
        /// <param name="simpleString"></param>
        public static void Write(string simpleString) {
            if (!IsEnabled) {
                return;
            }

            using (var fileWriter = GetFileWriter()) {
                fileWriter.WriteLine(simpleString);
                Debug.WriteLine(simpleString);
            }
        }

        public static void Write(string format, object arg0) {
            if (!IsEnabled) {
                return;
            }

            using (var fileWriter = GetFileWriter()) {
                fileWriter.WriteLine(format, arg0);
                Debug.WriteLine(format, arg0);
            }
        }

        public static void Write(string format, object arg0, object arg1) {
            if (!IsEnabled) {
                return;
            }

            using (var fileWriter = GetFileWriter()) {
                fileWriter.WriteLine(format, arg0, arg1);
                Debug.WriteLine(format, arg0, arg1);
            }
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public static void Write(string format, params object[] args) {
            if (!IsEnabled) {
                return;
            }

            using (var fileWriter = GetFileWriter()) {
                fileWriter.WriteLine(format, args);
                Debug.WriteLine(format, args);
            }
        }
        // ReSharper restore MethodOverloadWithOptionalParameter

        private static TextWriter GetFileWriter() {
            try {
                StreamWriter sr = new StreamWriter(_IsolatedStorage.OpenFile(_FileName, FileMode.Open, FileAccess.Write));
                sr.BaseStream.Position = sr.BaseStream.Length;

                return sr;
            } catch (Exception ex) {
                Debug.WriteLine("Couldn't open diagnostics file: {0}", ex);
                return new StringWriter();
            }
        }
    }
}