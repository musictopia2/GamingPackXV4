namespace SolitaireBoardGame.Blazor.Views;
public partial class SolitaireBoardGameMainView
{
    private static SolitaireBoardGameCollection GetSpaceList()
    {
        SolitaireBoardGameSaveInfo thisSave = aa1.Resolver!.Resolve<SolitaireBoardGameSaveInfo>();
        return thisSave.SpaceList;
    }
}