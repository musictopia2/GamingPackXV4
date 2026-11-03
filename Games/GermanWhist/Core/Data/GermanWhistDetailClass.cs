namespace GermanWhist.Core.Data;
[SingletonGame]
public class GermanWhistDetailClass : IGameInfo, ICardInfo<GermanWhistCardInformation>, ITrickData,
    IBeginningRegularCards<GermanWhistCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "German Whist";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<GermanWhistCardInformation>.CardsToPassOut => 13;
    BasicList<int> ICardInfo<GermanWhistCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<GermanWhistCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<GermanWhistCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<GermanWhistCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<GermanWhistCardInformation>.PassOutAll => false;
    bool ICardInfo<GermanWhistCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<GermanWhistCardInformation>.NoPass => false;
    bool ICardInfo<GermanWhistCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<GermanWhistCardInformation> ICardInfo<GermanWhistCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<GermanWhistCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<GermanWhistCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<GermanWhistCardInformation>.DiscardExcludeList(IListShuffler<GermanWhistCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<GermanWhistCardInformation>.AceLow => false;
    bool IBeginningRegularCards<GermanWhistCardInformation>.CustomDeck => false;
}