namespace Xactika.Core.Logic;
public interface IShapeProcesses
{
    Task ShapeChosenAsync(EnumShapes shape);
    Task FirstCallShapeAsync();
}