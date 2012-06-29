SDammann.WindowsPhone.Utils
===========================

Useful utility functions and extensions for Windows Phone

Documentation is a big TODO but to help you on your way:

Naming Convention
---------------------------
The folders and namespaces are very much like the .NET namespaces. For example, there is a Windows/Data folder. In code, the classes in those folders are in the SDammann.Utils.Windows.Data namespace. You can expect the same type of objects as one would expect in the System.Windows.Data namespace.

Projects
---------------------------
SDammann.Utils.Base: Contains most utility classes. All classes in this library are also safe to use by background agents.

SDammann.Utils: Contains non-backgroundworker safe classes. 


Highlights
---------------------------

Classes to check out:
- UserInterfaceThreadDispatcher and DelegateExtensions: Helper interface for dispatching asynchronous updates back to the UI
- SettingsBase: Base class for general application settings
- ListBoxEx: Extended version of the Windows Phone listbox that supports an empty view (shown when there are no items in the list), command binding for MVVM.
- GlobalApplication: Base class for you application definition (App.xaml) that provides you with debugging tools and prevents boilerplate code.

Style files to check out:
- TransitionStyles.xaml: Avoid that bloilerplate clutter of Silverlight Toolkit transitions and apply one of the style to your page.
- ListBoxStyles.xaml/ListBoxEmptyStretchTemplate: Fixes a bug in the standard Windows Phone listbox, where items are not horizontally stretched to its container.


Wiki pages
----------------------------
Check out the [wiki pages](https://github.com/Sebazzz/SDammann.WindowsPhone.Utils/wiki).