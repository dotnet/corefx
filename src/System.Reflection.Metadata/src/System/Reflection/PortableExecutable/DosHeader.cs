// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.PortableExecutable
{
    // DOS .EXE header(Referenced WinNT.h for structure IMAGE_DOS_HEADER) 
    public sealed class DosHeader
    {
        #region standard header fields

        /// <summary>
		/// Signature Word, this contains a 'magic number' which provides a simple check that the file really is a DOS .EXE file; it follows that the filename extension does not in fact have to be .EXE, as long as programs check this word. The value of this word is 5A4DH.  
        /// </summary>
        public ushort EMagic { get; private set; }
		
        /// <summary>
		/// Last Page Size, The file occupies a number of 512 byte pages. The last page may contain between 1 and 512 bytes. This word indicates the number of bytes actually used in the last page, with the special case of a full page being represented by a value of zero.        
        /// </summary>
        public ushort ECBlp { get; private set; }	

        /// <summary>
		/// File Pages, 
        /// Total number of pages required to hold the file(a page is a 512 bytes entity), including the last partial page.
        /// </summary>
        public ushort ECP { get; private set; }

        /// <summary>
        /// Number of entries in the relocation pointer table.		
        /// </summary>
        public ushort ECRlc { get; private set; }	

        /// <summary>
        /// Size of header in paragraphs(a paragraph is 16 bytes entity). It indicates the offset of the program's compiled/assembled and linked image(the load module) within the .EXE file. The size of the load module can be deduced by subtracting this value (converted to bytes) from the overall file size derived from combining the File Pages and Last Page Size values.
        /// </summary>
        public ushort ECParHdr { get; private set; }	

        /// <summary>      
		/// This word indicates the minimum number of paragraphs the program requires to begin execution. This is in addition to the memory required to hold the load module. This value normally represents the total size of any uninitialised data and/or stack segments that are linked at the end of a program. This space is not directly included in the load module, since there are no particular initialising values and it would simply waste disk space.
        /// </summary>
        public ushort EMinAlloc { get; private set; }

        /// <summary>       
		/// This word indicates the maximum number of paragraphs that the program would like allocated to it before it begins execution. This indicates additional memory over and above that required by the load module and the value specified by EMinAlloc. If the request cannot be satisfied, the program is allocated as much memory as is available. 
        /// </summary>
        public ushort EMaxAlloc { get; private set; }

        /// <summary>        
		/// This word contains the paragraph address of the stack segment relative to the start of the load module. At load time, this value is relocated by adding the address of the start segment of the program to it, and the resulting value is placed in the SS register before the program is started. In DOS, the start segment of the program is the first segment boundary in memory after the PSP. 
        /// </summary>
        public ushort ESS { get; private set; }

        /// <summary>
        /// This word contains the absolute value that must be loaded into the SP register before the program is given control. Since the actual stack segment is determined by the loader, and this is merely a value within that segment, it does not need to be relocated. 
        /// </summary>
        public ushort ESP { get; private set; }	
		
        /// <summary>
        /// Checksum.
        /// </summary>
        public ushort ECSum { get; private set; }	

        /// <summary>
        /// This word contains the absolute value that should be loaded into the IP register in order to transfer control to the program. Since the actual code segment is determined by the loader, and this is merely a value within that segment, it does not need to be relocated.
        /// </summary>
        public ushort EIP { get; private set; }	

        /// <summary>
        /// This word contains the initial value, relative to the start of the load module, that should be placed in the CS register in order to transfer control to the program. At load time, this value is relocated by adding the address of the start segment of the program to it, and the resulting value is placed in the CS register when control is transferred. 
        /// </summary>
        public ushort ECS { get; private set; }

        /// <summary>
        /// File address of relocation table(byte offset), a value of 0x40 indicates a none MZ kind of executable file, otherwise it is a byte offset
		/// or an array of segmented addresses. At run time each such address gets updated by adding the paragraph number of CS in the segment portion of the segmented address.
        /// </summary>
        public ushort ElFARlc { get; private set; }

        /// <summary>
        /// Overlay number, overlays are sections of a program that remain on disk until the program actually requires them. This word is normally set to 0000H.
        /// </summary>
        public ushort EOVNO { get; private set; }
		
		#endregion
		
		#region extended header fields

        /// <summary>
        /// Reserved words.
        /// </summary>
        public ushort[] ERes { get; private set; }

        /// <summary>
        /// OEM identifier(for EOemInfo).
        /// </summary>
        public ushort EOemID { get; private set; }

        /// <summary>
        /// OEM information(EOemID specific).
        /// </summary>
        public ushort EOemInfo { get; private set; }

        /// <summary>
        /// Reserved words.
        /// </summary>
        public ushort[] ERes2 { get; private set; }		
						
		/// <summary>
        /// File address of new exe header.
        /// </summary>
        public int ElFANew { get; private set; }
										
        #endregion

        #region DOS stub
        
		public byte[] DosStub { get; private set; }

        #endregion 		
		
	    internal DosHeader(ref PEBinaryReader reader)
        {		
		    EMagic = reader.ReadUInt16();
			ECBlp = reader.ReadUInt16();
			ECP = reader.ReadUInt16();
			ECRlc = reader.ReadUInt16();
			ECParHdr = reader.ReadUInt16();
			EMinAlloc = reader.ReadUInt16();
			EMaxAlloc = reader.ReadUInt16();
			ESS = reader.ReadUInt16();
			ESP = reader.ReadUInt16();
			ECSum = reader.ReadUInt16();
			EIP = reader.ReadUInt16();
			ECS = reader.ReadUInt16();
			ElFARlc = reader.ReadUInt16();
			EOVNO = reader.ReadUInt16();
			
			ERes = new ushort[4];
			Buffer.BlockCopy(reader.ReadBytes(4*sizeof(ushort)), 0, ERes, 0, 4*sizeof(ushort));
			EOemID = reader.ReadUInt16();
			EOemInfo = reader.ReadUInt16();
			ERes2 = new ushort[10];
			Buffer.BlockCopy(reader.ReadBytes(10*sizeof(ushort)), 0, ERes2, 0, 10*sizeof(ushort));
			ElFANew = reader.ReadInt32();
			
			if (ElFARlc == 0x40 && ECRlc == 0)
			{
			    DosStub = reader.ReadBytes(ElFANew - ECParHdr*16);
			}
            else
            {
			     DosStub = null;
            }			
	    }
	}
}	