
 List<string> GetOnlyUpperCaseWords(List<string> words)
{

    List<string> l = new List<string>();
    foreach (var i in words)
    {
        if (l.Contains(i))
        {
            continue;
        }
        bool uppercase = true;
        foreach (var j in i)
        {
            if (!char.IsUpper(j))
            {
                uppercase = false;
            }


        }
        if (uppercase)
        {
            l.Add(i);
        }

    }
    return l;
}
List<string> word = new List<string>();
List<string> result = new List<string>();
string userInput;
Console.WriteLine("Read the list");
do
{
    userInput = Console.ReadLine();
    word.Add(userInput);

} while (userInput != "stop");

result = GetOnlyUpperCaseWords(word);
Console.WriteLine("Result:");
foreach (var i in result)
{
    Console.WriteLine(i);
}


