namespace BasicGameFrameworkLibrary.Core.ViewModelInterfaces;
public interface ISubmitText
{
    string Text { get; }
    ICustomCommand Command { get; }
}