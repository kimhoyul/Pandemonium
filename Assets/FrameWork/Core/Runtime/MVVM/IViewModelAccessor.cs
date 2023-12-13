using CommunityToolkit.Mvvm.ComponentModel;

public interface IViewModelAccessor
{
    public ObservableObject ViewModelInstance { get; }
}
