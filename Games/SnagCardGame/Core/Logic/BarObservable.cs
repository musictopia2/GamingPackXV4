﻿namespace SnagCardGame.Core.Logic;
public class BarObservable : HandObservable<SnagCardGameCardInformation>
{
    private readonly SnagCardGameGameContainer _gameContainer;
    private bool _wasSaved;
    public BarObservable(SnagCardGameGameContainer gameContainer) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
        Text = "Bar";
    }
    protected override bool CanSelectSingleObject(SnagCardGameCardInformation thisObject)
    {
        if (thisObject.Equals(HandList.Last()))
        {
            return true;
        }
        if (HandList.Count == 1)
        {
            return false;
        }
        int index = HandList.Count - 2;
        return thisObject.Equals(HandList[index]);
    }
    private bool NeedsReverse()
    {
        if (_gameContainer.BasicData!.MultiPlayer == false && _wasSaved == false)
        {
            return true;
        }
        if (_gameContainer.BasicData.MultiPlayer == false)
        {
            return false;
        }
        if (_gameContainer.BasicData.Client == false && _wasSaved == false)
        {
            return true;
        }
        if (_gameContainer.BasicData.Client == true && _wasSaved == true)
        {
            return true;
        }
        return false;
    }
    public void LoadBarCards(IDeckDict<SnagCardGameCardInformation> thisList)
    {
        if (_wasSaved == false && thisList.Count != 5)
        {
            throw new CustomBasicException("Must have 5 cards for the bar.");
        }
        if (_wasSaved == true && thisList.Count > 5)
        {
            throw new CustomBasicException("The bar can never have more than 5 cards");
        }
        bool rets = NeedsReverse();
        if (rets == true)
        {
            thisList.Reverse();
        }
        DeckRegularDict<SnagCardGameCardInformation> tempList = new();
        thisList.ForEach(thisCard =>
        {
            var newCard = new SnagCardGameCardInformation();
            newCard.Populate(thisCard.Deck);
            newCard.Rotated = true;
            tempList.Add(newCard);
        });
        HandList.ReplaceRange(tempList);
    }
    public void LoadSavedGame()
    {
        _wasSaved = true;
        LoadBarCards(_gameContainer.SaveRoot!.BarList);
    }
    public void Clear()
    {
        _wasSaved = false;
    }
    public DeckRegularDict<SnagCardGameCardInformation> PossibleList
    {
        get
        {
            if (HandList.Count <= 2)
            {
                return HandList.ToRegularDeckDict();
            }
            return new DeckRegularDict<SnagCardGameCardInformation>() { HandList.First(), HandList[1] };
        }
    }
}