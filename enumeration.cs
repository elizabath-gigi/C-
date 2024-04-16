using System;
enum Race
{
  Earthling,
  Marsian
}class User
{
  public string username;
  public Race race;
  public readonly int HEIGHT;
  public User(string username,Race race)
  {
    this.username=username;
    this.race=race;
    if(race==Race.Earthling)
    {
      HEIGHT=100;
    }
    else if(race==Race.Marsian)
    {
      HEIGHT=180;
    }
  }
}

class Entrypoint
{
  static void Main()
  {
    User u=new User("Ram",Race.Earthling);
    Console.WriteLine(u.HEIGHT);
  }
}