using BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
using BasicGameFrameworkLibrary.Core.MahjongTileClasses;
using SvgHelper.Blazor.Logic;
using SvgHelper.Blazor.Logic.Classes.SubClasses;
using System.Drawing; //don't worry about globalusings because this is all there is.
namespace MahjongTilesBlazor;
public class MahjongTilesComponent : BaseDeckGraphics<MahjongSolitaireTileInfo>
{
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
        Image image = new();
        image.PopulateBasicExternalImage($"{DeckObject!.Index}.svg");
        PopulateImage(image);
        MainGroup!.Children.Add(image); //hopefully this simple.
    }
}