namespace Uno.Core.Data;
[SingletonGame]
public class UnoSaveInfo : BasicSavedCardClass<UnoPlayerItem, UnoCardInformation>, IMappable, ISaveInfo
{
    public bool HasDrawn { get; set; }
    public bool HasSkipped { get; set; }
    public EnumGameStatus GameStatus { get; set; } = EnumGameStatus.NormalPlay;
    public EnumColorTypes CurrentColor { get; set; }
}