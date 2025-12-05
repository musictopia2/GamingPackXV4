namespace ThreeLetterFun.Core.Logic;
internal static class Extensions
{
    extension (IDeckDict<ThreeLetterFunCardData> list)
    {
        public void RemoveTiles()
        {
            foreach (var thisCard in list)
            {
                thisCard.ClearTiles();
            }
        }
    }
    extension (BasicList<TileInformation> list)
    {
        public void RemoveTiles(ThreeLetterFunVMData model)
        {
            list.RemoveRange(0, 2);
            model.TileBoard1!.UpdateBoard(); //i think
        }
        public void RemoveTiles()
        {
            list.RemoveRange(0, 2);
        }
    }
    extension (PlayerCollection<ThreeLetterFunPlayerItem> players)
    {
        public void TakeTurns()
        {
            players.ForEach(player =>
            {
                player.ClearTurn();
            });
        }
    }
    
}