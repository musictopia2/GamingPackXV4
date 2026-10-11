namespace Xactika.Core.ViewModels;
public class XactikaSubmitShapeViewModel(CommandContainer commandContainer,
    XactikaVMData model,
    IShapeProcesses processes,
    XactikaGameContainer gameContainer
        ) : BasicSubmitViewModel(commandContainer)
{
    public override string Text => "Choose Shape";
    public override bool CanSubmit => model.ShapeChosen != EnumShapes.None;
    public override async Task SubmitAsync()
    {
        if (gameContainer.BasicData.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync("shapeused", model.ShapeChosen);
        }
        await processes.ShapeChosenAsync(model.ShapeChosen);
    }
}