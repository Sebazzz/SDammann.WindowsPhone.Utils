namespace SDammann.Utils.Collections.ObjectModel {
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Threading;


    /// <summary>
    /// Represents a <see cref="ReadOnlyObservableCollection{T}"/> that invokes it's events on the correct thread
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncedReadOnlyObservableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged {
        private NotifyCollectionChangedEventHandler collectionChanged;
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SyncedReadOnlyObservableCollection&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="list"> The list. </param>
        public SyncedReadOnlyObservableCollection (ObservableCollection<T> list) : base(list) {
            list.CollectionChanged += (sender, args) => this.OnCollectionChanged(args);
            ((INotifyPropertyChanged) list).PropertyChanged += (sender, args) => this.OnPropertyChanged(args);
        }


        // ReSharper disable DelegateSubtraction
        /// <summary>
        ///   Occurs when [collection changed].
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged {
            add { this.collectionChanged += value; }
            remove {
                if (this.collectionChanged != null) {
                    this.collectionChanged -= value;
                }
            }
        }

        /// <summary>
        ///   Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged {
            add { this.propertyChanged += value; }
            remove {
                if (this.propertyChanged != null) {
                    this.propertyChanged -= value;
                }
            }
        }

        // ReSharper restore DelegateSubtraction

        /// <summary>
        ///   Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="args"> The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data. </param>
        protected virtual void OnPropertyChanged (PropertyChangedEventArgs args) {
            PropertyChangedEventHandler ev = this.propertyChanged;

            if (ev == null) {
                return;
            }

            ev.InvokeSafe(this, args);
        }

        /// <summary>
        ///   Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="args"> The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data. </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
            NotifyCollectionChangedEventHandler ev = this.collectionChanged;

            if (ev == null) {
                return;
            }

            ev.InvokeSafe(this, args);
        }
    }
}