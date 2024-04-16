

namespace WarriorWars;
using WarriorWars.Enum;
using WarriorWars.Equipment;
public class Warrior
{
    private int health;
    private string name;
    private bool isAlive;

    private const int Good_st_hlth = 20;
    private const int Bad_st_hlth = 20;

    private readonly Faction faction;


    private Weapon w;
    private Armor a;

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }

    }
    public Warrior(string name,Faction faction)
    {
        this.name = name;
        this.faction = faction;
        isAlive = true;
        w = new Weapon(faction);
        a = new Armor(faction);
        switch (faction)
        {
            case Faction.Goodguy:
                
                health = Good_st_hlth;
                break;
            case Faction.Badguy:
               // Weapon w = new Weapon(faction);
                //Armor a = new Armor(faction);
                health = Bad_st_hlth;
                break;
            default:
                break;
        }
    }
    public void Attack(Warrior enemy)
    {
        int damage = w.Damage / enemy.a.ArmorPoints;
        enemy.health -= damage;
      if (enemy.health <= 0)
        {
            enemy.isAlive = false;
            Console.WriteLine($"{enemy.name} is dead,{name} is victorious.");
        }
        else
        {
            Console.WriteLine($"{name} attacked {enemy.name},{damage} was inflicted,remaining health is{enemy.health}");
        }
    }
}
