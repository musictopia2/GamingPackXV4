namespace TileRummy.Core.Logic;
public class TileHand : HandObservable<TileInfo>
{
    public TileHand(CommandContainer command) : base(command)
    {
        Text = "Your Tiles";
        AutoSelect = EnumHandAutoType.SelectAsMany;
    }
    public override void EndTurn()
    {
        HandList.ForEach(thisTile =>
        {
            if (thisTile.WhatDraw != EnumDrawType.FromSet)
            {
                thisTile.WhatDraw = EnumDrawType.IsNone;
            }
            thisTile.IsSelected = false;
            thisTile.Drew = false; //needs this as well.
        });
    }
}