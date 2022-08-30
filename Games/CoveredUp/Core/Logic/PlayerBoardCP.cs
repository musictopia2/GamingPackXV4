namespace CoveredUp.Core.Logic;
public class PlayerBoardCP : GameBoardObservable<RegularSimpleCard>
{
    private enum EnumMatchCategory
    {
        None,
        Two, //this means top/bottom
        Four //this means top/bottom and 2 right next to each other.
    }
    private readonly CoveredUpGameContainer _gameContainer;
    private readonly CoveredUpVMData _model;
    private BasicList<int> _orders;
    private CoveredUpPlayerItem? _player;
    public PlayerBoardCP(CoveredUpGameContainer gameContainer, CoveredUpVMData model) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
        _model = model;
        Rows = 2;
        Columns = 4;
        Text = "Your Cards";
        _orders = new()
        {
            4,
            0,
            5,
            1,
            6,
            2,
            7,
            3
        };
    }
    protected override async Task ClickProcessAsync(RegularSimpleCard card)
    {
        if (_gameContainer.PileClickedAsync is null)
        {
            throw new CustomBasicException("Nobody handled the pile clicked"); //you can't choose another players pile anyways.
        }
        await _gameContainer.PileClickedAsync.Invoke(card);
    }
    public void LoadBoard(CoveredUpPlayerItem player)
    {
        _player = player;
        Text = _player.NickName;
        ObjectList.ReplaceRange(player.MainHandList);
    }
    protected override bool CanExecute(RegularSimpleCard card)
    {
        if (_model.OtherPile!.PileEmpty() == true)
        {
            return false;
        }
        if (_gameContainer.SaveRoot.UpTo == 7)
        {
            return true;
        }
        int index = ObjectList.IndexOf(card);
        int takes = _gameContainer.SaveRoot.UpTo + 2;
        var temps = _orders.Take(takes);
        return temps.Contains(index);
    }
    //could eventually decide to put this into global as well.  since its a common requirement.
    private int _tempDeck = -1;
    public void TradeCard(int oldDeck, int newDeck)
    {
        RegularSimpleCard thisCard = new();
        thisCard.Populate(newDeck);
        thisCard.IsUnknown = false;
        if (ObjectList.ObjectExist(thisCard.Deck))
        {
            var card = ObjectList.GetSpecificItem(thisCard.Deck);
            ObjectList.ReplaceDictionary(thisCard.Deck, _tempDeck, card);
            card.Deck = _tempDeck; //try this too.
            _tempDeck--;
            if (ObjectList.ObjectExist(thisCard.Deck))
            {
                throw new CustomBasicException("Did not replace with new temporary value.  Rethink");
            }
        }
        TradeObject(oldDeck, thisCard);
    }
    private void PointsSoFar()
    {
        _player!.PointsSoFar = TotalPointsInRound();
    }
    private EnumMatchCategory GetMatchCategory(int column)
    {
        RegularSimpleCard topCard = GetObject(1, column);
        RegularSimpleCard bottomCard = GetObject(2, column);
        if (topCard.IsUnknown || bottomCard.IsUnknown)
        {
            return EnumMatchCategory.None; //has to be known first though.
        }    
        if (topCard.Value != bottomCard.Value)
        {
            return EnumMatchCategory.None;
        }
        if (column == 4)
        {
            //can never be double.
            return EnumMatchCategory.Two;
        }
        RegularSimpleCard otherTops = GetObject(1, column + 1);
        RegularSimpleCard otherBottoms = GetObject(2, column + 1);
        if (otherTops.IsUnknown || otherBottoms.IsUnknown)
        {
            return EnumMatchCategory.Two; //can't be four because something is not known.
        }
        if (otherTops.Value == otherBottoms.Value && topCard.Value == otherTops.Value)
        {
            return EnumMatchCategory.Four;
        }
        return EnumMatchCategory.Two;
    }
    public int TotalPointsInRound()
    {
        //now needs to calculate points so far.
        EnumMatchCategory category1 = GetMatchCategory(1);
        int output;
        if (category1 == EnumMatchCategory.None)
        {
            output = Category1NonePoints();
            return output;
        }
        int score;
        BasicList<int> points = new();
        if (category1 == EnumMatchCategory.Two)
        {
            score = MatchSinglePoints(1);
            points.Add(score);
            score = Category2Points();
            points.Add(score);
        }
        else if (category1 == EnumMatchCategory.Four)
        {
            score = MatchDoublePoints(1);
            points.Add(score);
            score = Category3Points();
            points.Add(score);
        }
        return points.Sum();
    }
    private int MatchSinglePoints(int column)
    {
        RegularSimpleCard otherTops = GetObject(1, column);
        if (otherTops.Value == EnumRegularCardValueList.Joker)
        {
            return -10; //i think
        }
        return 0;
    }
    private int MatchDoublePoints(int column)
    {
        //the first column in a double.
        RegularSimpleCard otherTops = GetObject(1, column);
        if (otherTops.Value == EnumRegularCardValueList.Joker)
        {
            return -40;
        }
        return -20;
    }
    private int Category2Points()
    {
        EnumMatchCategory category2;
        category2 = GetMatchCategory(2);
        int score;
        BasicList<int> points = new();
        if (category2 == EnumMatchCategory.None)
        {
            score = ColumnPoints(2);
            points.Add(score);
            score = Category3Points();
            points.Add(score);
        }
        else if (category2 == EnumMatchCategory.Two)
        {
            score = MatchSinglePoints(2);
            points.Add(score);
            score = Category3Points();
            points.Add(score);
        }
        else if (category2 == EnumMatchCategory.Four)
        {
            score = MatchDoublePoints(2);
            points.Add(score);
            score = Category4Points();
            points.Add(score); //can't consider number 3 because it was double (2 and 3).
        }
        return points.Sum();
    }
    private int Category1NonePoints()
    {
        BasicList<int> points = new();
        int score = ColumnPoints(1);
        points.Add(score);
        score = Category2Points();
        points.Add(score);
        return points.Sum();
    }
    private int Category3Points()
    {
        EnumMatchCategory category3;
        int score;
        BasicList<int> points = new();
        category3 = GetMatchCategory(3);
        if (category3 == EnumMatchCategory.None)
        {
            score = ColumnPoints(3);
            points.Add(score);
            score = Category4Points();
            points.Add(score);
        }
        else if (category3 == EnumMatchCategory.Two)
        {
            //this means you have to do the category 4 stuff.
            score = MatchSinglePoints(3);
            points.Add(score);
            score = Category4Points();
            points.Add(score);
        }
        else if (category3 == EnumMatchCategory.Four)
        {
            score = MatchDoublePoints(3);
            points.Add(score); //hopefully this simple (?)
        }
        return points.Sum();
    }
    private int Category4Points()
    {
        EnumMatchCategory category4;
        category4 = GetMatchCategory(4);
        int output;
        if (category4 == EnumMatchCategory.None)
        {
            output = ColumnPoints(4);
        }
        else
        {
            output = MatchSinglePoints(4);
        }
        return output;
    }
    private int ColumnPoints(int column)
    {
        RegularSimpleCard topCard = GetObject(1, column);
        RegularSimpleCard bottomCard = GetObject(2, column);
        int output = topCard.Points() + bottomCard.Points();
        return output;
    }
    public void EndTurn()
    {
        if (_gameContainer.SaveRoot.UpTo == 6)
        {
            if (_gameContainer.SaveRoot.WentOut)
            {
                ObjectList.MakeAllObjectsKnown(); //needs this before getting points so far as well.
            }
            PointsSoFar();
            return;
        }
        
        int takes = _gameContainer.SaveRoot.UpTo + 2;
        var temps = _orders.Take(takes);
        var index = temps.Last();
        ObjectList[index].IsUnknown = false; //period.
        PointsSoFar(); //has to do again.
    }
    public bool FirstWentOut()
    {
        if (_gameContainer.SaveRoot.WentOut)
        {
            return false; //because somebody already went out.
        }
        if (_gameContainer.SaveRoot.UpTo < 6)
        {
            return false;
        }
        return ObjectList.All(x => x.IsUnknown == false);
    }
}