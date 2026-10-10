namespace BasicGameFrameworkLibrary.Core.CommonInterfaces;
public interface IStateChanged
{
    event Action? StateChanged;
}