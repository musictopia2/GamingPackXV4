namespace RoundsCardGame.Core.Data;
[SingletonGame]
public class RoundsCardGameDetailClass : IGameInfo, ICardInfo<RoundsCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<RoundsCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Rounds Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<RoundsCardGameCardInformation>.CardsToPassOut => 9;
    BasicList<int> ICardInfo<RoundsCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<RoundsCardGameCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<RoundsCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RoundsCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RoundsCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<RoundsCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<RoundsCardGameCardInformation>.NoPass => false;
    bool ICardInfo<RoundsCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<RoundsCardGameCardInformation> ICardInfo<RoundsCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<RoundsCardGameCardInformation>();
    bool ICardInfo<RoundsCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<RoundsCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<RoundsCardGameCardInformation>.DiscardExcludeList(IListShuffler<RoundsCardGameCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<RoundsCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<RoundsCardGameCardInformation>.CustomDeck => false;
}