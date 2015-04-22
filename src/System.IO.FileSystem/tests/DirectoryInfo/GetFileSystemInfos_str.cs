// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.IO;
using Xunit;

public class DirectoryInfo_GetFileSystemInfos_str
{
    public static String s_strTFName = "GetFileSystemInfos _str.cs";

    [Fact]
    public static void runTest()
    {
        //////////// Global Variables used for all tests
        int iCountErrors = 0;
        int iCountTestcases = 0;
        String strLoc = "Loc_000oo";
        String strValue = String.Empty;


        try
        {
            /////////////////////////  START TESTS ////////////////////////////
            ///////////////////////////////////////////////////////////////////

            DirectoryInfo dir2;
            String dirName = Path.GetRandomFileName();
            FileSystemInfo[] fsArr;

            if (Directory.Exists(dirName))
                Directory.Delete(dirName, true);

            // [] Should return zero length array for an empty directory
            //-----------------------------------------------------------------
            strLoc = "Loc_4y982";

            dir2 = Directory.CreateDirectory(dirName);
            fsArr = dir2.GetFileSystemInfos();
            iCountTestcases++;
            if (fsArr.Length != 0)
            {
                iCountErrors++;
                printerr("Error_207v7! Incorrect number of files returned");
            }
            //-----------------------------------------------------------------


            // [] Create a directorystructure and try different searchcriterias
            //-----------------------------------------------------------------
            strLoc = "Loc_2398c";

            dir2.CreateSubdirectory("TestDir1");
            dir2.CreateSubdirectory("TestDir2");
            dir2.CreateSubdirectory("TestDir3");
            dir2.CreateSubdirectory("Test1Dir1");
            dir2.CreateSubdirectory("Test1Dir2");
            new FileInfo(Path.Combine(dir2.FullName, "TestFile1")).Create();
            new FileInfo(Path.Combine(dir2.FullName, "TestFile2")).Create();
            new FileInfo(Path.Combine(dir2.FullName, "TestFile3")).Create();
            new FileInfo(Path.Combine(dir2.FullName, "Test1File1")).Create();
            new FileInfo(Path.Combine(dir2.FullName, "Test1File2")).Create();

            // [] Search criteria ending with '*'

            iCountTestcases++;
            fsArr = dir2.GetFileSystemInfos("TestFile*");
            iCountTestcases++;
            if (fsArr.Length != 3)
            {
                iCountErrors++;
                printerr("Error_1yt75! Incorrect number of files returned");
            }
            String[] names = new String[fsArr.Length];
            int i = 0;
            foreach (FileSystemInfo f in fsArr)
                names[i++] = f.Name;
            
            if (!Interop.IsWindows) // test is expecting sorted order as provided by Windows
            {
                Array.Sort(names);
            }

            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile1") < 0)
            {
                iCountErrors++;
                printerr("Error_3y775! Incorrect name==" + fsArr[0].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile2") < 0)
            {
                iCountErrors++;
                printerr("Error_90885! Incorrect name==" + fsArr[1].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile3") < 0)
            {
                iCountErrors++;
                printerr("Error_879by! Incorrect name==" + fsArr[2].Name);
            }


            // [] Search criteria is '*'

            fsArr = dir2.GetFileSystemInfos("*");
            iCountTestcases++;
            if (fsArr.Length != 10)
            {
                iCountErrors++;
                printerr("Error_t5792! Incorrect number of files==" + fsArr.Length);
            }
            names = new String[fsArr.Length];
            i = 0;
            foreach (FileSystemInfo f in fsArr)
                names[i++] = f.Name;
            
            if (!Interop.IsWindows) // test is expecting sorted order as provided by Windows
            {
                Array.Sort(names);
            }

            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir1") < 0)
            {
                iCountErrors++;
                printerr("Error_4898v! Incorrect name==" + fsArr[0].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir2") < 0)
            {
                iCountErrors++;
                printerr("Error_4598c! Incorrect name==" + fsArr[1].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir1") < 0)
            {
                iCountErrors++;
                printerr("Error_209d8! Incorrect name==" + fsArr[2].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_10vtu! Incorrect name==" + fsArr[3].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir3") < 0)
            {
                iCountErrors++;
                printerr("Error_190vh! Incorrect name==" + fsArr[4].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1File1") < 0)
            {
                iCountErrors++;
                printerr("Error_4898v! Incorrect name==" + fsArr[5].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1File2") < 0)
            {
                iCountErrors++;
                printerr("Error_4598c! Incorrect name==" + fsArr[6].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile1") < 0)
            {
                iCountErrors++;
                printerr("Error_209d8! Incorrect name==" + fsArr[7].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile2") < 0)
            {
                iCountErrors++;
                printerr("Error_10vtu! Incorrect name==" + fsArr[8].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile3") < 0)
            {
                iCountErrors++;
                printerr("Error_190vh! Incorrect name==" + fsArr[9].Name);
            }

            // [] Search criteria beginning with '*'

            fsArr = dir2.GetFileSystemInfos("*2");
            iCountTestcases++;
            if (fsArr.Length != 4)
            {
                iCountErrors++;
                printerr("Error_8019x! Incorrect number of files==" + fsArr.Length);
            }
            names = new String[fsArr.Length];
            i = 0;
            foreach (FileSystemInfo fs in fsArr)
                names[i++] = fs.Name;
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir2") < 0)
            {
                iCountErrors++;
                printerr("Error_247yg! Incorrect name==" + fsArr[0].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_24gy7! Incorrect name==" + fsArr[1].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1File2") < 0)
            {
                iCountErrors++;
                printerr("Error_167yb! Incorrect name==" + fsArr[2].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestFile2") < 0)
            {
                iCountErrors++;
                printerr("Error_49yb7! Incorrect name==" + fsArr[3].Name);
            }



            fsArr = dir2.GetFileSystemInfos("*Dir2");
            iCountTestcases++;
            if (fsArr.Length != 2)
            {
                iCountErrors++;
                printerr("Error_948yv! Incorrect number of files==" + fsArr.Length);
            }
            names = new String[fsArr.Length];
            i = 0;
            foreach (FileSystemInfo fs in fsArr)
                names[i++] = fs.Name;
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir2") < 0)
            {
                iCountErrors++;
                printerr("Error_247yg! Incorrect name==" + fsArr[0].Name);
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_24gy7! Incorrect name==" + fsArr[1].Name);
            }

            // [] Search criteria Beginning and ending with '*'

            new FileInfo(Path.Combine(dir2.FullName, "AAABB")).Create();
            Directory.CreateDirectory(Path.Combine(dir2.FullName, "aaabbcc"));

            fsArr = dir2.GetFileSystemInfos("*BB*");
            iCountTestcases++;
            if (fsArr.Length != (Interop.IsWindows ? 2 : 1))
            {
                iCountErrors++;
                printerr("Error_4y190! Incorrect number of files==" + fsArr.Length);
            }
            names = new String[fsArr.Length];
            i = 0;
            foreach (FileSystemInfo fs in fsArr)
                names[i++] = fs.Name;

            if (Interop.IsWindows)
            {

                iCountTestcases++;
                if (Array.IndexOf(names, "aaabbcc") < 0)
                {
                    iCountErrors++;
                    printerr("Error_956yb! Incorrect name==" + fsArr[0]);
                    foreach (FileSystemInfo s in fsArr)
                        Console.WriteLine(s.Name);
                }
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "AAABB") < 0)
            {
                iCountErrors++;
                printerr("Error_48yg7! Incorrect name==" + fsArr[1]);
                foreach (FileSystemInfo s in fsArr)
                    Console.WriteLine(s.Name);
            }
            strLoc = "Loc_0001";

            // [] Should not search on fullpath
            // [] Search Criteria without match should return empty array

            fsArr = dir2.GetFileSystemInfos("Directory");
            iCountTestcases++;
            if (fsArr.Length != 0)
            {
                iCountErrors++;
                printerr("Error_209v7! Incorrect number of files==" + fsArr.Length);
            }

            new FileInfo(Path.Combine(dir2.FullName, "TestDir1", "Test.tmp")).Create();
            fsArr = dir2.GetFileSystemInfos(Path.Combine("TestDir1", "*"));
            iCountTestcases++;
            if (fsArr.Length != 1)
            {
                iCountErrors++;
                printerr("Error_28gyb! Incorrect number of files");
            }
        }
        catch (Exception exc_general)
        {
            ++iCountErrors;
            Console.WriteLine("Error Err_8888yyy!  strLoc==" + strLoc + ", exc_general==" + exc_general.ToString());
        }
        ////  Finish Diagnostics
        if (iCountErrors != 0)
        {
            Console.WriteLine("FAiL! " + s_strTFName + " ,iCountErrors==" + iCountErrors.ToString());
        }

        Assert.Equal(0, iCountErrors);
    }

    public static void printerr(String err, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Console.WriteLine("ERROR: ({0}, {1}, {2}) {3}", memberName, filePath, lineNumber, err);
    }
}
