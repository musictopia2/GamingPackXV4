namespace DealCardGame.Blazor;
public partial class TestCardComponent
{
    private int _index = 21;
    private DealCardGameCardInformation _card = new();
    private void AnotherOne()
    {
        if (_index <= 47)
        {
            _index++;
            _card = new();
            _card.Populate(_index);
        }
    }
    protected override void OnInitialized()
    {
        _card.Populate(_index);
        base.OnInitialized();
    }
}