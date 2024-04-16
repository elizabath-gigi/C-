
namespace WarriorWars.Equipment;
using WarriorWars.Enum;
class Armor
{
    private int armorPoints;
    private const int good_armor = 5;
    private const int bad_armour = 5;
    public int ArmorPoints
    {
        get
        {
            return armorPoints;
        }
    }
    public Armor(Faction faction)
    {
        switch (faction)
        {
            case Faction.Goodguy:
                armorPoints = good_armor;
                break;
            case Faction.Badguy:
                armorPoints = bad_armour;
                break;
            default:
                break;
        }
    }

}
