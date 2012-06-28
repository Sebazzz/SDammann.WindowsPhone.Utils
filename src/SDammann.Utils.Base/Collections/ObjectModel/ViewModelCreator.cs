namespace SDammann.Utils.Collections.ObjectModel {
    /// <summary>
    ///   Delegate with the responsibility of creating a <typeparamref name="TViewModel" /> object using the specified <typeparamref
    ///    name="TModel" /> instance
    /// </summary>
    /// <typeparam name="TViewModel"> The type of the view model. </typeparam>
    /// <typeparam name="TModel"> The type of the model. </typeparam>
    /// <param name="model"> The model. </param>
    /// <returns> </returns>
    // ReSharper disable TypeParameterCanBeVariant
    public delegate TViewModel ViewModelCreator<TViewModel, TModel>(TModel model)
            where TViewModel : class, IViewModelFor<TModel>
            where TModel : class;
    // ReSharper restore TypeParameterCanBeVariant
}