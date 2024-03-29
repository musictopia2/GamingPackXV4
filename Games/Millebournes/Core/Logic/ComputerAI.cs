﻿namespace Millebournes.Core.Logic;
[SingletonGame]
[AutoReset]
public class ComputerAI
{
    public struct MoveInfo
    {
        public int Team; // this is the team the card will be played on (if any)
        public bool WillThrowAway;
        public int Deck; // this is the card being played
        public EnumPileType WhichPile;
    }
    private DeckRegularDict<MillebournesCardInformation> _extendedList = new();

    private readonly MillebournesVMData _model;
    private readonly MillebournesGameContainer _gameContainer;
    public ComputerAI(MillebournesVMData model, MillebournesGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    private void GenerateExtendedList()
    {
        _extendedList = new DeckRegularDict<MillebournesCardInformation>();
        foreach (var thisCard in _gameContainer.SingleInfo!.MainHandList)
        {
            _extendedList.Add(thisCard);
        }
        if (_model.Pile1!.PileEmpty() == false)
        {
            _extendedList.Add(_model.Pile1.GetCardInfo());
        }
    }
    private DeckRegularDict<MillebournesCardInformation> PossibleMoves()
    {
        DeckRegularDict<MillebournesCardInformation> output = new();
        _extendedList.ForEach(thisCard =>
        {
            _gameContainer.CurrentCP!.CurrentCard = thisCard;
            if (AcceptMove)
            {
                output.Add(thisCard);
            }
        });
        return output;
    }
    private bool AcceptMove
    {
        get
        {
            return _gameContainer!.CurrentCP!.CurrentCard!.CardType switch
            {
                EnumCardCategories.Miles => _gameContainer.CurrentCP.CanPlaceMiles(out _),
                EnumCardCategories.Speed => _gameContainer.TeamList.Any(items => items.HasSpeedLimit == false && items.TeamNumber != _gameContainer.SaveRoot!.CurrentTeam),
                EnumCardCategories.Hazard => _gameContainer.TeamList.Any(items => items.WhichHazard == EnumHazardType.None && items.TeamNumber != _gameContainer.SaveRoot!.CurrentTeam),
                EnumCardCategories.Remedy => _gameContainer.CurrentCP.CanFixHazard(out _),
                EnumCardCategories.EndLimit => _gameContainer.CurrentCP.CanEndSpeedLimit(out _),
                EnumCardCategories.Safety => _gameContainer.CurrentCP.CanPlaceSafety(out _),
                _ => throw new CustomBasicException("Cannot find out whether the move was acceptable based on the card type"),
            };
        }
    }
    private static int MilesPlaced(IDeckDict<MillebournesCardInformation> moveList)
    {
        var thisCard = moveList.Where(items => items.CardType == EnumCardCategories.Miles).OrderByDescending(items => items.Mileage).FirstOrDefault();
        if (thisCard == null)
        {
            return 0;
        }
        return thisCard.Deck;
    }
    private bool IsRoundAlmostOver(int previous, IDeckDict<MillebournesCardInformation> playList)
    {
        if (_model.Deck1!.IsEndOfDeck() == true)
        {
            return true;
        }
        int manys = playList.Count(items => items.Mileage == 200);
        int safes = _gameContainer.TeamList.Sum(items => items.NumberOfSafeties) + previous;
        foreach (var thisTeam in _gameContainer.TeamList)
        {
            if (thisTeam.TeamNumber != _gameContainer.SaveRoot!.CurrentTeam && _gameContainer.PlayerList!.Count <= 3 || _gameContainer.PlayerList!.Count > 3)
            {
                if (thisTeam.HasSpeedLimit == false && thisTeam.Miles == 800 && manys < 4 && thisTeam.WhichHazard == EnumHazardType.None && safes < 3)
                {
                    return true;
                }
                if (thisTeam.HasSpeedLimit == false && thisTeam.Miles >= 900 && manys < 4 && thisTeam.WhichHazard == EnumHazardType.None && safes < 3)
                {
                    return true;
                }
                if (thisTeam.Miles >= 950 && thisTeam.WhichHazard == EnumHazardType.None && safes < 3)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private static DeckRegularDict<MillebournesCardInformation> HazardList(IDeckDict<MillebournesCardInformation> moveList)
    {
        DeckRegularDict<MillebournesCardInformation> output = new();
        output.AddRange(moveList);
        output.KeepConditionalItems(items => items.CardType == EnumCardCategories.Hazard || items.CardType == EnumCardCategories.Speed);
        return output;
    }
    private DeckRegularDict<MillebournesCardInformation> GetPlayList()
    {
        DeckRegularDict<MillebournesCardInformation> output = new();
        _gameContainer.TeamList.ForEach(thisTeam =>
        {
            output.AddRange(thisTeam.CardsPlayed);
        });
        output.AddRange(_model.Pile2!.DiscardList());
        if (_model.Pile2.PileEmpty() == false)
        {
            output.Add(_model.Pile2.GetCardInfo());
        }
        output.AddRange(_extendedList);
        return output;
    }
    private static int ThrowAwayMiles(IDeckDict<MillebournesCardInformation> throwList, bool hadSpeed)
    {
        var temps = throwList.OrderBy(items => items.Mileage).ToRegularDeckDict();
        if (hadSpeed == true)
        {
            temps.KeepConditionalItems(items => items.CardType == EnumCardCategories.Miles && items.Mileage <= 50);
        }
        else
        {
            temps.KeepConditionalItems(items => items.CardType == EnumCardCategories.Miles);
        }
        if (temps.Count == 0)
        {
            return 0;
        }
        return temps.First().Deck;
    }
    private bool DupCards(MillebournesCardInformation thisCard)
    {
        if (thisCard.CardType != EnumCardCategories.EndLimit && thisCard.CardType != EnumCardCategories.Remedy)
        {
            return false;
        }
        return _extendedList.Count(items => items.CardName == thisCard.CardName) > 1;
    }
    private MoveInfo ComputerThrow()
    {
        MoveInfo output = new();
        output.WillThrowAway = true;
        int milesNeeded = 1000 - _gameContainer.CurrentCP!.Miles;
        DeckRegularDict<MillebournesCardInformation> throwList = _extendedList.Where(items => items.CardType != EnumCardCategories.Safety).ToRegularDeckDict();
        var playList = GetPlayList();
        foreach (var thisCard in throwList)
        {
            if (thisCard.CardType == EnumCardCategories.Miles && thisCard.Mileage == 200 && _gameContainer.CurrentCP.HowMany200S >= 2)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardType == EnumCardCategories.Miles && thisCard.Mileage > milesNeeded)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Gasoline" && _gameContainer.CurrentCP.SafetyHas("Extra Tank"))
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Repairs" && _gameContainer.CurrentCP.SafetyHas("Driving Ace"))
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Spare Tire" && _gameContainer.CurrentCP.SafetyHas("Puncture Proof"))
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Roll" && _gameContainer.CurrentCP.SafetyHas("Right Of Way"))
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "End Of Limit" && _gameContainer.CurrentCP.SafetyHas("Right Of Way"))
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Roll" && playList.Count(items => items.CardName == "Stop") > 5)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Gasoline" && playList.Count(items => items.CardName == "Out Of Gas") > 2)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Spare Tire" && playList.Count(items => items.CardName == "Flat Tire") > 2)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "Repairs" && playList.Count(items => items.CardName == "Accident") > 2)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
            if (thisCard.CardName == "End Of Limit" && playList.Count(items => items.CardName == "Speed Limit") > 3)
            {
                output.Deck = thisCard.Deck;
                return output;
            }
        }
        foreach (var thisCard in throwList)
        {
            if (DupCards(thisCard))
            {
                output.Deck = thisCard.Deck;
                return output;
            }
        }
        int decks = ThrowAwayMiles(throwList, _gameContainer.CurrentCP.HasSpeedLimit);
        if (decks > 0)
        {
            output.Deck = decks;
            return output;
        }
        output.Deck = throwList.GetRandomItem().Deck;
        return output;
    }
    public bool CanGiveOne { get; private set; }
    private MoveInfo BestHazard(IDeckDict<MillebournesCardInformation> thisList)
    {
        BasicList<MoveInfo> possibleList = new();
        MoveInfo thisMove;
        thisList.ForEach(thisCard =>
        {
            _gameContainer.TeamList.ForEach(thisTeam =>
            {
                thisTeam.CurrentCard = thisCard;
                if (thisCard.CardType == EnumCardCategories.Speed)
                {
                    if (thisTeam.CanGiveSpeedLimit(out _))
                    {
                        thisMove = new ();
                        thisMove.WhichPile = EnumPileType.Speed;
                        thisMove.Team = thisTeam.TeamNumber;
                        thisMove.Deck = thisCard.Deck;
                        possibleList.Add(thisMove);
                    }
                }
                else if (thisTeam.CanGiveHazard(out _))
                {
                    thisMove = new ();
                    thisMove.WhichPile = EnumPileType.Hazard;
                    thisMove.Team = thisTeam.TeamNumber;
                    thisMove.Deck = thisCard.Deck;
                    possibleList.Add(thisMove);
                }
            });
        });
        if (possibleList.Count == 0)
        {
            CanGiveOne = false;
            return new MoveInfo();
        }
        CanGiveOne = true;
        return possibleList.GetRandomItem();
    }
    private int SafetyForHazard()
    {
        DeckRegularDict<MillebournesCardInformation> whatList = _extendedList.Where(items => items.CardType == EnumCardCategories.Safety).ToRegularDeckDict();
        if (whatList.Count == 0)
        {
            return 0;
        }
        foreach (var thisCard in whatList)
        {
            if (thisCard.CardName == "Extra Tank" && _gameContainer.CurrentCP!.WhichHazard == EnumHazardType.OutOfGas)
            {
                return thisCard.Deck;
            }
            if (thisCard.CardName == "Driving Ace" && _gameContainer.CurrentCP!.WhichHazard == EnumHazardType.Accident)
            {
                return thisCard.Deck;
            }
            if (thisCard.CardName == "Puncture Proof" && _gameContainer.CurrentCP!.WhichHazard == EnumHazardType.FlatTire)
            {
                return thisCard.Deck;
            }
            if (thisCard.CardName == "Right Of Way" && (_gameContainer.CurrentCP!.HasSpeedLimit == true || _gameContainer.CurrentCP.WhichHazard == EnumHazardType.StopSign))
            {
                return thisCard.Deck;
            }
        }
        return 0;
    }
    private int WhatSafety()
    {
        DeckRegularDict<MillebournesCardInformation> whatList = _extendedList.Where(items => items.CardType == EnumCardCategories.Safety).ToRegularDeckDict();
        if (whatList.Count == 0)
        {
            return 0;
        }
        var playList = GetPlayList();
        if (IsRoundAlmostOver(whatList.Count, playList))
        {
            return whatList.GetRandomItem().Deck;
        }
        foreach (var thisCard in whatList)
        {
            if (thisCard.CardName == "Extra Tank" && playList.Count(items => items.CardName == "Out Of Gas") == 3)
            {
                return thisCard.Deck;
            }
            if (thisCard.CardName == "Driving Ace" && playList.Count(items => items.CardName == "Flat Tire") == 3)
            {
                return thisCard.Deck;
            }
            if (thisCard.CardName == "Puncture Proof" && playList.Count(items => items.CardName == "Accident") == 3)
            {
                return thisCard.Deck;
            }
            if (thisCard.CardName == "Right Of Way" && playList.Count(items => items.CardName == "Stop") == 6 && playList.Count(items => items.CardName == "Speed Limit") == 4)
            {
                return thisCard.Deck;
            }
        }
        return 0;
    }
    private BasicList<MoveInfo> RemedyList(IDeckDict<MillebournesCardInformation> moveList)
    {
        BasicList<MoveInfo> output = new();
        var temps = moveList.Where(items => items.CardType == EnumCardCategories.Remedy || items.CardType == EnumCardCategories.EndLimit).ToRegularDeckDict();
        temps.ForEach(thisCard =>
        {
            MoveInfo thisMove = new();
            thisMove.Deck = thisCard.Deck;
            if (thisCard.CardType == EnumCardCategories.Remedy)
            {
                thisMove.WhichPile = EnumPileType.Hazard;
            }
            else
            {
                thisMove.WhichPile = EnumPileType.Speed;
            }
            thisMove.Team = _gameContainer.SaveRoot!.CurrentTeam;
            output.Add(thisMove);
        });
        return output;
    }

    public MoveInfo ComputerMove()
    {
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        _gameContainer.CurrentCP = _gameContainer.FindTeam(_gameContainer.SingleInfo.Team);
        GenerateExtendedList();
        var moveList = PossibleMoves();
        if (moveList.Count == 0)
        {
            return ComputerThrow();
        }
        var whatHazards = HazardList(moveList);
        if (whatHazards.Count > 0)
        {
            MoveInfo newMove = BestHazard(whatHazards);
            if (CanGiveOne)
            {
                return newMove;
            }
        }
        int safes = WhatSafety();
        MoveInfo thisMove;
        if (safes > 0)
        {
            thisMove = new ();
            thisMove.Team = _gameContainer.SaveRoot!.CurrentTeam;
            thisMove.Deck = safes;
            thisMove.WhichPile = EnumPileType.Safety;
            return thisMove;
        }
        var newList = RemedyList(moveList);
        if (newList.Count > 0)
        {
            return newList.GetRandomItem();
        }
        safes = SafetyForHazard();
        if (safes > 0)
        {
            thisMove = new ();
            thisMove.Team = _gameContainer.SaveRoot!.CurrentTeam;
            thisMove.Deck = safes;
            thisMove.WhichPile = EnumPileType.Safety;
            return thisMove;
        }
        int miles = MilesPlaced(moveList);
        if (miles > 0)
        {
            thisMove = new ();
            thisMove.Team = _gameContainer.SaveRoot!.CurrentTeam;
            thisMove.Deck = miles;
            thisMove.WhichPile = EnumPileType.Miles;
            return thisMove;
        }
        return ComputerThrow();
    }
}