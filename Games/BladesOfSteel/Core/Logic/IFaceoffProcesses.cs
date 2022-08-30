namespace BladesOfSteel.Core.Logic;
public interface IFaceoffProcesses
{
    Task FaceOffCardAsync(RegularSimpleCard card);
}