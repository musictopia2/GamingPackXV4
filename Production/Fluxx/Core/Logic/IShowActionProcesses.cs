namespace Fluxx.Core.Logic;
public interface IShowActionProcesses
{
    Task ChoseOtherCardSelectedAsync(int deck);
    Task ShowRuleTrashedAsync(int selectedIndex);
    Task ShowLetsDoAgainAsync(int selectedIndex);
    Task ShowRulesSimplifiedAsync(BasicList<int> list);
    Task ShowDirectionAsync(int selectedIndex);
    Task ShowTradeHandAsync(int selectedIndex);
    Task ShowPlayerForCardChosenAsync(int selectedIndex);
    Task ShowChosenForEverybodyGetsOneAsync(BasicList<int> selectedList, int selectedIndex);
    Task ShowCardUseAsync(int deck);
}