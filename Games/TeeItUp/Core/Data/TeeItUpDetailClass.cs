namespace TeeItUp.Core.Data;
[SingletonGame]
public class TeeItUpDetailClass : IGameInfo, ICardInfo<TeeItUpCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Tee It Up";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<TeeItUpCardInformation>.CardsToPassOut => 8;
    BasicList<int> ICardInfo<TeeItUpCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<TeeItUpCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<TeeItUpCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<TeeItUpCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<TeeItUpCardInformation>.PassOutAll => false;
    bool ICardInfo<TeeItUpCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<TeeItUpCardInformation>.NoPass => false;
    bool ICardInfo<TeeItUpCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<TeeItUpCardInformation> ICardInfo<TeeItUpCardInformation>.DummyHand { get; set; } = new DeckRegularDict<TeeItUpCardInformation>();
    bool ICardInfo<TeeItUpCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<TeeItUpCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<TeeItUpCardInformation>.DiscardExcludeList(IListShuffler<TeeItUpCardInformation> deckList)
    {
        return new();
    }
}