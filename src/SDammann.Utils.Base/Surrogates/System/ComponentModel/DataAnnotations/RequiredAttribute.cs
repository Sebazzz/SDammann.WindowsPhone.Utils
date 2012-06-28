// ReSharper disable CheckNamespace
namespace System.ComponentModel.DataAnnotations {
    using Diagnostics;


    /// <summary />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    [Conditional("ExcludeBySilverlight")]
    public sealed class RequiredAttribute : ValidationAttribute {

    }
}
// ReSharper restore CheckNamespace