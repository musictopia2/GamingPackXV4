namespace BasicGameFrameworkLibrary.Core.ViewModelInterfaces;
public interface IBlankGameVM : IMainScreen
{
    CommandContainer CommandContainer { get; set; }
}