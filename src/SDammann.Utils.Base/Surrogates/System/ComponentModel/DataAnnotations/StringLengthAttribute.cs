// ReSharper disable CheckNamespace
namespace System.ComponentModel.DataAnnotations {
    using Diagnostics;

    /// <summary />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    [Conditional("ExcludeBySilverlight")]
    public sealed class StringLengthAttribute : ValidationAttribute {

        /// <summary />
        public int MinimumLength { get; set; }

        /// <summary />
        public int MaximumLength { get; set; }

        /// <summary />
        public StringLengthAttribute(int maximumLength) {
            this.MaximumLength = maximumLength;
        }
    }
}
// ReSharper restore CheckNamespace