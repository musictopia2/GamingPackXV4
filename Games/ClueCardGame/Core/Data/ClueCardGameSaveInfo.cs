namespace ClueCardGame.Core.Data;
[SingletonGame]
public class ClueCardGameSaveInfo : BasicSavedCardClass<ClueCardGamePlayerItem, ClueCardGameCardInformation>, IMappable, ISaveInfo
{
    public PredictionInfo? CurrentPrediction { get; set; }
    internal void LoadMod(ClueCardGameVMData model)
    {
        if (CurrentPrediction is not null)
        {
            model.CurrentCharacterName = CurrentPrediction.CharacterName;
            model.CurrentWeaponName = CurrentPrediction.WeaponName;
            model.CurrentRoomName = CurrentPrediction.RoomName;
        }
    }
    public bool AccusationMade { get; set; }
    public PredictionInfo Solution { get; set; } = new();
    public int PreviousClue { get; set; } //this means if given, then whoevers turn it is can show that clue.
    public EnumClueStatusList GameStatus { get; set; }
}