namespace BasicGameFrameworkLibrary.Core.ViewModelInterfaces;
public interface IBeginningColorViewModel
{
    string Turn { get; set; }
    string Instructions { get; set; }
    void Initialize();
    void Close();
}