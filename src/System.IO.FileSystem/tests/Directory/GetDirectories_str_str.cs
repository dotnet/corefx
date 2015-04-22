// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

public class Directory_Co5674GetDirectories_Str_Str
{
    public static String s_strTFName = "GetDirectories_Str_Str.cs";

    [Fact]
    public static void runTest()
    {
        int iCountErrors = 0;
        int iCountTestcases = 0;
        String strLoc = "Loc_000oo";

        try
        {
            /////////////////////////  START TESTS ////////////////////////////
            ///////////////////////////////////////////////////////////////////

            DirectoryInfo dir2;
            String dirName = "GetDirectories_Str_Str_test_TestDir";
            String[] strArr;

            if (Directory.Exists(dirName))
                Directory.Delete(dirName, true);


            strLoc = "Loc_4yg7b";
            iCountTestcases++;
            try
            {
                String[] sDirs = Directory.GetDirectories(".", String.Empty);
                // To avoid OS differences we have decided not to throw an argument exception when empty
                // string passed. But we should return 0 items.
                if (sDirs.Length != 0)
                {
                    iCountErrors++;
                    printerr("Error_543543! Invalid number of directories returned");
                }
            }
            catch (Exception exc)
            {
                iCountErrors++;
                printerr("Error_21t0b! Incorrect exception thrown, exc==" + exc.ToString());
            }

            //-----------------------------------------------------------------

            // [] ArgumentException for all whitespace in directory name or search string
            //-----------------------------------------------------------------
            strLoc = "Loc_1190x";

            iCountTestcases++;
            try
            {
                String[] strDirs = Directory.GetDirectories(".", "\n");
                if (strDirs.Length > 0)
                {
                    iCountErrors++;
                    printerr("Error_27109! Incorrect number of directories are return.. " + strDirs.Length);
                }
            }
            catch (IOException)
            {
            }
            catch (Exception exc)
            {
                iCountErrors++;
                printerr("Error_107gy! Incorrect exception thrown, exc==" + exc.ToString());
            }

            // [] Should return zero length array for an empty directory
            //-----------------------------------------------------------------
            strLoc = "Loc_4y982";

            dir2 = Directory.CreateDirectory(dirName);
            strArr = Directory.GetDirectories(dirName, "*");
            iCountTestcases++;
            if (strArr.Length != 0)
            {
                iCountErrors++;
                printerr("Error_207v7! Incorrect number of directories returned");
            }
            //-----------------------------------------------------------------


            // [] Create a directorystructure and check it
            //-----------------------------------------------------------------
            strLoc = "Loc_2398c";

            dir2.CreateSubdirectory("TestDir1");
            dir2.CreateSubdirectory("TestDir2");
            dir2.CreateSubdirectory("TestDir3");
            dir2.CreateSubdirectory("Test1Dir1");
            dir2.CreateSubdirectory("Test1Dir2");
            new FileInfo(dir2.ToString() + "TestFile1");
            new FileInfo(dir2.ToString() + "TestFile2");

            // [] SearchString ending with '*'

            iCountTestcases++;
            strArr = Directory.GetDirectories(Path.Combine(".", dirName), "TestDir*");
            iCountTestcases++;
            if (strArr.Length != 3)
            {
                iCountErrors++;
                printerr("Error_1yt75! Incorrect number of directories returned");
            }

            String[] names = new String[strArr.Length];
            int i = 0;
            foreach (String d in strArr)
                names[i++] = Path.GetFileName(d);

            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir1") < 0)
            {
                iCountErrors++;
                printerr("Error_3y775! Incorrect name==" + strArr[0].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_90885! Incorrect name==" + strArr[1].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir3") < 0)
            {
                iCountErrors++;
                printerr("Error_879by! Incorrect name==" + strArr[2].ToString());
            }


            // [] SearchString is '*'
            strLoc = "Loc_249yv";

            strArr = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), dirName), "*");
            iCountTestcases++;
            if (strArr.Length != 5)
            {
                iCountErrors++;
                printerr("Error_t5792! Incorrect number of directories==" + strArr.Length);
            }
            names = new String[strArr.Length];
            i = 0;
            foreach (String d in strArr)
                names[i++] = Path.GetFileName(d);

            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir1") < 0)
            {
                iCountErrors++;
                printerr("Error_4898v! Incorrect name==" + strArr[0].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir2") < 0)
            {
                iCountErrors++;
                printerr("Error_4598c! Incorrect name==" + strArr[1].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir1") < 0)
            {
                iCountErrors++;
                printerr("Error_209d8! Incorrect name==" + strArr[2].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_10vtu! Incorrect name==" + strArr[3].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir3") < 0)
            {
                iCountErrors++;
                printerr("Error_190vh! Incorrect name==" + strArr[4].ToString());
            }

            // [] SearchString starting with '*'
            strLoc = "Loc_20v99";

            strArr = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), dirName), "*Dir2");
            iCountTestcases++;
            if (strArr.Length != 2)
            {
                iCountErrors++;
                printerr("Error_8019x! Incorrect number of directories==" + strArr.Length);
            }
            names = new String[strArr.Length];
            i = 0;
            foreach (String d in strArr)
                names[i++] = Path.GetFileName(d);
            iCountTestcases++;
            if (Array.IndexOf(names, "Test1Dir2") < 0)
            {
                iCountErrors++;
                printerr("Error_167yb! Incorrect name==" + strArr[0].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_49yb7! Incorrect name==" + strArr[1].ToString());
            }


            // SearchString beginning and ending with '*'
            strLoc = "Loc_2498g";
            dir2.CreateSubdirectory("AAABB");
            dir2.CreateSubdirectory("aaabbcc");

            strArr = Directory.GetDirectories(Path.Combine(".", dirName), "*BB*");
            iCountTestcases++;
            if (strArr.Length != (Interop.IsWindows ? 2 : 1))
            {
                iCountErrors++;
                printerr("Error_4y190! Incorrect number of directories==" + strArr.Length);
            }
            names = new String[strArr.Length];
            i = 0;
            foreach (String d in strArr)
                names[i++] = Path.GetFileName(d);
            iCountTestcases++;
            if (Array.IndexOf(names, "AAABB") < 0)
            {
                iCountErrors++;
                printerr("Error_956yb! Incorrect name==" + strArr[0]);
            }
            if (Interop.IsWindows)
            {
                iCountTestcases++;
                if (Array.IndexOf(names, "aaabbcc") < 0)
                {
                    iCountErrors++;
                    printerr("Error_48yg7! Incorrect name==" + strArr[1]);
                }
            }

            // [] Should not search on fullpath
            // [] No matches should return empty array

            strArr = Directory.GetDirectories(Path.Combine(".", dirName), "Directory");
            iCountTestcases++;
            if (strArr.Length != 0)
            {
                iCountErrors++;
                printerr("Error_209v7! Incorrect number of directories==" + strArr.Length);
            }

            //Code coverage
            //Search pattern could have subdirectories
            strArr = Directory.GetDirectories(".", Path.Combine(dirName, "TestDir*"));
            if (strArr.Length != 3)
            {
                iCountErrors++;
                printerr("Error_1yt75! Incorrect number of directories returned");
            }

            names = new String[strArr.Length];
            i = 0;
            foreach (String d in strArr)
                names[i++] = Path.GetFileName(d);

            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir1") < 0)
            {
                iCountErrors++;
                printerr("Error_3y775! Incorrect name==" + strArr[0].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir2") < 0)
            {
                iCountErrors++;
                printerr("Error_90885! Incorrect name==" + strArr[1].ToString());
            }
            iCountTestcases++;
            if (Array.IndexOf(names, "TestDir3") < 0)
            {
                iCountErrors++;
                printerr("Error_879by! Incorrect name==" + strArr[2].ToString());
            }




            //-----------------------------------------------------------------



            if (Directory.Exists(dirName))
                Directory.Delete(dirName, true);

            ///////////////////////////////////////////////////////////////////
            /////////////////////////// END TESTS /////////////////////////////
        }
        catch (Exception exc_general)
        {
            ++iCountErrors;
            printerr("Error Err_8888yyy!  strLoc==" + strLoc + ", exc_general==" + exc_general.ToString());
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
