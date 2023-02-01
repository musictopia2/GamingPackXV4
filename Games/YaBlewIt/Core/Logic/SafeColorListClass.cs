namespace YaBlewIt.Core.Logic;
public class SafeColorListClass : IEnumListClass<EnumColors>
{
    public static void ClearContainer()
    {
        _gameContainer = null;
    }
    public static BasicList<EnumColors> GetColorChoices()
    {
        _gameContainer ??= aa1.Resolver!.Resolve<YaBlewItGameContainer>();
        if (_gameContainer.SingleInfo is null)
        {
            return new(); //hopefully will be okay (?)
        }
        BasicList<EnumColors> output = new();
        //needs to be here so if there are no more colors to choose from, then won't even let the player choose the safe card.
        //needs to already be set to the proper SingleInfo.
        
        var filters = _gameContainer.SingleInfo.MainHandList.Where(x => x.CardColor != EnumColors.Wild && x.CardCategory == EnumCardCategory.Gem).ToBasicList();
        var groups = filters.GroupBy(x => x.CardColor).ToBasicList();
        var others = _gameContainer.SaveRoot.ProtectedColors;
        foreach (var group in groups)
        {
            var color = group.Key;
            if (others.Contains(color) == false && color != _gameContainer.SingleInfo.CursedGem)
            {
                output.Add(color);
            }
        }
        //since it was able to return blue, this means there is real hope to show the proper colors.
        //i have the game container if needed as well.

        //for now, only return blue.  to see if there is hope.
        //BasicList<EnumColors> output = new()
        //{
        //    EnumColors.Blue
        //};
        return output;
    }

    private static YaBlewItGameContainer? _gameContainer;
    BasicList<EnumColors> IEnumListClass<EnumColors>.GetEnumList()
    {

        BasicList<EnumColors> output = GetColorChoices();



        
        return output;
    }
}