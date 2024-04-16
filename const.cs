using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


	public class Point
	{
	  public static int ID;
	  public int id;
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
	   
	    id=++ID;
	  }
	  public Point(int y)
	  {
	   
	    id=++ID;
	  }
	
	}
	class EntryPoint
	{
	  static void Main()
	  {
	    Point p=new Point(7);
	    Point p1=new Point();
	    Point p2=new Point();
	    p.X=6;
	    Console.WriteLine($"{p.id},{p1.id},{p2.id},{Point.ID}");
	    
	  }
	}