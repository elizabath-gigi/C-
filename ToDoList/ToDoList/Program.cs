// See https://aka.ms/new-console-template for more information


bool check;
int n;
string option;

List<string> todolist = new List<string>();
void PrintToDoList()
{
    Console.WriteLine("What is the index of the todo to be deleted");
    foreach (string i in todolist)
    {
        Console.WriteLine(i);
    }

}

do
{
    Console.WriteLine("Hello!");
    Console.WriteLine("What do you want to do?");
    Console.WriteLine("[S]ee all TODOs");
    Console.WriteLine("[A]dd a TODO");
    Console.WriteLine("[R]emove a TODO");
    Console.WriteLine("[E]xit");
    option = Console.ReadLine();
    switch (option)
    {
        case "S":
        case "s":
            if (todolist.Count == 0)
            {
                Console.WriteLine("TODO list is empty");
            }
            else
            {
                foreach (string i in todolist)
                {
                    Console.WriteLine(i);
                }
            }
            break;
        case "A":
        case "a":
            string newTodo = Console.ReadLine();
            bool noDuplicate = true;
            foreach (string i in todolist)
            {

                if (i == newTodo)
                {
                    noDuplicate = false;

                }
            }
            if (noDuplicate)
            {
                todolist.Add(newTodo);
            }
            break;
        case "R":
        case "r":

            do
            {

                PrintToDoList();
                string index = Console.ReadLine();
                check = int.TryParse(index, out int num);
                n = num;
            } while (!check);
            

            for (int k = 0; k < todolist.Count(); k++)
            {
                if (k == n-1)
                {
                    todolist.RemoveAt(k);
                }
            }
            break;
        default:
            Console.WriteLine("Invalid Index");
            break;





    }
}while (option != "E" && option != "e");




