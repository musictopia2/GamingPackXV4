namespace Hearts.Core.Logic;
[SingletonGame]
public class HeartsTrickAreaCP(TrickGameContainer<HeartsCardInformation, HeartsPlayerItem, HeartsSaveInfo, EnumSuitList> gameContainer) : SeveralPlayersTrickObservable<EnumSuitList, HeartsCardInformation, HeartsPlayerItem, HeartsSaveInfo>(gameContainer)
{
    
}