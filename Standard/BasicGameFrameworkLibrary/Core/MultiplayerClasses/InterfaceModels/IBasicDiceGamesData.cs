namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;
public interface IBasicDiceGamesData<D> : IViewModelData, ICup<D>
    where D : IStandardDice, new()
{
    //needs to be static unfortunately now.
    //if this causes problems, then i can always change the ui part.  since the label helpers are part of the game package, hopefully can look to the variable as well (well see what happens)
    //have here just in case.
    public static bool? NeedsRollIncrement { get; set; } = null;
    int RollNumber { get; set; }
    void LoadCup(ISavedDiceList<D> saveRoot, bool autoResume);
}