using System;
using System.Globalization;

public class Program
{
	public static void Main()
	{
		// Demonstrates common uses of formatting the DateTime struct.
		// Doc: https://msdn.microsoft.com/library/az4se3k1.aspx
		
		var date = DateTime.Now;
		
		// default formatting
		Console.WriteLine("Print DateTime with default formatting: {0}", date);
		Console.WriteLine();
		
		// short / long formatting
		Console.WriteLine("Print short date: {0}", date.ToString("d"));
		Console.WriteLine("Print long date: {0}", date.ToString("D"));
		Console.WriteLine("Print short time: {0}", date.ToString("t"));
		Console.WriteLine("Print long time: {0}", date.ToString("T"));
		Console.WriteLine();
		
		// custom formatting with templating
		Console.WriteLine("Print date with month/year format: {0}", date.ToString("MM/yy"));
		Console.WriteLine("Print date with month, year in verbose format: {0}", date.ToString("MMMM, yyyy"));
		Console.WriteLine("Print time: {0}", date.ToString("hh:mm:ss"));
		Console.WriteLine();
		
		// print with locale-specific formatting
		var ptBRCulture = new CultureInfo("pt-BR");
		var enUSCulture = new CultureInfo("en-US");
		var deDECulture = new CultureInfo("de-DE");
		var zhCNCulture = new CultureInfo("zh-CN");
		Console.WriteLine("Print DateTime with pt-BR local: {0}", date.ToString(ptBRCulture));
		Console.WriteLine("Print DateTime with en-US local: {0}", date.ToString(enUSCulture));
		Console.WriteLine("Print DateTime with de-DE local: {0}", date.ToString(deDECulture));
		Console.WriteLine("Print DateTime with zh-CN local: {0}", date.ToString(zhCNCulture));
		Console.WriteLine("Print long DateTime with zh-CN local: {0}", date.ToString("D",zhCNCulture));
		Console.WriteLine("Print long DateTime with de-DE local: {0}", date.ToString("D",deDECulture));
		
		Console.WriteLine();
	}
}
