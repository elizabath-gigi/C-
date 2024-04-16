// See https://aka.ms/new-console-template for more information

int a = 10;
int b = 11;
var name = "ABCD";
string userChoice = Console.ReadLine();
Console.WriteLine("User Input" + userChoice);
bool isUserInputABC = userChoice == "ABC";
Console.WriteLine(isUserInputABC);



++a;
--b;

Console.WriteLine("addition:" + a + b);
Console.WriteLine("subtraction:" + (a - b));
Console.WriteLine("multiplication:" + a * b);
Console.WriteLine("division:" + a / b);
Console.WriteLine(name);
Console.ReadKey();

