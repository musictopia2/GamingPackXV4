namespace SorryDicedGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SorryDicedGameVMData : ISimpleBoardGamesData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
}