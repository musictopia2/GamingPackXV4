namespace BasicGameFrameworkLibrary.Core.ViewModelInterfaces;

public interface IBeginningColorViewModel : IScreen
{
    string Turn { get; set; }
    string Instructions { get; set; }
}