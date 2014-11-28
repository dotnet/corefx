using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using System.IO;

namespace TestCorefx
{
    class Program
    {
        static void Main(string[] args)
        {		          
           if (args.Length == 0 || ! File.Exists(args[0]))
           {
              return;
           } 

           System.Console.WriteLine("Processing " + args[0]);
           TestCorefx.Program obj = new TestCorefx.Program();  
           Byte[] b = File.ReadAllBytes(args[0]);
           Stream s = new MemoryStream(b);
		   
		   /*
		       This constructor call is defined as...
			   public PEHeaders(Stream peStream, int size) : this(peStream, (int?)size) {} 
			   In the above definition the call to the constructor is made to to the definition...
			   private PEHeaders(Stream peStream, int? sizeOpt) {... }
			   This above definition is where all of the heavy work of parsing is done... Now some thing about that weird looking question mark 
               The symbol ? in here means that the type in question is a nullable type, Nullable types are instances of the System.Nullable struct...
               That is why we also see such z definition as...
			   public PEHeaders(Stream peStream) : this(peStream, null) {}
		   */
           PEHeaders pe = new PEHeaders(s);
		   		   		   
		   /* Get DOS header and DOS stub */
		   byte[] dosStub = pe.DOSHeader;
		   CoffHeader ntCoff = pe.CoffHeader;
           PEHeader ntPE = pe.PEHeader;		   
		   
		   System.Console.WriteLine(dosStub.Length);
		   
		   for (int i = 0; i < dosStub.Length; i++) {
		   
		      System.Console.WriteLine("Hex: {0:X}", dosStub[i]);
		   }
		   
           if (pe.IsExe)
           {
              obj.ExeFile(ref pe);
           }
           else if (pe.IsDll)
           {

           }
           else
           {
              System.Console.WriteLine("It is even possible?");
           }
        }

        void ExeFile(ref PEHeaders pe)
        {
           System.Console.WriteLine("Processing an EXE file...");

           System.Console.WriteLine(pe.PEHeader.Magic.ToString());           
        }   
    }
}
