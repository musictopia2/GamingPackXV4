namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;

public interface IDiceBoardGamesData : ISimpleBoardGamesData, ICup<SimpleDice>
{
    void LoadCup(ISavedDiceList<SimpleDice> saveRoot, bool autoResume);
}