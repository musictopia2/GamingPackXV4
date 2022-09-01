namespace SkuckCardGame.Core.Data;
[SingletonGame]
public class SkuckCardGameDetailClass : IGameInfo, ICardInfo<SkuckCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<SkuckCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Skuck Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<SkuckCardGameCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<SkuckCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<SkuckCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<SkuckCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<SkuckCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<SkuckCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<SkuckCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<SkuckCardGameCardInformation>.NoPass => false;
    bool ICardInfo<SkuckCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<SkuckCardGameCardInformation> ICardInfo<SkuckCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<SkuckCardGameCardInformation>();
    bool ICardInfo<SkuckCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<SkuckCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => true;
    BasicList<int> ICardInfo<SkuckCardGameCardInformation>.DiscardExcludeList(IListShuffler<SkuckCardGameCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<SkuckCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<SkuckCardGameCardInformation>.CustomDeck => false;
}