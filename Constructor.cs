using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


	public class Human
	{
	  public int height;
	  public int weight;
	  public int heart_rate;
	  public Human()
	  {
	    
	  }
	  public Human(int height,int w)
	  {
	    this.height=height;
	    weight=w;
	    heart_rate=72;
	  }
	
	}
	class EntryPoint
	{
	  static void Main()
	  {
	    Human h1=new Human();
	    Human h2=new Human(150,50);
	    Console.WriteLine($"{h2.height},{h2.weight},{h2.heart_rate}");
	    h1.height=170;
	    h1.weight=67;
	    Console.WriteLine($"{h1.height},{h1.weight}");
	  }
	}