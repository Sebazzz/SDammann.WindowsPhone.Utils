namespace SDammann.Utils.Settings {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using IO;
    using Microsoft.Phone.Shell;
    using ServiceLocator;

    /// <summary>
    /// Mock interface for <see cref="SettingsBase"/>
    /// </summary>
    public interface ISettingsBase : INotifyPropertyChanged {
        /// <summary>
        /// Gets a value indicating whether this instance automatically saves items to the persistent storage. If false, <see cref="Save"/> must be used to update
        /// persistent storage with the internal cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto saving; otherwise, <c>false</c>.
        /// </value>
        bool AutoSave { [DebuggerStepThrough] get; }

        /// <summary>
        ///   Clears all the settings
        /// </summary>
        void Clear();

        /// <summary>
        /// Saves all the settings to persistent storage. Only required when <see cref="SettingsBase.AutoSave"/> is true. 
        /// </summary>
        /// <remarks>
        /// If <see cref="SettingsBase.AutoSave"/> is true, any call to this method is ignored and will just explicitly save the internal storage object.
        /// It is recommended to call this method in the <see cref="PhoneApplicationService.Deactivated"/> and <see cref="PhoneApplicationService.Closing"/> event handlers.
        /// </remarks>
        void Save();

        /// <summary>
        /// Reloads all settings from isolated storage, discarding any newly set cached values. This method has no effect if <see cref="SettingsBase.AutoSave"/> is true.
        /// </summary>
        void Reload();
    }

    /// <summary>
    /// Represents the base class for application settings
    /// </summary>
    public abstract class SettingsBase : ISettingsBase {
        private const string SettingsVersionKeyTemplate = "{0}{1}_SDammannSettingsBaseVersion";
        private const char PrefixSeperator = '_';
        private static readonly object SyncRoot = new object();

        private bool _isVersionUpgradeChecked;
        private bool _isMigratingSettings;
        private readonly int _version;
        private readonly bool _autoSave;
        private readonly Dictionary<string, object> _cachedValues;
        private readonly bool _designMode;
        private readonly bool _isSynchronized;
        private readonly IIsolatedStorageSettings _isolatedStorageSettings;

        private readonly string _settingsPrefix;

        /// <summary>
        /// Gets the current declared (compiled) settings version
        /// </summary>
        protected int DeclaredVersion {
            get { return this._version; }
        }

        /// <summary>
        /// Gets the actual version (isolated storage) of the settings. When null, no version was used.
        /// </summary>
        protected int? PersistentVersion {
            get {
                string key = String.Format(SettingsVersionKeyTemplate, this._settingsPrefix, PrefixSeperator);
                return GetSetting<int?>(key, () => null);
            }
            set {
                string key = String.Format(SettingsVersionKeyTemplate, this._settingsPrefix, PrefixSeperator);
                SetSetting(key, value);
            }
        }

        /// <summary>
        /// Initializes the <see cref="SettingsBase"/> class.
        /// </summary>
        static SettingsBase() {
            // inject isolated storage settings runtime object
            ServiceResolver.AddMapping<IIsolatedStorageSettings>(() => new RuntimeIsolatedStorageSettings());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        /// <param name="version">Declared compiled settings version</param>
        /// <param name="autoSave">if set to <c>true</c> automatically save the settings when set.</param>
        /// <param name="isSynchronized">if set to <c>true</c> if the object is thread-safe. If true, no other components should access the isolated storage settings via a non-thread-safe object.</param>
        /// <param name="customSettingsKey">The custom settings key to use for backwards compatibility.</param>
        protected SettingsBase(int version, bool autoSave = true, bool isSynchronized = false, string customSettingsKey = null) {
            this._version = version;
            this._autoSave = autoSave;
            this._isSynchronized = isSynchronized;

            this._settingsPrefix = (customSettingsKey ?? this.GetType().FullName) + PrefixSeperator;
            this._cachedValues = new Dictionary<string, object>();

            if (!DesignerProperties.IsInDesignTool) {
                this._isolatedStorageSettings = ServiceResolver.GetService<IIsolatedStorageSettings>();
                this._designMode = false;
            }
            else {
                this._designMode = true;
            }
        }

        /// <summary>
        /// Executes any pending version upgrades
        /// </summary>
        protected void ExecuteVersionUpgrade() {
            if (this._isVersionUpgradeChecked) {
                return;
            }

            int? persistedVersion = this.PersistentVersion;
            
            // check if current version equals: no upgrade necessary
            if (persistedVersion != null && persistedVersion.Value == this._version) {
                this._isVersionUpgradeChecked = true;
                return;
            }

            // if persisted version is null, we're executing an upgrade 
            int newVersion;
            
            // migrate pre-version support updates
            if (persistedVersion == null) {
                // check we have any settings actually
                if (this._isolatedStorageSettings.Keys.OfType<string>().Count(k => k.StartsWith(this._settingsPrefix, StringComparison.OrdinalIgnoreCase)) <= 1) {
                    // always the 'persisted version' key will be made, hence the '1'
                    this.PersistentVersion = this.DeclaredVersion;
                    this._isVersionUpgradeChecked = true;

                    this.Save();
                    return;
                }

                this._isMigratingSettings = true;
                this.MigrateFromUnknownToFirstVersion(out newVersion);
                this.PersistentVersion = newVersion;
            } else {
                this._isMigratingSettings = true;
                newVersion = persistedVersion.Value;
            }

            // while we're not up-to-date, execute upgrades
            while (newVersion < this.DeclaredVersion) {
                this.MigrateVersion(newVersion, out newVersion);
            }

            // set version
            this.PersistentVersion = DeclaredVersion;
            this.Save();
            this._isVersionUpgradeChecked = true;
            this._isMigratingSettings = false;
        }

        private void EnsureCorrectVersion() {
            if (this._isVersionUpgradeChecked || this._isMigratingSettings) {
                return;
            }

            Debug.WriteLine("Warning: ExecuteVersionUpgrade was NOT called in the constructor. For proper performance, please call this method.");
            this.ExecuteVersionUpgrade();

            Debug.Assert(this._isVersionUpgradeChecked);
        }

        #region ISettingsBase Members

        /// <summary>
        /// Gets a value indicating whether this instance automatically saves items to the persistent storage. If false, <see cref="Save"/> must be used to update
        /// persistent storage with the internal cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto saving; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSave {
            [DebuggerStepThrough]
            get { return this._autoSave; }
        }

        /// <summary />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   Clears all the settings
        /// </summary>
        public void Clear() {
            if (this._designMode) {
                return;
            }

            // clear settings with our prefix
            IEnumerable<string> keys = this._isolatedStorageSettings.Keys.OfType<string>().Where(s => s.StartsWith(_settingsPrefix + PrefixSeperator));
            foreach (string settingKey in keys) {
                if (settingKey.StartsWith(this._settingsPrefix)) {
                    this._isolatedStorageSettings.Remove(settingKey);
                }
            }

            this.Reload();
        }

        /// <summary>
        /// Saves all the settings to persistent storage. Only required when <see cref="AutoSave"/> is true. 
        /// </summary>
        /// <remarks>
        /// If <see cref="AutoSave"/> is true, any call to this method is ignored and will just explicitly save the internal storage object.
        /// It is recommended to call this method in the <see cref="PhoneApplicationService.Deactivated"/> and <see cref="PhoneApplicationService.Closing"/> event handlers.
        /// </remarks>
        public virtual void Save() {
            if (!this._autoSave) {
                foreach (var cachedValue in this._cachedValues) {
                    this._isolatedStorageSettings[cachedValue.Key] = cachedValue.Value;
                }
            }

            this._isolatedStorageSettings.Save();
        }

        /// <summary>
        /// Reloads all settings from isolated storage, discarding any newly set cached values. This method has no effect if <see cref="AutoSave"/> is true.
        /// </summary>
        public void Reload() {
            this._cachedValues.Clear();
        }

        #endregion

        /// <summary>
        ///   Sets the <see cref="System.Object" /> with the specified key and value. 
        ///   The key should be equal to the property that calls this.
        /// </summary>
        /// <remarks>
        /// Undocumented by MSDN, but very much true: You can set any [DataContractAttribute] that is public with a public parameterless constructor.
        /// </remarks>
        protected void Set<T>(string key, T value) {
            // ignore design-time set requests
            if (this._designMode) {
                return;
            }

            this.EnsureCorrectVersion();

            // set value, optionally locking
            try {
                if (this._isSynchronized) {
                    GetSettingsLock();
                }
                this.SetSetting(this._settingsPrefix + key, value);
            }
            finally {
                if (this._isSynchronized) {
                    ReleaseSettingsLock();
                }
            }
        }

        /// <summary>
        /// Clears the value for the specified setting from the persistent store
        /// </summary>
        /// <param name="key"></param>
        protected void Clear(string key) {
            // ignore design-time clear requests
            if (this._designMode) {
                return;
            }

            // clear value, optionally locking
            try {
                if (this._isSynchronized) {
                    GetSettingsLock();
                }

                this.ClearSetting(this._settingsPrefix + key);
            } finally {
                if (this._isSynchronized) {
                    ReleaseSettingsLock();
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
            if (this._designMode) {
                return (designTimeValueFactory ?? defaultValueFactory).Invoke();
            }

            this.EnsureCorrectVersion();

            // get value, optionally locking
            try {
                if (this._isSynchronized) {
                    GetSettingsLock();
                }

                return this.GetSetting(this._settingsPrefix + key, defaultValueFactory);
            }
            finally {
                if (this._isSynchronized) {
                    ReleaseSettingsLock();
                }
            }
        }

        /// <summary>
        /// Releases a lock on the settings
        /// </summary>
        private static void ReleaseSettingsLock() {
            Monitor.Exit(SyncRoot);
        }

        /// <summary>
        /// Acquires a lock on the settings
        /// </summary>
        private static void GetSettingsLock() {
            Monitor.Enter(SyncRoot);
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
            if (!this._cachedValues.TryGetValue(key, out originalObject)) {
                // ... from isolated storage settings
                this._isolatedStorageSettings.TryGetValue(key, out originalValue);
            }
            else {
                originalValue = (T) originalObject;
            }

            // update value in cache
            this._cachedValues[key] = value;
            if (this._autoSave) {
                this._isolatedStorageSettings[key] = value;
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
        private T GetSetting<T>(string key, Func<T> defaultValueFactory) {
            // get value from cache
            Object retrievedObject;
            T retrievedValue;
            if (!this._cachedValues.TryGetValue(key, out retrievedObject)) {
                // get from isolated sotrange
                if (!this._isolatedStorageSettings.TryGetValue(key, out retrievedValue)) {
                    retrievedValue = defaultValueFactory.Invoke();
                }

                // update cache and persistent storage
                this._cachedValues[key] = retrievedValue;

                if (this._autoSave) {
                    this._isolatedStorageSettings[key] = retrievedValue;
                }
            }
            else {
                retrievedValue = (T) retrievedObject;
            }

            return retrievedValue;
        }

        /// <summary>
        /// Clears the setting specified
        /// </summary>
        private void ClearSetting(string key) {
            Object retrievedObject;
            this._isolatedStorageSettings.Remove(key);
            if (this._cachedValues.TryGetValue(key, out retrievedObject)) {
                this._cachedValues.Remove(key);
            }
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj"> The <see cref="System.Object" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c> . </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) {
            var other = obj as SettingsBase;

            return other != null && other._settingsPrefix == this._settingsPrefix;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="System.String" /> that represents this instance. </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() {
            return this._settingsPrefix + ":" + this._version;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() {
            return this._settingsPrefix.GetHashCode() ^ this._version.GetHashCode();
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
        /// Migrates settings from a unknown version to the first version. Output parameter is the new version we're upgraded to.
        /// </summary>
        protected abstract void MigrateFromUnknownToFirstVersion(out int newVersion);

        /// <summary>
        /// Migrates settings from the specified source version, output parameter specifies new version the logic has upgraded the settings to.
        /// </summary>
        /// <param name="sourceVersion">Declared settings version</param>
        /// <param name="newVersion"> </param>
        protected abstract void MigrateVersion(int sourceVersion, out int newVersion);
    }
}