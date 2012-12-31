namespace SDammann.Utils.Settings {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;
    using IO;
    using Microsoft.Phone.Shell;


    /// <summary>
    /// Represents the base class for application settings
    /// </summary>
    public abstract class SettingsBase : INotifyPropertyChanged {
        private static readonly object SyncRoot = new object();
        private const char PrefixSeperator = '_';

        private readonly IIsolatedStorageSettings isolatedStorageSettings;

        private readonly bool autoSave;
        private readonly bool isSynchronized;
        private readonly bool designMode;

        private readonly string settingsPrefix;
        private readonly Dictionary<string, object> cachedValues;

        /// <summary>
        /// Gets a value indicating whether this instance automatically saves items to the persistent storage. If false, <see cref="Save"/> must be used to update
        /// persistent storage with the internal cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto saving; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSave {
            [DebuggerStepThrough]
            get { return this.autoSave; }
        }

        /// <summary>
        ///   Sets the <see cref="System.Object" /> with the specified key and value. 
        ///   The key should be equal to the property that calls this.
        /// </summary>
        /// <remarks>
        /// Undocumented by MSDN, but very much true: You can set any [<see cref="DataContractAttribute"/>] that is public with a public parameterless constructor.
        /// </remarks>
        protected void Set<T>(string key, T value) {
            // ignore design-time set requests
            if (this.designMode) { return; }

            // set value, optionally locking
            try {
                if (this.isSynchronized) {
                    Monitor.Enter(SyncRoot);
                }
                this.SetSetting(this.settingsPrefix + key, value);
            } finally {
                if (this.isSynchronized) {
                    Monitor.Exit(SyncRoot);
                }
            }
            
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key. Returns the specified default value if the key is not found.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="designTimeValue">The design time value.</param>
        /// <returns></returns>
        protected T Get<T>(string key, T defaultValue, T designTimeValue) {
            return Get(key, defaultValue, () => designTimeValue);
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key. Returns the specified default value if the key is not found.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="designTimeValueFactory">The design time value factory. May be null. If null, <paramref name="defaultValue"/> is used.</param>
        /// <returns></returns>
        protected T Get<T>(string key, T defaultValue, Func<T> designTimeValueFactory = null) {
            return Get(key, () => defaultValue, designTimeValueFactory);
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key. Returns the specified default value if the key is not found.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValueFactory">A method that returns the default value for the setting. May be null. If null, the default value of <typeparamref name="T"/> is used.</param>
        /// <param name="designTimeValueFactory">The design time value factory. May be null. If null, <paramref name="defaultValueFactory"/> is used.</param>
        /// <returns></returns>
        protected T Get<T>(string key, Func<T> defaultValueFactory = null, Func<T> designTimeValueFactory = null) {
            defaultValueFactory = defaultValueFactory ?? (() => default(T));

            // return design-time value if required
            if (this.designMode) {
                return (designTimeValueFactory ?? defaultValueFactory).Invoke();
            }

            // get value, optionally locking
            try {
                if (this.isSynchronized) {
                    Monitor.Enter(SyncRoot);
                }

                return this.GetSetting(this.settingsPrefix + key, defaultValueFactory);
            } finally {
                if (this.isSynchronized) {
                    Monitor.Exit(SyncRoot);
                }
            }
        }

        /// <summary>
        /// Sets the setting with the specified key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void SetSetting<T>(string key, T value) {
            // get original value if available
            // ... from cache
            Object originalObject;
            T originalValue;
            if (!this.cachedValues.TryGetValue(key, out originalObject)) {
                // ... from isolated storage settings
                this.isolatedStorageSettings.TryGetValue(key, out originalValue);
            } else {
                originalValue = (T) originalObject;
            }
            
            // update value in cache
            this.cachedValues [key] = value;
            if (this.autoSave) {
                this.isolatedStorageSettings[key] = value;
            }

            // check for property change and raise event
            if (!EqualityComparer<T>.Default.Equals(originalValue, value)) {
                this.OnPropertyChanged(new PropertyChangedEventArgs(key));
            }
        }

        /// <summary>
        /// Gets the setting or uses the <paramref name="defaultValueFactory"/> to retrieve a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValueFactory">The default value factory.</param>
        /// <returns></returns>
        private T GetSetting <T>(string key, Func<T> defaultValueFactory) {
            // get value from cache
            Object retrievedObject;
            T retrievedValue;
            if (!this.cachedValues.TryGetValue(key, out retrievedObject)) {
                // get from isolated sotrange
                if (!this.isolatedStorageSettings.TryGetValue(key, out retrievedValue)) {
                    retrievedValue = defaultValueFactory.Invoke();
                }

                // update cache and persistent storage
                this.cachedValues[key] = retrievedValue;
                
                if (this.autoSave) {
                    this.isolatedStorageSettings[key] = retrievedValue;
                }
            } else {
                retrievedValue = (T) retrievedObject;
            }

            return retrievedValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        /// <param name="autoSave">if set to <c>true</c> automatically save the settings when set.</param>
        /// <param name="isSynchronized">if set to <c>true</c> if the object is thread-safe. If true, no other components should access the isolated storage settings via a non-thread-safe object.</param>
        /// <param name="customSettingsKey">The custom settings key to use for backwards compatibility.</param>
        protected SettingsBase(bool autoSave = true, bool isSynchronized = false, string customSettingsKey =null) {
            this.autoSave = autoSave;
            this.isSynchronized = isSynchronized;

            this.settingsPrefix = (customSettingsKey ?? this.GetType().FullName) + PrefixSeperator;
            this.cachedValues = new Dictionary<string, object>();

            if (!DesignerProperties.IsInDesignTool) {
                this.isolatedStorageSettings = ServiceLocator.ServiceResolver.GetService<IIsolatedStorageSettings>();
                this.designMode = false;
            } else {
                this.designMode = true;
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///   Clears all the settings
        /// </summary>
        public void Clear() {
            if (this.designMode) { return; }

            var settingKeys = new string[this.isolatedStorageSettings.Keys.Count];
            this.isolatedStorageSettings.Keys.CopyTo(settingKeys, 0);

            foreach (string settingKey in settingKeys) {
                if (settingKey.StartsWith(this.settingsPrefix)) {
                    this.isolatedStorageSettings.Remove(settingKey);
                }
            }
        }

        /// <summary>
        /// Saves all the settings to persistent storage. Only required when <see cref="AutoSave"/> is true. 
        /// </summary>
        /// <remarks>
        /// If <see cref="AutoSave"/> is true, any call to this method is ignored and will just explicitly save the internal storage object.
        /// It is recommended to call this method in the <see cref="PhoneApplicationService.Deactivated"/> and <see cref="PhoneApplicationService.Closing"/> event handlers.
        /// </remarks>
        public void Save() {
            if (!this.autoSave) {
                foreach (KeyValuePair<string, object> cachedValue in cachedValues) {
                    this.isolatedStorageSettings[cachedValue.Key] = cachedValue.Value;
                }
            }

            this.isolatedStorageSettings.Save();
        }

        /// <summary>
        /// Reloads all settings from isolated storage, discarding any newly set cached values. This method has no effect if <see cref="AutoSave"/> is true.
        /// </summary>
        public void Reload() {
            this.cachedValues.Clear();
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj"> The <see cref="System.Object" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c> . </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) {
            SettingsBase other = obj as SettingsBase;

            return other != null && other.settingsPrefix == this.settingsPrefix;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="System.String" /> that represents this instance. </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() {
            return this.settingsPrefix;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() {
            return this.settingsPrefix.GetHashCode();
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> of the current instance.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> instance that represents the exact runtime type of the current instance.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType() {
            return base.GetType();
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, e);
            }
        }

        /// <summary>
        /// Initializes the <see cref="SettingsBase"/> class.
        /// </summary>
        static SettingsBase() {
            // inject isolated storage settings runtime object
            ServiceLocator.ServiceResolver.AddMapping<IIsolatedStorageSettings>(() => new RuntimeIsolatedStorageSettings());
        }
    }
}