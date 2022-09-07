namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;
public interface IBasicDiceGamesData<D> : IViewModelData, ICup<D>
    where D : IStandardDice, new()
{
    public static bool? NeedsRollIncrement { get; set; } = null;
    int RollNumber { get; set; }
    void LoadCup(ISavedDiceList<D> saveRoot, bool autoResume);
}