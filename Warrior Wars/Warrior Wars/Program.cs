using WarriorWars.Enum;

namespace WarriorWars;

class EntryPoint()
{
    static Random rng = new Random();
    static void Main()
    {
        Warrior g = new Warrior("Peter", Faction.Goodguy);
        Warrior b = new Warrior("John", Faction.Badguy);

        while (g.IsAlive && b.IsAlive)
        {
            if (rng.Next(0, 10) < 5)
            {
                g.Attack(b);
            }
            else
            {
                b.Attack(g);
            }
        }
    }
}