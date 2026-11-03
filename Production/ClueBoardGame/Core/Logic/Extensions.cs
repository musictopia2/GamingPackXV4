namespace ClueBoardGame.Core.Logic;
public static class Extensions
{
    extension (Dictionary<int, WeaponInfo> thisList)
    {
        public void PlaceWeaponsInRooms()
        {
            if (thisList.Count != 6)
            {
                throw new CustomBasicException("There has to be 6 weapons");
            }
            BasicList<int> tempList = Enumerable.Range(1, 9).ToBasicList();
            tempList.ShuffleList();
            int x = 0;
            foreach (var thisWeapon in thisList.Values)
            {
                thisWeapon.Room = tempList[x];
                x++;
            }
        }
    }
    extension(Dictionary<int, CharacterInfo> thisList)
    {
        public void PlayerChoseColor(ClueBoardGamePlayerItem thisPlayer)
        {
            int id = thisPlayer.Id;
            foreach (var thisCharacter in thisList.Values)
            {
                if (thisCharacter.MainColor.Equals(thisPlayer.Color.Color))
                {
                    thisCharacter.Player = id;
                    return;
                }
            }
            throw new CustomBasicException("No color for player");
        }
    }
}