using System;

public class Program
{
	public static void Main()
	{
		// Demonstrates common uses of DateTime struct.
		// Doc: https://msdn.microsoft.com/library/system.datetime.aspx
		
		// Get and manipulate time
		Console.WriteLine("Print local DateTime");
		Console.WriteLine(DateTime.Now);
		Console.WriteLine();
		
		Console.WriteLine("Print UTC DateTime");
		Console.WriteLine(DateTime.UtcNow);
		Console.WriteLine();
		
		Console.WriteLine("Print local DateTime + 10 mins");
		Console.WriteLine(DateTime.Now.AddMinutes(10));
		Console.WriteLine();
		
		Console.WriteLine("Print local DateTime - 5 days and 4 hours");
		Console.WriteLine(DateTime.Now.AddDays(-5).AddHours(-4));
		Console.WriteLine();
		
		// Create a DateTime for a specific date/time
		// Uses Unix epoch as the example date - https://en.wikipedia.org/wiki/Unix_time
		// Always specific DateTimeKind when creating a new DateTime
		DateTime epoch = new DateTime(1970,1,1,0,0,0,DateTimeKind.Local);
		Console.WriteLine("Create and print the DateTime for 1/1/1970 (unix epoch)");
		Console.WriteLine(epoch);
		Console.WriteLine();
		
		var unixTime = 1581941594; // 2/17/2020 12:13:14 PM
		Console.WriteLine("Create and print a DateTime with Unix time");
		Console.WriteLine("The math: {0} + {1} seconds", epoch.Year, unixTime);
		Console.WriteLine(epoch.AddSeconds(unixTime));
		Console.WriteLine();

		// Operate on parts of DateTime
		Console.WriteLine("Print local year: {0}",DateTime.Now.Year);
		Console.WriteLine("Print local day of month: {0}", DateTime.Now.Day);
		Console.WriteLine("Print local date without time: {0}", DateTime.Now.Date);
		Console.WriteLine("Print local time: {0}", DateTime.Now.TimeOfDay);
		Console.WriteLine();
		
		// Convenience formatting - not yet supported on .NET Core
		//Console.WriteLine("Print short date: {0}", DateTime.Now.ToShortDateString());
		//Console.WriteLine("Print short time: {0}", DateTime.Now.ToShortTimeString());
		
		//Console.WriteLine();	
	}
}
