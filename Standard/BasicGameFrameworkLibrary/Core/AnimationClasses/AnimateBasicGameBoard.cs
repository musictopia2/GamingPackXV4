namespace BasicGameFrameworkLibrary.Core.AnimationClasses;
public class AnimateBasicGameBoard
{
    readonly IEventAggregator _thisE;
    public PointF LocationFrom { get; set; }
    public PointF LocationTo { get; set; }
    public PointF CurrentLocation { get; set; }
    public bool FastAnimation { get; set; } //mexican train dominos will use the option.  the game has to decide whether to enable this.
    public bool AnimationGoing { get; set; } // so the gameboard knows whether it needs something special or not.
    private float _destinationX;
    private float _destinationY;
    private float _startX;
    private float _startY;
    private int _totalSteps;
    public int LongestTravelTime { get; set; }
    private const int _interval = 20;
    public AnimateBasicGameBoard(IEventAggregator thisE)
    {
        _thisE = thisE;
    }
    private bool _xup = false;
    private bool _yup = false;
    public async Task DoAnimateAsync()
    {
        _startX = LocationFrom.X; 
        _startY = LocationFrom.Y;
        _destinationX = LocationTo.X;
        _destinationY = LocationTo.Y;
        _totalSteps = LongestTravelTime / _interval;
        CurrentLocation = LocationFrom;
        _xup = LocationTo.X > LocationFrom.X;
        _yup = LocationTo.Y > LocationFrom.Y;
        AnimationGoing = true; // so when they access the information, they will do something different.
        _thisE.RepaintMessage(EnumRepaintCategories.Main);
        float totalXDistance;
        float totalYDistance;
        float eachx = 0;
        float eachy = 0;
        int x;
        float upTox = 0;
        float upToy = 0;
        if (FastAnimation)
        {
            _totalSteps = 2;
        }
        await Task.Run(() =>
        {
            totalXDistance = _destinationX - _startX;
            totalYDistance = _destinationY - _startY;
            eachx = totalXDistance / _totalSteps;
            eachy = totalYDistance / _totalSteps;
            upTox = _startX;
            upToy = _startY;
        });
        int loopTo = _totalSteps;
        if (FastAnimation)
        {
            loopTo = 1;
        }
        for (x = 1; x <= loopTo; x++)
        {
            upTox += eachx;
            upToy += eachy;
            if (_xup && upTox > LocationTo.X)
            {
                upTox = LocationTo.X;
            }
            else if (_xup == false && upTox < LocationTo.X)
            {
                upTox = LocationTo.X;
            }
            if (_yup && upToy > LocationTo.Y)
            {
                upToy = LocationTo.Y;
            }
            else if (_yup == false && upToy < LocationTo.Y)
            {
                upToy = LocationTo.Y;
            }
            CurrentLocation = new PointF(upTox, upToy);
            _thisE.RepaintMessage(EnumRepaintCategories.Main);
            if (FastAnimation)
            {
                await Task.Delay(5);
            }
            else
            {
                await Task.Delay(_interval);
            }
        }
        if (FastAnimation)
        {
            CurrentLocation = LocationTo;
            _thisE.RepaintMessage(EnumRepaintCategories.Main);
            await Task.Delay(5);
        }
        AnimationGoing = false;
        RepaintEventModel.UpdatePartOfBoard = null;
    }
}