// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello! \n Enter the first number:");
string firstInput = Console.ReadLine();
int firstNumber = int.Parse(firstInput);
Console.WriteLine("Enter the second number:");
string secondInput = Console.ReadLine();
int secondNumber = int.Parse(secondInput);
Console.WriteLine("What do you want to do with those numbers?");
Console.WriteLine("[A]dd");
Console.WriteLine("[S]ubtract");
Console.WriteLine("[M]ultiply");
string userOption = Console.ReadLine();
if(userOption=="A")
{
    Console.WriteLine($"{firstNumber}+{secondNumber}={firstNumber+secondNumber}");
}
else if (userOption == "S")
{
    Console.WriteLine($"{firstNumber}-{secondNumber}={firstNumber - secondNumber}");
}
else if (userOption == "M")
{
    Console.WriteLine($"{firstNumber}*{secondNumber}={firstNumber * secondNumber}");
}
else
{
    Console.WriteLine("Invalid");
}
Console.ReadKey();
