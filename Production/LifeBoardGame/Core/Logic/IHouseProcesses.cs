namespace LifeBoardGame.Core.Logic;
public interface IHouseProcesses
{
    Task ChoseHouseAsync(int house);
    void LoadHouseList();
    Task ShowYourHouseAsync();
}