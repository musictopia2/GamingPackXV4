namespace SolitaireBoardGame.Blazor.Views;
public class SolitaireGameBoard : GridGameBoard<GameSpace>
{
    protected override bool CanAddControl(IBoardCollection<GameSpace> itemsSource, int row, int column)
    {
        if (column >= 3 && column <= 5)
        {
            return true;
        }
        if (row >= 3 && row <= 5)
        {
            return true;
        }
        return false;
    }
}