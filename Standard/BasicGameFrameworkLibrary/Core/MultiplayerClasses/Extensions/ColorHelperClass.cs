namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.Extensions;
public static class ColorHelperClass
{
    extension <P>(PlayerCollection<P> players)
        where P : class, IPlayerColors, new()
    {
        public bool DidChooseColors()
        {
            foreach (var player in players)
            {
                if (player.DidChooseColor == false && player.InGame)
                {
                    return false;
                }
            }
            return true;
        }
        public void EraseColors()
        {
            players!.ForEach(items =>
            {
                items.Clear();
            });
        }
    }
    
}