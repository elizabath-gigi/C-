using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


	public class Point
	{
	  private int x;
	  private int y;
	  public int X
	  {
	    //write only
	    set
	    {
	      if(value>5)
	      {
	        x=value;
	        Console.WriteLine("Value set");
	      }
	      else
	      {
	        Console.WriteLine("Oops!");
	      }
	    }
	  }
	  public int Y
	  {
	    //read only
	    get
	    {
	      return y;
	    }
	    
	  }
	
	  public Point()
	  {
	    
	    y=9;
	  }
	
	}
	class EntryPoint
	{
	  static void Main()
	  {
	    Point p=new Point();
	    p.X=6;
	    Console.WriteLine($"{p.Y}");
	  }
	}