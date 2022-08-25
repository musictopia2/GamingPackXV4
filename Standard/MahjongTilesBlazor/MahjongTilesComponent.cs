using BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
using BasicGameFrameworkLibrary.Core.MahjongTileClasses;
using SvgHelper.Blazor.Logic;
using SvgHelper.Blazor.Logic.Classes.SubClasses;
using System.Drawing; //don't worry about globalusings because this is all there is.
namespace MahjongTilesBlazor;
public class MahjongTilesComponent : BaseDeckGraphics<MahjongSolitaireTileInfo>
{
    //somehow don't have this anymore (?)
    //iffy now
    //protected override bool NeedsCommand()
    //{
    //    return DeckObject!.IsEnabled;
    //}
    public MahjongTilesComponent() { }
    protected override SizeF DefaultSize { get; } = new SizeF(136, 176);
    protected override bool NeedsToDrawBacks { get; } = false;
    protected override bool ShowDisabledColors => true;
    protected override bool CanStartDrawing()
    {
        return true;
    }
    protected override void DrawBacks()
    {
        //has no backs.
    }
    protected override void DrawImage() //what makes this one so big is the mahjong tiles.
    {
        //this is where i focus on the image i am drawing.
        Image image = new();
        image.PopulateFullExternalImage(this, $"{DeckObject!.Index}.svg");
        PopulateImage(image);
        MainGroup!.Children.Add(image); //hopefully this simple.
    }
}