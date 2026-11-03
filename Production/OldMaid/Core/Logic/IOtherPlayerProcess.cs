namespace OldMaid.Core.Logic;
public interface IOtherPlayerProcess
{
    Task SelectCardAsync(int deck);
    void SortOtherCards();
}