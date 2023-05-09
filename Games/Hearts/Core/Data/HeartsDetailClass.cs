namespace Hearts.Core.Data;
[SingletonGame]
public class HeartsDetailClass : IGameInfo, ICardInfo<HeartsCardInformation>, ITrickData,
    IBeginningRegularCards<HeartsCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => true; //i think.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //the computer can't skip turns. otherwise, gets stuck.
    string IGameInfo.GameName => "Hearts";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 4;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    int ICardInfo<HeartsCardInformation>.CardsToPassOut => 13; //change to what you need.
    BasicList<int> ICardInfo<HeartsCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<HeartsCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HeartsCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HeartsCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HeartsCardInformation>.PassOutAll => false;
    bool ICardInfo<HeartsCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HeartsCardInformation>.NoPass => false;
    bool ICardInfo<HeartsCardInformation>.NeedsDummyHand => false; //no dummy hand.
    DeckRegularDict<HeartsCardInformation> ICardInfo<HeartsCardInformation>.DummyHand { get; set; } = new DeckRegularDict<HeartsCardInformation>();
    BasicList<int> ICardInfo<HeartsCardInformation>.DiscardExcludeList(IListShuffler<HeartsCardInformation> deckList)
    {
        return new();
    }
    bool ICardInfo<HeartsCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<HeartsCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.Hearts;
    bool ITrickData.HasDummy => false;
    bool IBeginningRegularCards<HeartsCardInformation>.AceLow => false;
    bool IBeginningRegularCards<HeartsCardInformation>.CustomDeck => false;
}