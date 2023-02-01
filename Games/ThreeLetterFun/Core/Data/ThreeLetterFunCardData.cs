namespace ThreeLetterFun.Core.Data;
public class ThreeLetterFunCardData : SimpleDeckObject, IDeckObject, IAdvancedDIContainer
{
    public IGamePackageResolver? MainContainer { get; set; } //hopefully it will ignore because its a resolver (if not, can put in there as well).
    private ThreeLetterFunDeckInfo? _deckInfo;
    private GlobalHelpers? _thisGlobal;
    private ThreeLetterFunMainGameClass? _mainGame; //iffy.
    private BasicList<char> _completeList = new();
    private BasicList<char> _charList = new();
    public BasicList<char> CharList
    {
        get { return _charList; }
        set
        {
            if (SetProperty(ref _charList, value))
            {
                _completeList = CharList.ToBasicList();
            }
        }
    }
    public BasicList<char> GetCompleteList => _completeList.ToBasicList();
    public EnumClickPosition ClickLocation { get; set; } = EnumClickPosition.None;
    public void ReloadSaved()
    {
        _completeList = CharList.ToBasicList();
    }
    public bool CompletedWord()
    {
        if (_completeList.Exists(x => vb1.AscW(x) == 0) == true)
        {
            return false;
        }
        return true;
    }
    private void SetObjects()
    {
        if (_deckInfo != null)
        {
            return;
        }
        aa1.PopulateContainer(this);
        _deckInfo = MainContainer!.Resolve<ThreeLetterFunDeckInfo>(); //need the full one.
        _thisGlobal = MainContainer.Resolve<GlobalHelpers>();
        _mainGame = MainContainer.Resolve<ThreeLetterFunMainGameClass>();
    }
    public ThreeLetterFunCardData CloneCard()
    {
        ThreeLetterFunCardData output = (ThreeLetterFunCardData)MemberwiseClone();
        output._completeList = _completeList.ToBasicList();
        output.CharList = CharList.ToBasicList(); //i think.
        return output;
    }
    public BasicList<TileInformation> Tiles { get; set; } = new();
    public int HiddenValue { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public void ClearTiles()
    {
        _completeList = CharList.ToBasicList();
        Tiles.Clear();
        HiddenValue += 1; // so it can repaint (?)
    }
    public string GetWord()
    {
        if (CompletedWord() == false)
        {
            throw new CustomBasicException("Has to complete a word before it can get the word");
        }
        return _completeList.First().ToString().ToLower() + _completeList[1].ToString().ToLower() + _completeList.Last().ToString().ToLower();
    }
    public int GetLetterPosition(EnumClickPosition whichCategory)
    {
        if (whichCategory == EnumClickPosition.None)
        {
            throw new CustomBasicException("Must specify left or right");
        }
        int x = default;
        int y = default;
        foreach (var thisItem in _completeList)
        {
            if (vb1.AscW(thisItem) == 0)
            {
                if (whichCategory == EnumClickPosition.Left)
                {
                    return x;
                }
                else
                {
                    y += 1;
                }
                if (y > 1)
                {
                    return x;
                }
            }
            x += 1;
        }
        return -1;
    }
    public int LetterRemaining()
    {
        var thisItem = (from items in _completeList
                        where vb1.AscW(items) == 0
                        select items).Single();
        return _completeList.IndexOf(thisItem);
    }
    public void AddLetter(int tileDeck, int index)
    {
        if (_completeList.Count == 0)
        {
            throw new CustomBasicException("No Complete List");
        }
        if (vb1.AscW(_completeList[index]) > 0)
        {
            var thisChar = _completeList[index];
            Tiles.RemoveAllOnly(x => x.Letter == thisChar);
        }
        if (_thisGlobal == null)
        {
            SetObjects();
        }
        var thisTile = _thisGlobal!.GetTile(tileDeck);
        thisTile.Index = index;
        Tiles.Add(thisTile);
        _completeList[index] = thisTile.Letter;
    }
    public bool IsValidWord()
    {
        if (_mainGame!.SaveRoot!.Level == EnumLevel.None)
        {
            throw new CustomBasicException("Needs to choose a level before figure out whether its a valid word or not");
        }
        var thisWord = GetWord();
        if (_mainGame.SaveRoot.Level == EnumLevel.Hard)
        {
            return _thisGlobal!.SavedWords!.Any(xx => xx.Word == thisWord);
        }
        return _thisGlobal!.SavedWords!.Any(xx => xx.Word == thisWord && xx.IsEasy == true);
    }
    public ThreeLetterFunCardData()
    {
        DefaultSize = new SizeF(69, 36);
    }
    public void Populate(int chosen)
    {
        if (chosen == 0)
        {
            throw new CustomBasicException("Deck cannot be 0 for this game");
        }
        SetObjects();
        if (_deckInfo!.PrivateSavedList.Count == 0)
        {
            _deckInfo.InitCards();
        }
        Deck = chosen;
        if (Deck > _deckInfo.PrivateSavedList.Count)
        {
            throw new CustomBasicException($"{Deck} not found");
        }
        CharList = _deckInfo.PrivateSavedList[Deck - 1].CharacterList.ToBasicList();
    }
    public void Reset() { }
}