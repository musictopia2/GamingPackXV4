namespace LifeBoardGame.Core.Logic;
public interface ITwinProcesses
{
    Task GetTwinsAsync(BasicList<EnumGender> twinList);
}