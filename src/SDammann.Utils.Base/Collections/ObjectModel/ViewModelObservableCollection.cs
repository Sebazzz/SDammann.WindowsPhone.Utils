namespace SDammann.Utils.Collections.ObjectModel {
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;


    /// <summary>
    ///   Represents an observable collection where objects of <typeparamref name="TViewModel" /> are kept in sync as <typeparamref
    ///    name="TModel" /> objects are removed and added to the source collection of type <see
    ///    name="ObservableCollection{TModel}" /> . The items are also filtered.
    /// </summary>
    /// <typeparam name="TViewModel"> </typeparam>
    /// <typeparam name="TModel"> </typeparam>
    public class ViewModelObservableCollection<TViewModel, TModel> : ObservableCollection<TViewModel>, IDisposable
        where TViewModel : class, IViewModelFor<TModel>
        where TModel : class {
        private readonly SynchronizationContext creationContext;
        private readonly bool isSynchronizedCollection;
        private readonly Thread originalThread;
        private readonly SyncedReadOnlyObservableCollection<TModel> sourceCollection;
        private readonly ViewModelCreator<TViewModel, TModel> viewModelCreator;

        private bool disposed;

        private bool IsOnCurrentThread {
            get {
                Thread currentThread = Thread.CurrentThread;
                return currentThread.ManagedThreadId == this.originalThread.ManagedThreadId;
            }
        }

        /// <summary>
        ///   Gets the source collection being observed. Internal use only.
        /// </summary>
        internal SyncedReadOnlyObservableCollection<TModel> SourceCollection {
            [DebuggerStepThrough]
            get { return this.sourceCollection; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelObservableCollection&lt;TViewModel, TModel&gt;"/> class.
        /// </summary>
        /// <param name="viewModelCreator">The view model creator'delegate.</param>
        /// <param name="sourceCollection">The original collection.</param>
        /// <param name="synchronize">if set to <c>true</c> to synchronize the collection for use on a specific thread.</param>
        public ViewModelObservableCollection(ViewModelCreator<TViewModel, TModel> viewModelCreator,
                                             SyncedReadOnlyObservableCollection<TModel> sourceCollection,
                                             bool synchronize = false) {
            this.isSynchronizedCollection = synchronize;
            this.creationContext = synchronize ? SynchronizationContext.Current : null;
            this.originalThread = synchronize ? Thread.CurrentThread : null;

            this.viewModelCreator = viewModelCreator;
            this.sourceCollection = sourceCollection;

            INotifyCollectionChanged watcher = this.sourceCollection;
            watcher.CollectionChanged += this.OnSourceCollectionChanged;
        }


        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ViewModelObservableCollection&lt;TViewModel, TModel&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~ViewModelObservableCollection() {
            this.Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"> <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. </param>
        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    if (this.sourceCollection != null) {
                        INotifyCollectionChanged watcher = this.sourceCollection;
                        watcher.CollectionChanged -= this.OnSourceCollectionChanged;
                    }
                }

                this.disposed = true;
            }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Triggers an UI refresh for the item on the specified index
        /// </summary>
        /// <param name="index"></param>
        public void RefreshIndex(int index) {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[index], this[index], index));
        }

        /// <summary>
        ///   Called when the <see cref="sourceCollection" /> is changed
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data. </param>
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (this.isSynchronizedCollection && !this.IsOnCurrentThread) {
                this.creationContext.Post(delegate { this.OnSourceCollectionChanged(sender, e); }, null);
                return;
            }

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    var addedItems = e.NewItems.OfType<TModel>();
                    foreach (TModel item in addedItems) {
                        this.Add(this.viewModelCreator.Invoke(item));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removedItems = e.OldItems.OfType<TModel>();
                    foreach (TModel removedItem in removedItems) {
                        this.Remove(this.Single(vm => ReferenceEquals(vm.Model, removedItem)));
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    TModel newItem = e.NewItems.OfType<TModel>().FirstOrDefault();

                    if (newItem != null) {
                        this.SetItem(e.NewStartingIndex, this.viewModelCreator.Invoke(newItem));
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.ClearItems();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}