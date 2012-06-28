namespace SDammann.Utils.Threading {
    using System;
    using System.Collections.Generic;
    using System.Windows;


    /// <summary>
    ///   Defines some extensions for delegates. Make sure to have the <see cref="UserInterfaceThreadDispatcher"/> initialized first.
    /// </summary>
    public static class DelegateExtensions {
        /// <summary>
        ///   Invokes the specified <paramref name="delegate" /> delegate on the correct thread
        /// </summary>
        /// <param name="delegate"> </param>
        /// <param name="arguments"> </param>
        public static void InvokeSafe (this Delegate @delegate, params object[] arguments) {
            if (@delegate == null) {
                return;
            }

            Delegate[] invokationList = @delegate.GetInvocationList();

            InvokeDelegateList(invokationList, arguments);
        }

        private static void InvokeDelegateList (IEnumerable<Delegate> invokationList, params object[] arguments) {
            foreach (Delegate del in invokationList) {
                DependencyObject depObject = del.Target as DependencyObject;

                if (depObject != null) {
                    if (depObject.CheckAccess()) {
                        del.DynamicInvoke(arguments);
                    } else {
                        depObject.Dispatcher.BeginInvoke(del, arguments);
                    }

                    continue;
                }

                ISynchronizedObject syncObject = del.Target as ISynchronizedObject;
                if (syncObject != null) {
                    syncObject.ObjectSynchronizationContext
                              .Post(d => ((Delegate) d).DynamicInvoke(arguments), del);
                    continue;
                }

                UserInterfaceThreadDispatcher.ExecuteDelegate(del, arguments);
            }
        }

        /// <summary>
        ///   Invokes the specified event safely on another thread
        /// </summary>
        /// <typeparam name="TEventArgs"> </typeparam>
        /// <param name="eventHandler"> </param>
        /// <param name="sender"> </param>
        /// <param name="eventArgs"> </param>
        public static void InvokeSafe<TEventArgs> (this EventHandler<TEventArgs> eventHandler,
                                                   object sender, TEventArgs eventArgs)
                where TEventArgs : EventArgs {
            if (eventHandler == null) {
                return;
            }

            Delegate[] invokationList = eventHandler.GetInvocationList();
            InvokeDelegateList(invokationList, sender, eventArgs);
        }
    }
}