using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        // Demonstrates common uses of List<T>.
        // Doc: https://msdn.microsoft.com/library/6sh2ey19.aspx
        
        // Create a list of parts.
        var parts = new List<Part>();

        // Add a part to the list.
        var crank = new Part();
        crank.PartName = "crank arm";
        crank.PartId = 1234;
        parts.Add(crank);

        // Add parts to the list with more compact syntax via object initializers.
        parts.Add(new Part() { PartName = "chain ring", PartId = 1334 });
        parts.Add(new Part() { PartName = "regular seat", PartId = 1434 });
        parts.Add(new Part() { PartName = "banana seat", PartId = 1444 });
        parts.Add(new Part() { PartName = "cassette", PartId = 1534 });
        parts.Add(new Part() { PartName = "shift lever", PartId = 1634 }); ;
 
        PrintParts(parts);
        
        Console.WriteLine();
        Console.WriteLine("Insert brake lever part at index 2");
        parts.Insert(2, new Part() { PartName = "brake lever", PartId = 1834 });

        PrintParts(parts);
        
        Console.WriteLine();
        Console.WriteLine("Remove part at index 3");
        parts.RemoveAt(3);
        
        PrintParts(parts);
         
        /*
        Expected console output:
        
        6 Parts:
        ID: 1234, Name: crank arm
        ID: 1334, Name: chain ring
        ID: 1434, Name: regular seat
        ID: 1444, Name: banana seat
        ID: 1534, Name: cassette
        ID: 1634, Name: shift lever
        
        Insert brake lever part at index 2
        
        7 Parts:
        ID: 1234, Name: crank arm
        ID: 1334, Name: chain ring
        ID: 1834, Name: brake lever
        ID: 1434, Name: regular seat
        ID: 1444, Name: banana seat
        ID: 1534, Name: cassette
        ID: 1634, Name: shift lever
        
        Remove part at index 3
        
        6 Parts:
        ID: 1234, Name: crank arm
        ID: 1334, Name: chain ring
        ID: 1834, Name: brake lever
        ID: 1444, Name: banana seat
        ID: 1534, Name: cassette
        ID: 1634, Name: shift lever

     */
	}
    
    // Write out the parts in the list.
    static void PrintParts(List<Part> parts)
    {
        Console.WriteLine();
        Console.WriteLine("{0} Parts:", parts.Count);
        
        // List<T> implements IEnumerable, so supports foreach.
        foreach (var part in parts)
        {
            // WriteLine calls the overridden ToString method in the Part class.
            Console.WriteLine(part);
        }
    }
}

// Part describes a 'part' in an order system
public class Part
{
    public string PartName {get; set;}
    public int PartId {get; set;}

    public override string ToString()
    {
        return String.Format("ID: {0}, Name: {1}", PartId, PartName);
    }
}
