namespace PickelCardGame.Core.Data;
[SingletonGame]
public class PickelCardGameDetailClass : IGameInfo, ICardInfo<PickelCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<PickelCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Pickel Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<PickelCardGameCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<PickelCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<PickelCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<PickelCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<PickelCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<PickelCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<PickelCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<PickelCardGameCardInformation>.NoPass => false;
    bool ICardInfo<PickelCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<PickelCardGameCardInformation> ICardInfo<PickelCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<PickelCardGameCardInformation>();
    bool ICardInfo<PickelCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<PickelCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<PickelCardGameCardInformation>.DiscardExcludeList(IListShuffler<PickelCardGameCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<PickelCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<PickelCardGameCardInformation>.CustomDeck => true;
}