namespace Rook.Core.Logic;
public interface INestProcesses
{
    Task ProcessNestAsync(DeckRegularDict<RookCardInformation> list);
}