namespace ClueBoardGame.Core.Data;
public class WeaponInfo
{
    public string Name { get; set; } = "";
    public int Room { get; set; }
    public EnumWeaponList Weapon { get; set; }
    public SizeF DefaultSize
    {
        get
        {
            if (Weapon == EnumWeaponList.None)
            {
                return new SizeF(55, 72);
            }
            switch (Weapon)
            {
                case EnumWeaponList.Candlestick:
                    {
                        return new SizeF(25, 35);
                    }

                case EnumWeaponList.Knife:
                    {
                        return new SizeF(25, 22);
                    }

                case EnumWeaponList.LeadPipe:
                    {
                        return new SizeF(15, 37);
                    }

                case EnumWeaponList.Revolver:
                    {
                        return new SizeF(25, 15);
                    }

                case EnumWeaponList.Rope:
                    {
                        return new SizeF(25, 20);
                    }

                case EnumWeaponList.Wrench:
                    {
                        return new SizeF(25, 28);
                    }

                default:
                    {
                        throw new CustomBasicException("Not supported");
                    }
            }
        }
    }
}