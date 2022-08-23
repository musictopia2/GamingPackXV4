namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public class BasicSavedDominosClass<D, P> : BasicSavedGameClass<P>
    where D : IDominoInfo, new()
    where P : class, IPlayerSingleHand<D>, new()
{
    public SavedScatteringPieces<D>? BoneYardData { get; set; }
}