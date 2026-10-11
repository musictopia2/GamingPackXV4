namespace Xactika.Core.Logic;
[UseStateChanged]
public partial class ChooseShapeObservable : SimpleControlObservable
{
    private readonly XactikaGameContainer _gameContainer;
    public ChooseShapeObservable(XactikaGameContainer gameContainer) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
        CreateCommands();
    }
    partial void CreateCommands();
    private EnumShapes _shapeChosen = EnumShapes.None;
    public EnumShapes ShapeChosen
    {
        get
        {
            return _shapeChosen;
        }
        set
        {
            if (SetProperty(ref _shapeChosen, value) == true)
            {
                _gameContainer.SaveRoot!.ShapeChosen = value;
            }
        }
    }
    public int HowMany { get; set; } = 0;
    [StateProperty]
    public partial bool Visible { get; set; }
    public ControlCommand? ShapeSelectedCommand { get; set; }
    public BasicList<PieceModel> PieceList { get; set; } = [];
    [Command(EnumCommandCategory.Control)]
    private void ProcessPieceSelected(PieceModel piece)
    {
        PieceList.ForEach(tempPiece =>
        {
            if (tempPiece.Equals(piece))
            {
                tempPiece.IsSelected = true;
            }
            else
            {
                tempPiece.IsSelected = false;
            }
        });
        HowMany = piece.HowMany;
        ShapeChosen = piece.ShapeUsed;
    }
    protected override void EnableChange()
    {
        ShapeSelectedCommand!.ReportCanExecuteChange();
    }
    public void ChoosePiece(EnumShapes shape)
    {
        if (PieceList.Count > 0)
        {
            var piece = (from x in PieceList
                         where x.ShapeUsed.Value == shape.Value
                         select x).Single();
            piece.IsSelected = false;
            PieceList.ReplaceAllWithGivenItem(piece);
        }
        else
        {
            PieceModel newPiece = new();
            newPiece.ShapeUsed = shape;
            newPiece.HowMany = _gameContainer.SaveRoot!.Value;
            PieceList.ReplaceAllWithGivenItem(newPiece);
        }
        Visible = true;
        OnStateChanged();
    }
    public void Reset()
    {
        ShapeChosen = EnumShapes.None;
        HowMany = 0;
        _gameContainer.SaveRoot!.ShapeChosen = EnumShapes.None;
        Visible = false;
        OnStateChanged();
    }
    public void LoadPieceList(XactikaCardInformation card)
    {
        BasicList<PieceModel> tempList = [];
        PieceModel thisPiece = new();
        thisPiece.HowMany = card.HowManyBalls;
        thisPiece.ShapeUsed = EnumShapes.Balls;
        tempList.Add(thisPiece);
        thisPiece = new ();
        thisPiece.HowMany = card.HowManyCones;
        thisPiece.ShapeUsed = EnumShapes.Cones;
        tempList.Add(thisPiece);
        thisPiece = new ();
        thisPiece.HowMany = card.HowManyCubes;
        thisPiece.ShapeUsed = EnumShapes.Cubes;
        tempList.Add(thisPiece);
        thisPiece = new ();
        thisPiece.HowMany = card.HowManyStars;
        thisPiece.ShapeUsed = EnumShapes.Stars;
        tempList.Add(thisPiece);
        PieceList.ReplaceRange(tempList);
        Visible = true;
        OnStateChanged();
    }
}
