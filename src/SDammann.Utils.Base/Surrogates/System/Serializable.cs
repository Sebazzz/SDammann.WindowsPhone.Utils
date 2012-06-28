// ReSharper disable CheckNamespace
namespace System {
    using Diagnostics;

    /// <summary>
    /// Surrogate for the SerializableAttribute in the System namespace of the .NET Full framework
    /// </summary>
    /// <remarks>
    /// Note: When generating dependency graphs, Visual Studio somehow breaks on this, stating "SerializableAttribute" is not present in mscorlib.dll
    /// But, luckily, by including the <see cref="ConditionalAttribute"/> the attribute will actually never be applied as the constance given is not true.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    [Conditional("ExcludeBySilverlight")]
    public sealed class SerializableAttribute : Attribute {
        // nothing
    }
}
// ReSharper restore CheckNamespace