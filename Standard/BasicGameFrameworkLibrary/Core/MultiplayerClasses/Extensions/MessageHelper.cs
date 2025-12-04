namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.Extensions;
public static class MessageHelper
{
    extension <P>(P singleInfo)
         where P : class, IPlayerItem, new()
    {
        public bool CanSendMessage(BasicData thisData)
        {
            if (thisData.MultiPlayer == false)
            {
                return false;
            }
            if (singleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                return true;
            }
            if (thisData.Client == false && singleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                return true;
            }
            return false;
        }
    }   
}