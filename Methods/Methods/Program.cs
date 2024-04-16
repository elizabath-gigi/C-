// See https://aka.ms/new-console-template for more information
Console.WriteLine("Choose your favourite sport");
Console.WriteLine("[F]ootball");
Console.WriteLine("[V]olleyball");
Console.WriteLine("[C]ricket");
bool k = Is("hello");
bool l = Long("hello");
bool isLong = IsLong("hello");
Console.WriteLine(isLong);
Console.WriteLine(l);
Console.WriteLine(k);
var userChoice = Console.ReadLine();

if (userChoice=="F")
{
    PrintSelectedOption("Football");
}
else if(userChoice=="V")
{
    PrintSelectedOption("Volleyball");
}
else
{
    PrintSelectedOption("Cricket");
}
int result=sum(9, 10);
Console.WriteLine(result);
void PrintSelectedOption(string option)
{
    Console.WriteLine($"Selected option is {option}");

}
int sum(int a,int b)
{
    return a + b;
}
Console.ReadKey();
bool Long(string input)
{
    if(input.Length >10)
    {
        return true;
    }
    else
    {
        return false;
    }
}
bool Is(string input)
{
    if (input.Length > 10)
    {
        return true;
    }
   
    return false;
   
}
bool IsLong(string input)
{
   
    return input.Length > 10;
  
}