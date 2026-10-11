namespace Xactika.Core.Logic;
[SingletonGame]
[AutoReset]
public class ShapeProcesses(XactikaVMData model, XactikaGameContainer gameContainer) : IShapeProcesses
{
    public async Task FirstCallShapeAsync()
    {
        model.ShapeChoose1.Visible = true;
        //if (gameContainer.LoadShapeButton == null)
        //{
        //    throw new CustomBasicException("Nobody is loading the shapes.  Rethink");
        //}
        gameContainer.LoadShapeButton?.Invoke(); //hopefully its this simple (?)
        var thisCard = gameContainer.SaveRoot!.TrickList.Single();
        model!.ShapeChoose1!.LoadPieceList(thisCard);
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task ShapeChosenAsync(EnumShapes shape)
    {
        var thisCard = gameContainer.SaveRoot!.TrickList.Single();
        gameContainer.SaveRoot.ShapeChosen = shape;
        if (shape == EnumShapes.Balls)
        {
            gameContainer.SaveRoot.Value = thisCard.HowManyBalls;
        }
        else if (shape == EnumShapes.Cubes)
        {
            gameContainer.SaveRoot.Value = thisCard.HowManyCubes;
        }
        else if (shape == EnumShapes.Cones)
        {
            gameContainer.SaveRoot.Value = thisCard.HowManyCones;
        }
        else if (shape == EnumShapes.Stars)
        {
            gameContainer.SaveRoot.Value = thisCard.HowManyStars;
        }
        else
        {
            throw new CustomBasicException("Don't know what to use");
        }
        gameContainer.SaveRoot.GameStatus = EnumStatusList.Normal;
        model!.ShapeChoose1!.ChoosePiece(shape);
        if (gameContainer.CloseShapeButton == null)
        {
            throw new CustomBasicException("Nobody is closing the shapes.  Rethink");
        }
        gameContainer.CloseShapeButton.Invoke();
        await gameContainer.EndTurnAsync!.Invoke();
    }
}