namespace Hearts.Core.Logic;
[SingletonGame]
public class HeartsTrickAreaCP : SeveralPlayersTrickObservable<EnumSuitList, HeartsCardInformation, HeartsPlayerItem, HeartsSaveInfo>
{
    public HeartsTrickAreaCP(TrickGameContainer<HeartsCardInformation, HeartsPlayerItem, HeartsSaveInfo, EnumSuitList> gameContainer) : base(gameContainer)
    {
    }
    //not sure what i need here though (hopefully becomes obvious) (?)

}