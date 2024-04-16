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
	    get
	    {
	      return x;
	    }
	    set
	    {
	      x=value;
	    }
	  }
	  public int Y
	  {
	    get
	    {
	      return y;
	    }
	    set
	    {
	      y=value;
	    }
	  }
	  public Point()
	  {
	    
	  }
	  public Point(int x,int y)
	  {
	    this.x=x;
	    this.y=y;
	  }
	
	}
	class EntryPoint
	{
	  static void Main()
	  {
	    Point p=new Point(5,4);
	    Console.WriteLine($"{p.X},{p.Y}");
	    Point p1=new Point();
	    p1.X=170;
	    p1.Y=67;
	    Console.WriteLine($"{p1.X},{p1.Y}");
	  }
	}