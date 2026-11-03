namespace DominosRegular.Core.Data;
[SingletonGame]
public class DominosRegularSaveInfo : BasicSavedDominosClass<SimpleDominoInfo, DominosRegularPlayerItem>, IMappable, ISaveInfo
{
    public SimpleDominoInfo? CenterDomino { get; set; }
    public SimpleDominoInfo? FirstDomino { get; set; }
    public SimpleDominoInfo? SecondDomino { get; set; }
    public bool Beginnings { get; set; }
}