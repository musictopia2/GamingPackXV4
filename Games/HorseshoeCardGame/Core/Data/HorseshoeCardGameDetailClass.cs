namespace HorseshoeCardGame.Core.Data;
[SingletonGame]
public class HorseshoeCardGameDetailClass : IGameInfo, ICardInfo<HorseshoeCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<HorseshoeCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Horseshoe";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<HorseshoeCardGameCardInformation>.CardsToPassOut => 6;
    BasicList<int> ICardInfo<HorseshoeCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<HorseshoeCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HorseshoeCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HorseshoeCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HorseshoeCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<HorseshoeCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HorseshoeCardGameCardInformation>.NoPass => false;
    bool ICardInfo<HorseshoeCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<HorseshoeCardGameCardInformation> ICardInfo<HorseshoeCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<HorseshoeCardGameCardInformation>();
    bool ICardInfo<HorseshoeCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<HorseshoeCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => false;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => true;
    BasicList<int> ICardInfo<HorseshoeCardGameCardInformation>.DiscardExcludeList(IListShuffler<HorseshoeCardGameCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<HorseshoeCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<HorseshoeCardGameCardInformation>.CustomDeck => false;
}