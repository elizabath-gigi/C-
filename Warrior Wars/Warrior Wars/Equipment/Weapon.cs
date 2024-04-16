
namespace WarriorWars.Equipment;
using WarriorWars.Enum;
class Weapon
{
    private const int good_damage = 5;
    private const int bad_damage = 5;
    private int damage;
    public int Damage
    {
        get
        {
            return damage;
        }
    }
    public Weapon(Faction faction)
    {
        switch (faction)
        {
            case Faction.Goodguy:
                damage = good_damage;
                break;
            case Faction.Badguy:
                damage = bad_damage;
                break;
            default:
                break;
        }


    }
}