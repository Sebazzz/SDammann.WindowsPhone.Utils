namespace SDammann.Utils.Windows.Controls {
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;


    /// <summary>
    ///   Extended version of a <see cref="ListBox" /> .
    /// </summary>
    /// <seealso cref="EmptyContent" />
    /// <seealso cref="ItemSelectCommand" />
    [ContentProperty ("EmptyContent")]
    public class ListBoxEx : ListBox {
        public static readonly DependencyProperty EmptyTemplateProperty =
                DependencyProperty.Register("EmptyTemplate",
                                            typeof (ControlTemplate),
                                            typeof (ListBoxEx),
                                            new PropertyMetadata(null));

        public static readonly DependencyProperty EmptyContentProperty =
                DependencyProperty.Register("EmptyContent", typeof (object), typeof (ListBoxEx), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ItemSelectCommandProperty =
                DependencyProperty.Register("ItemSelectCommand",
                                            typeof (ICommand),
                                            typeof (ListBoxEx),
                                            new PropertyMetadata(default(ICommand)));

        private bool designTimeShowEmptyContent;

        /// <summary>
        ///   Gets or sets the template for the empty listbox
        /// </summary>
        public ControlTemplate EmptyTemplate {
            get { return (ControlTemplate) this.GetValue(EmptyTemplateProperty); }
            set { this.SetValue(EmptyTemplateProperty, value); }
        }

        /// <summary>
        ///   Gets or sets the command to execute when an item is selected. This is only applicable if <see
        ///    cref="ListBox.SelectionMode" /> is <see cref="SelectionMode.Single" />
        /// </summary>
        public ICommand ItemSelectCommand {
            get { return (ICommand) this.GetValue(ItemSelectCommandProperty); }
            set { this.SetValue(ItemSelectCommandProperty, value); }
        }

        /// <summary>
        ///   Gets if the list is currently empty
        /// </summary>
        private bool IsListEmpty {
            get {
                if (this.Items.Count > 0) {
                    return false;
                }

                if (this.ItemsSource == null) {
                    return true;
                }

                // we cant do .Any on a Enumerable
                IEnumerator numerator = this.ItemsSource.GetEnumerator();

                return !numerator.MoveNext();
            }
        }

        /// <summary>
        ///   Gets the content shown when the listbox is empty and shows no items
        /// </summary>
        public object EmptyContent {
            get { return this.GetValue(EmptyContentProperty); }
            set { this.SetValue(EmptyContentProperty, value); }
        }


        public bool DesignTimeShowEmptyContent {
            get { return this.designTimeShowEmptyContent; }
            set {
                this.designTimeShowEmptyContent = value;

                if (DesignerProperties.GetIsInDesignMode(this)) {
                    this.Refresh();
                }
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ListBoxEx" /> class.
        /// </summary>
        public ListBoxEx() {
            this.DefaultStyleKey = typeof(ListBoxEx);
            
            // load style
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("/SDammann.Utils;component/Themes/generic.xaml", UriKind.Relative);
            this.Style = (Style) dict [this.DefaultStyleKey];
            this.ApplyTemplate();

            this.Refresh();

            // item selection command
            this.SelectionChanged += this.OnSelectionChanged;
        }

        private void OnSelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (this.SelectionMode != SelectionMode.Single || this.ItemSelectCommand == null) {
                return;
            }

            if (this.SelectedItem != null) {
                this.ItemSelectCommand.Execute(this.SelectedItem);
            }

            this.SelectedIndex = -1;
        }

        /// <summary>
        ///   Provides handling for the <see cref="E:System.Windows.Controls.ItemContainerGenerator.ItemsChanged" /> event.
        /// </summary>
        /// <param name="e"> A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> that contains the event data. </param>
        protected override void OnItemsChanged (NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);

            this.Refresh();
        }

        private void Refresh() {
            ItemsPresenter itemsControl = this.GetTemplateChild("ListBoxItemsControl") as ItemsPresenter;
            ContentControl contentPresenter = this.GetTemplateChild("ListBoxEmptyContent") as ContentControl;

            if (itemsControl == null || contentPresenter == null) {
                if (Debugger.IsAttached) {
                    Debugger.Break();
                }
                return;
            }

            if (this.IsListEmpty ||
                DesignerProperties.GetIsInDesignMode(this) && this.DesignTimeShowEmptyContent) {
                itemsControl.Visibility = Visibility.Collapsed;
                contentPresenter.Visibility = Visibility.Visible;
            } else {
                itemsControl.Visibility = Visibility.Visible;
                contentPresenter.Visibility = Visibility.Collapsed;
            }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
        }
    }
}