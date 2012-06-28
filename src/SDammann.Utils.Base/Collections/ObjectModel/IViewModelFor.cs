namespace SDammann.Utils.Collections.ObjectModel {
    /// <summary>
    ///   Defines that the object implementing this interface is a view model to some object
    /// </summary>
    /// <typeparam name="TModel"> The type of model this object is view model to </typeparam>
    // ReSharper disable TypeParameterCanBeVariant
    public interface IViewModelFor<TModel> where TModel : class {
        /// <summary>
        ///   Gets the model this object is a view model to
        /// </summary>
        TModel Model { get; }
    }
    // ReSharper restore TypeParameterCanBeVariant
}