namespace GolfCardGame.Core.Logic;
public interface IBeginningProcesses
{
    Task SelectBeginningAsync(int player, DeckRegularDict<RegularSimpleCard> selectList, DeckRegularDict<RegularSimpleCard> unselectList);
}