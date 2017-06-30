// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This test is machine generated. Please see the readme.md next to the test for more information. Please do not modify this test by hand.

using System;
using Xunit;
public class NSStatus
{
    [Fact]
    public static void StringTest()
    {
        bool temp;
        int errcount = 0;
        //*** System.String.Split-0x801fbe4aea4c2406-101 ***
        temp = false;
        // erid(0x8c85191e291f5266
        object Param_0_005d850a = (System.String)("SDK, desktop, application, exe, dll");
        string[] Local_0_1 = ((System.String)Param_0_005d850a).Split(new char[] {';'},(System.StringSplitOptions)1);
        string[] Expected_0 = new string[] {"SDK, desktop, application, exe, dll"};
        temp = (Expected_0.Length == Local_0_1.Length); for(int i=0;i<Expected_0.Length;i++){if(Expected_0[i] != Local_0_1[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 0 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x8dbf69acbf253e60
        object Param_1_0088961b = (System.String)("ToolsVersions\u005c12.0");
        string[] Local_1_1 = ((System.String)Param_1_0088961b).Split(new char[] {'\u005c'},(System.StringSplitOptions)1);
        string[] Expected_1 = new string[] {"ToolsVersions","12.0"};
        temp = (Expected_1.Length == Local_1_1.Length); for(int i=0;i<Expected_1.Length;i++){if(Expected_1[i] != Local_1_1[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 1 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.LastIndexOf-0x8188eff74bdaa0fb-101 ***
        temp = false;
        // erid(0x80fa43d487b85a96
        System.String Param_2_00b650f0 = (System.String)("Microsoft.Build.Framework.ITask");
        int Local_2_1 = Param_2_00b650f0.LastIndexOf(".",(System.StringComparison)4);
        string Local_2_2 = Param_2_00b650f0.Substring(0,25);
        string Local_2_3 = Param_2_00b650f0.Substring(26,5);
        int Local_2_4 = Param_2_00b650f0.LastIndexOf(".",(System.StringComparison)4);
        string Local_2_5 = Param_2_00b650f0.Substring(0,25);
        string Local_2_6 = Param_2_00b650f0.Substring(26,5);
        int Local_2_7 = Param_2_00b650f0.LastIndexOf(".",30,31,(System.StringComparison)4);
        int Expected_2 = 25;
        temp = (Expected_2 == Local_2_7);
        if(!temp) { Console.WriteLine(@"FAILED -> 2 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.IndexOf-0x8dc4331296d88a6e-101 ***
        temp = false;
        // erid(0x803b48d802354f69
        System.String Param_3_0097a72a = (System.String)("SOFTWARE\u005cMICROSOFT\u005cMicrosoft SDKs\u005c\u005cSilverlight\u005cv5.0");
        int Local_3_1 = Param_3_0097a72a.IndexOf("\u005c",(System.StringComparison)5);
        int Local_3_2 = Param_3_0097a72a.IndexOf("\u005c",9,(System.StringComparison)5);
        int Local_3_3 = Param_3_0097a72a.IndexOf("\u005c",19,(System.StringComparison)5);
        int Local_3_4 = Param_3_0097a72a.IndexOf("\u005c",34,(System.StringComparison)5);
        int Local_3_5 = Param_3_0097a72a.IndexOf("\u005c",35,(System.StringComparison)5);
        int Expected_3 = 46;
        temp = (Expected_3 == Local_3_5);
        if(!temp) { Console.WriteLine(@"FAILED -> 3 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x804ca7b5a862a6de
        System.String Param_4_0029dd0b = (System.String)("{A693A243-4743-4034-AED4-BEC4E79E0B3B}\u005cVisibility");
        int Local_4_1 = Param_4_0029dd0b.IndexOf("\u005c",(System.StringComparison)5);
        int Local_4_2 = Param_4_0029dd0b.IndexOf("\u005c",39,(System.StringComparison)5);
        int Expected_4 = -1;
        temp = (Expected_4 == Local_4_2);
        if(!temp) { Console.WriteLine(@"FAILED -> 4 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.LastIndexOf-0x8ea938b3c044193f-101 ***
        temp = false;
        // erid(0x4af20f9bd0cf9d6a
        System.String Param_5_0002608f = (System.String)("HKEY_CURRENT_USER\u005cSoftware\u005cMicrosoft\u005cVisualStudio\u005c14.0");
        int Local_5_1 = Param_5_0002608f.LastIndexOf('\u005c');
        int Local_5_2 = Param_5_0002608f.LastIndexOf('\u005c',48);
        int Expected_5 = 36;
        temp = (Expected_5 == Local_5_2);
        if(!temp) { Console.WriteLine(@"FAILED -> 5 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Split-0x927835cb5a818192-101 ***
        temp = false;
        // erid(0x802a457b9f0f2069
        object Param_6_005dcbf6 = (System.String)("?LinkID=785123&clcid=0x409");
        string[] Local_6_1 = ((System.String)Param_6_005dcbf6).Split(new char[] {'?'});
        string[] Expected_6 = new string[] {"","LinkID=785123&clcid=0x409"};
        temp = (Expected_6.Length == Local_6_1.Length); for(int i=0;i<Expected_6.Length;i++){if(Expected_6[i] != Local_6_1[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 6 "); errcount++;}
        // //Summary 1-classes 1-modules 

        //*** System.String.StartsWith-0x9ea19cc412fad033-101 ***
        temp = false;
        // erid(0x8017a589f692a1df
        System.String Param_8_00b31ed2 = (System.String)("C:\u005cProgram Files (x86)\u005cReference Assemblies\u005cMicrosoft\u005cFramework\u005c.NETPortable\u005cv4.0\u005cProfile\u005cProfile41\u005cRedistList\u005cFrameworkList.xml");
        string Local_8_1 = Param_8_00b31ed2.TrimEnd(new char[] {'\u0009','\u000a','\u000b','\u000c','\u000d',' ','\u0085','\u00a0'});
        int Local_8_2 = Param_8_00b31ed2.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_8_3 = Param_8_00b31ed2.IndexOf(':',2);
        Param_8_00b31ed2.CopyTo(0,new char[] {'f','i','l','e',':','/','/','/','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000','\u0000'},8,128);
        string Local_8_5 = Param_8_00b31ed2.ToString();
        bool Local_8_6 = Param_8_00b31ed2.StartsWith("analyzers");
        bool Expected_8 = false;
        temp = (Expected_8 == Local_8_6);
        if(!temp) { Console.WriteLine(@"FAILED -> 8 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x803a032f0ba6c9a0
        object Param_9_0089614c = (System.String)("C# Interactive");
        bool Local_9_1 = ((System.String)Param_9_0089614c).StartsWith("{}");
        bool Expected_9 = false;
        temp = (Expected_9 == Local_9_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 9 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.IndexOf-0xa53d09a6c66a32a6-101 ***
        temp = false;
        // erid(0x8055e210c564e003
        object Param_10_002e0c7c = (System.String)("Microsoft.CustomDocWell.Options.DependencyObjectType");
        int Local_10_1 = ((System.String)Param_10_002e0c7c).IndexOf("*");
        int Expected_10 = -1;
        temp = (Expected_10 == Local_10_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 10 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Join-0xaa0dee53d20d38dd-101 ***
        temp = false;
        // erid(0x819be676d5f46a2d
        string Local_11_0 = System.String.Join(",",new string[] {"1aab8aee-afba-4c01-b444-0a56dc17792f","14.0"},0,2);
        string Expected_11 = "1aab8aee-afba-4c01-b444-0a56dc17792f,14.0";
        temp = (Expected_11 == Local_11_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 11 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x8474c4cd9284b73c
        string Local_12_0 = System.String.Join(";",new string[] {"GlobalDesignTimeResolveAssemblyReferences"},0,1);
        string Expected_12 = "GlobalDesignTimeResolveAssemblyReferences";
        temp = (Expected_12 == Local_12_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 12 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Split-0xb14abcf8170b4a71-101 ***
        temp = false;
        // erid(0x81cb6341b16e23cd
        System.String Param_13_00c1a7ba = (System.String)("ST:0:0:{be4d7042-ba3f-11d2-840e-00c04f9902c1}");
        bool Local_13_1 = Param_13_00c1a7ba.StartsWith("{}");
        bool Local_13_2 = Param_13_00c1a7ba.StartsWith("{");
        int Local_13_3 = Param_13_00c1a7ba.GetHashCode();
        string[] Local_13_4 = Param_13_00c1a7ba.Split(new char[] {':'},4);
        string[] Expected_13 = new string[] {"ST","0","0","{be4d7042-ba3f-11d2-840e-00c04f9902c1}"};
        temp = (Expected_13.Length == Local_13_4.Length); for(int i=0;i<Expected_13.Length;i++){if(Expected_13[i] != Local_13_4[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 13 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.IndexOfAny-0xc5d60c835a76eee9-101 ***
        temp = false;
        // erid(0x800300ebc4d470e1
        System.String Param_14_0099465d = (System.String)("C:\u005cProgram Files (x86)\u005cMSBuild\u005cMicrosoft\u005cVisualStudio\u005cv14.0\u005cTeamTest\u005cMicrosoft.TeamTest.targets");
        string Local_14_1 = Param_14_0099465d.Trim();
        string Local_14_2 = Param_14_0099465d.Substring(0,95);
        string Local_14_3 = Param_14_0099465d.Trim();
        int Local_14_4 = Param_14_0099465d.IndexOf('%');
        int Local_14_5 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_14_6 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_14_7 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        string Local_14_8 = Param_14_0099465d.Trim();
        int Local_14_9 = Param_14_0099465d.IndexOf('%');
        int Local_14_10 = Param_14_0099465d.IndexOfAny(new char[] {'*','?'});
        int Local_14_11 = Param_14_0099465d.IndexOf('%');
        int Local_14_12 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_14_13 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        bool Local_14_14 = Param_14_0099465d.StartsWith("\u005c\u005c",(System.StringComparison)4);
        int Local_14_15 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_14_16 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_14_17 = Param_14_0099465d.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Expected_14 = -1;
        temp = (Expected_14 == Local_14_17);
        if(!temp) { Console.WriteLine(@"FAILED -> 14 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Join-0xcb474e8bc3331911-101 ***
        temp = false;
        // erid(0x8037c9b30b5df943
        string Local_15_0 = System.String.Join(",",new string[] {"Xamarin.VisualStudio.Breadcrumb","1.0.0.0"});
        string Expected_15 = "Xamarin.VisualStudio.Breadcrumb,1.0.0.0";
        temp = (Expected_15 == Local_15_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 15 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.GetTypeCode-0xcf7bb67fdd502e92-101 ***
        temp = false;
        // erid(0xfa3367af1aeaebcd
        System.String Param_16_0177d744 = (System.String)("Start Page");
        bool Local_16_1 = Param_16_0177d744.Contains(" -");
        bool Local_16_2 = Param_16_0177d744.Contains("- ");
        System.TypeCode Local_16_3 = Param_16_0177d744.GetTypeCode();
        System.TypeCode Expected_16 = (System.TypeCode)18;
        temp = (Expected_16 == Local_16_3);
        if(!temp) { Console.WriteLine(@"FAILED -> 16 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Trim-0xd6901e849c199f6d-101 ***
        temp = false;
        // erid(0x827e8c93e55efe6a
        System.String Param_17_00a1170c = (System.String)("HKEY_CURRENT_USER\u005cSoftware\u005cMicrosoft\u005cWisp\u005c");
        string Local_17_1 = Param_17_00a1170c.Replace('/','\u005c');
        string[] Local_17_2 = Param_17_00a1170c.Split(new char[] {';'});
        bool Local_17_3 = Param_17_00a1170c.Equals("");
        string Local_17_4 = Param_17_00a1170c.Trim(new char[] {' '});
        int Local_17_5 = Param_17_00a1170c.IndexOf('\u0000');
        bool Local_17_6 = Param_17_00a1170c.Equals("");
        string Local_17_7 = Param_17_00a1170c.Replace('/','\u005c');
        string[] Local_17_8 = Param_17_00a1170c.Split(new char[] {';'});
        bool Local_17_9 = Param_17_00a1170c.Equals("");
        string Local_17_10 = Param_17_00a1170c.Trim(new char[] {' '});
        int Local_17_11 = Param_17_00a1170c.IndexOf('\u0000');
        bool Local_17_12 = Param_17_00a1170c.Equals("");
        string Local_17_13 = Param_17_00a1170c.Replace('/','\u005c');
        string[] Local_17_14 = Param_17_00a1170c.Split(new char[] {';'});
        bool Local_17_15 = Param_17_00a1170c.Equals("");
        string Local_17_16 = Param_17_00a1170c.Trim(new char[] {' '});
        int Local_17_17 = Param_17_00a1170c.IndexOf('\u0000');
        bool Local_17_18 = Param_17_00a1170c.Equals("");
        string Local_17_19 = Param_17_00a1170c.Replace('/','\u005c');
        string[] Local_17_20 = Param_17_00a1170c.Split(new char[] {';'});
        bool Local_17_21 = Param_17_00a1170c.Equals("");
        string Local_17_22 = Param_17_00a1170c.Trim(new char[] {' '});
        int Local_17_23 = Param_17_00a1170c.IndexOf('\u0000');
        bool Local_17_24 = Param_17_00a1170c.Equals("");
        string Local_17_25 = Param_17_00a1170c.Replace('/','\u005c');
        string[] Local_17_26 = Param_17_00a1170c.Split(new char[] {';'});
        bool Local_17_27 = Param_17_00a1170c.Equals("");
        string Local_17_28 = Param_17_00a1170c.Trim(new char[] {' '});
        string Expected_17 = "HKEY_CURRENT_USER\u005cSoftware\u005cMicrosoft\u005cWisp\u005c";
        temp = (Expected_17 == Local_17_28);
        if(!temp) { Console.WriteLine(@"FAILED -> 17 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Substring-0xdaa4e6a1a57d6838-101 ***
        temp = false;
        // erid(0x80291d618e73da28
        System.String Param_18_0018ba45 = (System.String)("ui/styles/errortextboxstyle.xaml");
        bool Local_18_1 = Param_18_0018ba45.StartsWith("",(System.StringComparison)5);
        string Local_18_2 = Param_18_0018ba45.Substring(0);
        string Expected_18 = "ui/styles/errortextboxstyle.xaml";
        temp = (Expected_18 == Local_18_2);
        if(!temp) { Console.WriteLine(@"FAILED -> 18 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Intern-0xdfdde1b5beef5fa3-101 ***
        temp = false;
        // erid(0x84d81aff0207e26c
        string Local_19_0 = System.String.Intern("CheckMark");
        string Expected_19 = "CheckMark";
        temp = (Expected_19 == Local_19_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 19 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.ToLowerInvariant-0xe833c58cd9aeba57-101 ***
        temp = false;
        // erid(0x802c234705c5f70a
        System.String Param_20_0100e194 = (System.String)("13");
        string Local_20_1 = Param_20_0100e194.Trim();
        string Local_20_2 = Param_20_0100e194.ToLowerInvariant();
        string Expected_20 = "13";
        temp = (Expected_20 == Local_20_2);
        if(!temp) { Console.WriteLine(@"FAILED -> 20 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Concat-0xee8cdda89e79b5d4-101 ***
        temp = false;
        // erid(0x800ed221506197bb
        string Local_21_0 = System.String.Concat("C:\u005cPROGRAM FILES (X86)\u005cMICROSOFT VISUAL STUDIO 14.0\u005cCOMMON7\u005cIDE\u005cEXTENSIONS\u005cMicrosoft\u005cArchitecture Tools\u005cComponents","\u005c");
        string Expected_21 = "C:\u005cPROGRAM FILES (X86)\u005cMICROSOFT VISUAL STUDIO 14.0\u005cCOMMON7\u005cIDE\u005cEXTENSIONS\u005cMicrosoft\u005cArchitecture Tools\u005cComponents\u005c";
        temp = (Expected_21 == Local_21_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 21 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x8024601f51a599a6
        string Local_22_0 = System.String.Concat("Create","Microsoft.VisualStudio.Services.WebApi.Jwt.JsonWebToken");
        string Expected_22 = "CreateMicrosoft.VisualStudio.Services.WebApi.Jwt.JsonWebToken";
        temp = (Expected_22 == Local_22_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 22 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x802af48b23e13fed
        string Local_23_0 = System.String.Concat("14","_T");
        string Expected_23 = "14_T";
        temp = (Expected_23 == Local_23_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 23 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Substring-0xf32ff074e735eecf-101 ***
        temp = false;
        // erid(0x800019483a8f95d2
        System.String Param_24_009707b0 = (System.String)("C:\u005cProgram Files (x86)\u005cMicrosoft SDKs\u005cClickOnce Bootstrapper\u005cPackages");
        int Local_24_1 = Param_24_009707b0.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        string Local_24_2 = Param_24_009707b0.Substring(61,8);
        int Local_24_3 = Param_24_009707b0.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        string Local_24_4 = Param_24_009707b0.Substring(61,8);
        string Expected_24 = "Packages";
        temp = (Expected_24 == Local_24_4);
        if(!temp) { Console.WriteLine(@"FAILED -> 24 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.IndexOf-0xf391f336776becfb-101 ***
        temp = false;
        // erid(0x8020eeb8b75d49ed
        object Param_25_002ad960 = (System.String)("SolutionExistsAndNotBuildingAndNotDebugging & SolutionExistsAndFullyLoaded & (SolutionHasMultipleProjects | SolutionHasSingleProject)");
        int Local_25_1 = ((System.String)Param_25_002ad960).IndexOf("SolutionExistsAndNotBuildingAndNotDebugging",(System.StringComparison)5);
        int Expected_25 = 0;
        temp = (Expected_25 == Local_25_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 25 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Equals-0xf8ea3983e08a5940-101 ***
        temp = false;
        // erid(0x8000fd5f49fd9d96
        System.String Param_26_002e4c4f = (System.String)("Microsoft.CustomDocWell.Options.RenderOptions.BitmapScalingMode");
        int Local_26_1 = Param_26_002e4c4f.IndexOf("*");
        bool Local_26_2 = Param_26_002e4c4f.Equals("KeybindingScheme",(System.StringComparison)5);
        bool Local_26_3 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.Azure.CommonAzureTools.ActiveAccountList",(System.StringComparison)5);
        bool Local_26_4 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.ColorTheme",(System.StringComparison)5);
        bool Local_26_5 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.CommandAliases",(System.StringComparison)5);
        bool Local_26_6 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.DefaultCommandAliases",(System.StringComparison)5);
        bool Local_26_7 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.IDE.OnEnvironmentStartup",(System.StringComparison)5);
        bool Local_26_8 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.IDE.StartPage.IsDownloadRefreshEnabled",(System.StringComparison)5);
        bool Local_26_9 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.IDE.StartPage.NewsFeed",(System.StringComparison)5);
        bool Local_26_10 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.IDE.StartPage.RefreshInterval",(System.StringComparison)5);
        bool Local_26_11 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.Platform.TitleCaseMenus",(System.StringComparison)5);
        bool Local_26_12 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.Platform.WindowManagement.Layouts.WindowLayoutInfoList",(System.StringComparison)5);
        bool Local_26_13 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.Platform.WindowManagement.Layouts.WindowLayoutSkipApplyConfirmation",(System.StringComparison)5);
        bool Local_26_14 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.RoamingEnabled",(System.StringComparison)5);
        bool Local_26_15 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Formatting.AutoReformatOnEndTag",(System.StringComparison)5);
        bool Local_26_16 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Formatting.AutoReformatOnPaste",(System.StringComparison)5);
        bool Local_26_17 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Formatting.AutoReformatOnStartTag",(System.StringComparison)5);
        bool Local_26_18 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Formatting.QuoteStyle",(System.StringComparison)5);
        bool Local_26_19 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Formatting.WrapColumn",(System.StringComparison)5);
        bool Local_26_20 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Formatting.WrapTags",(System.StringComparison)5);
        bool Local_26_21 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Miscellaneous.AutoInsertAttributeQuotes",(System.StringComparison)5);
        bool Local_26_22 = Param_26_002e4c4f.Equals("Microsoft.VisualStudio.TextEditor.Xaml.Miscellaneous.AutoInsertCommas",(System.StringComparison)5);
        bool Expected_26 = false;
        temp = (Expected_26 == Local_26_22);
        if(!temp) { Console.WriteLine(@"FAILED -> 26 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.IndexOfAny-0xfbf5aa14142d11d2-101 ***
        temp = false;
        // erid(0x81429edeafd60532
        System.String Param_27_00937e9b = (System.String)("C:\u005cProgram Files (x86)\u005cMSBuild\u005cMicrosoft.Cpp\u005cv4.0\u005cV140\u005c");
        int Local_27_1 = Param_27_00937e9b.IndexOf('%');
        int Local_27_2 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_3 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Local_27_4 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},18);
        int Local_27_5 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},22);
        int Local_27_6 = Param_27_00937e9b.IndexOf('%');
        int Local_27_7 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_8 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Local_27_9 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},18);
        int Local_27_10 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},22);
        int Local_27_11 = Param_27_00937e9b.IndexOf('%');
        int Local_27_12 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_13 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Local_27_14 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},18);
        int Local_27_15 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},22);
        int Local_27_16 = Param_27_00937e9b.IndexOf('%');
        int Local_27_17 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_18 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Local_27_19 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},18);
        int Local_27_20 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},22);
        int Local_27_21 = Param_27_00937e9b.IndexOf('%');
        int Local_27_22 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_23 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Local_27_24 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},18);
        int Local_27_25 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},22);
        int Local_27_26 = Param_27_00937e9b.IndexOf('%');
        int Local_27_27 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_28 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Local_27_29 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},18);
        int Local_27_30 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},22);
        int Local_27_31 = Param_27_00937e9b.IndexOf('%');
        int Local_27_32 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'});
        int Local_27_33 = Param_27_00937e9b.IndexOfAny(new char[] {'%','*','?','@','$','(',')',';','\u0027'},0);
        int Expected_27 = 17;
        temp = (Expected_27 == Local_27_33);
        if(!temp) { Console.WriteLine(@"FAILED -> 27 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Replace-0xfdf6d6b173f5f977-101 ***
        temp = false;
        // erid(0x80c774ab39ff9fff
        object Param_28_01407b44 = (System.String)("/Png/Output.16.16.png");
        string Local_28_1 = ((System.String)Param_28_01407b44).Replace("#","%23");
        string Expected_28 = "/Png/Output.16.16.png";
        temp = (Expected_28 == Local_28_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 28 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Contains-0x044a768f2017ea43-101 ***
        temp = false;
        // erid(0x80f5aa701e38478d
        object Param_29_003e4d4f = (System.String)("/dependencyList/F4F37C32.bin");
        bool Local_29_1 = ((System.String)Param_29_003e4d4f).Contains("#");
        bool Expected_29 = false;
        temp = (Expected_29 == Local_29_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 29 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Compare-0x09fee4d7151479d8-101 ***
        temp = false;
        // erid(0xc0d28ffd89d9669b
        int Local_30_0 = System.String.Compare("Enterprise","Ultimate",true);
        int Expected_30 = -1;
        temp = (Expected_30 == Local_30_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 30 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x06ea192be96f19c7
        int Local_31_0 = System.String.Compare("Enterprise","Enterprise",true);
        int Expected_31 = 0;
        temp = (Expected_31 == Local_31_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 31 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Trim-0x0d87a08c61a08837-101 ***
        temp = false;
        // erid(0x8007333fcbd52f9f
        object Param_32_0003690a = (System.String)("{72F0B33F-F6D5-47E0-B81C-0ED36BF9D6C7}");
        string Local_32_1 = ((System.String)Param_32_0003690a).Trim();
        string Expected_32 = "{72F0B33F-F6D5-47E0-B81C-0ED36BF9D6C7}";
        temp = (Expected_32 == Local_32_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 32 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Format-0x1436fc3e9a14f75c-101 ***
        temp = false;
        // erid(0x84babdd4f689ec55
        object Param_33_017696fa = (System.String)("LayoutInfo");
        string Local_33_1 = System.String.Format("VS/Internal/StartPage/{0}",Param_33_017696fa);
        string Expected_33 = "VS/Internal/StartPage/LayoutInfo";
        temp = (Expected_33 == Local_33_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 33 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x8d21bdd510013035
        System.String Param_34_017692da = (System.String)("KeepPageOpenAfterOpenProject");
        int Local_34_1 = Param_34_017692da.GetHashCode();
        int Local_34_2 = Param_34_017692da.GetHashCode();
        string Local_34_3 = System.String.Format("VS/Internal/StartPage/{0}",(object)Param_34_017692da);
        string Expected_34 = "VS/Internal/StartPage/KeepPageOpenAfterOpenProject";
        temp = (Expected_34 == Local_34_3);
        if(!temp) { Console.WriteLine(@"FAILED -> 34 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Format-0x1436fc3e9a14f75c-201 ***
        temp = false;
        // erid(0x8a3a601f899ee52c
        object Param_35_00016258 = (System.UInt32)((uint)4294303411);
        string Local_35_1 = System.String.Format("#{0,8:X8}",Param_35_00016258);
        string Expected_35 = "#FFF5DEB3";
        temp = (Expected_35 == Local_35_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 35 "); errcount++;}
        // //Summary 2-classes 1-modules 


        //*** System.String.Concat-0x1b211a96d41c6bec-201 ***
        temp = false;
        // erid(0x83d4b2dde212161b
        System.String Param_36_01a938ce = (System.String)("2vPuLSLDC0WiIheNiUj0OA:");
        string Local_36_1 = Param_36_01a938ce.ToString();
        string Local_36_2 = Param_36_01a938ce.ToString();
        string Local_36_3 = Param_36_01a938ce.ToString();
        string Local_36_4 = Param_36_01a938ce.ToString();
        string Local_36_5 = Param_36_01a938ce.ToString();
        string Local_36_6 = Param_36_01a938ce.ToString();
        string Local_36_7 = Param_36_01a938ce.ToString();
        string Local_36_8 = Param_36_01a938ce.ToString();
        string Local_36_9 = Param_36_01a938ce.ToString();
        string Local_36_10 = Param_36_01a938ce.ToString();
        string Local_36_11 = Param_36_01a938ce.ToString();
        string Local_36_12 = Param_36_01a938ce.ToString();
        string Local_36_13 = Param_36_01a938ce.ToString();
        string Local_36_14 = Param_36_01a938ce.ToString();
        string Local_36_15 = Param_36_01a938ce.ToString();
        string Local_36_16 = Param_36_01a938ce.ToString();
        string Local_36_17 = Param_36_01a938ce.ToString();
        string Local_36_18 = Param_36_01a938ce.ToString();
        string Local_36_19 = Param_36_01a938ce.ToString();
        string Local_36_20 = Param_36_01a938ce.ToString();
        string Local_36_21 = Param_36_01a938ce.ToString();
        string Local_36_22 = Param_36_01a938ce.ToString();
        string Local_36_23 = Param_36_01a938ce.ToString();
        string Local_36_24 = Param_36_01a938ce.ToString();
        string Local_36_25 = Param_36_01a938ce.ToString();
        string Local_36_26 = Param_36_01a938ce.ToString();
        string Local_36_27 = Param_36_01a938ce.ToString();
        string Local_36_28 = Param_36_01a938ce.ToString();
        string Local_36_29 = Param_36_01a938ce.ToString();
        string Local_36_30 = Param_36_01a938ce.ToString();
        string Local_36_31 = Param_36_01a938ce.ToString();
        string Local_36_32 = Param_36_01a938ce.ToString();
        string Local_36_33 = Param_36_01a938ce.ToString();
        string Local_36_34 = Param_36_01a938ce.ToString();
        string Local_36_35 = Param_36_01a938ce.ToString();
        string Local_36_36 = Param_36_01a938ce.ToString();
        string Local_36_37 = Param_36_01a938ce.ToString();
        string Local_36_38 = Param_36_01a938ce.ToString();
        string Local_36_39 = Param_36_01a938ce.ToString();
        string Local_36_40 = Param_36_01a938ce.ToString();
        string Local_36_41 = Param_36_01a938ce.ToString();
        string Local_36_42 = Param_36_01a938ce.ToString();
        string Local_36_43 = Param_36_01a938ce.ToString();
        string Local_36_44 = Param_36_01a938ce.ToString();
        string Local_36_45 = Param_36_01a938ce.ToString();
        string Local_36_46 = Param_36_01a938ce.ToString();
        string Local_36_47 = Param_36_01a938ce.ToString();
        object Param_36_01a95189 = (System.Int64)((long)48);
        string Local_36_49 = System.String.Concat((object)Param_36_01a938ce,Param_36_01a95189);
        string Expected_36 = "2vPuLSLDC0WiIheNiUj0OA:48";
        temp = (Expected_36 == Local_36_49);
        if(!temp) { Console.WriteLine(@"FAILED -> 36 "); errcount++;}
        // //Summary 2-classes 1-modules 


        //*** System.String.StartsWith-0x1bb47fdfed6e7448-101 ***
        temp = false;
        // erid(0x800677389512f4bd
        object Param_37_00183ced = (System.String)("/Microsoft.VisualStudio.ImageCatalog;Component//Xaml/SilverlightFolderOpened.xaml");
        bool Local_37_1 = ((System.String)Param_37_00183ced).StartsWith("pack",(System.StringComparison)5);
        bool Expected_37 = false;
        temp = (Expected_37 == Local_37_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 37 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.IndexOf-0x1df24fa2dd8b556f-101 ***
        temp = false;
        // erid(0x80073c26a313a2f1
        object Param_38_00035f13 = (System.String)("TextEditor.JavaScript.EsLint.*");
        int Local_38_1 = ((System.String)Param_38_00035f13).IndexOf("\u005c",0,30,(System.StringComparison)5);
        int Expected_38 = -1;
        temp = (Expected_38 == Local_38_1);
        if(!temp) { Console.WriteLine(@"FAILED -> 38 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.TrimEnd-0x23ad85e08adb8af7-101 ***
        temp = false;
        // erid(0x801312c432089e5f
        System.String Param_39_002b0342 = (System.String)("C:\u005cPROGRAM FILES (X86)\u005cMICROSOFT VISUAL STUDIO 14.0\u005cCOMMON7\u005cIDE\u005cEXTENSIONS\u005cWWNAJPN3.F2I\u005cMicrosoft.VisualStudio.Services.Client.dll");
        string Local_39_1 = Param_39_002b0342.TrimEnd(new char[] {'\u0009','\u000a','\u000b','\u000c','\u000d',' ','\u0085','\u00a0'});
        int Local_39_2 = Param_39_002b0342.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        int Local_39_3 = Param_39_002b0342.IndexOf(':',2);
        string Local_39_4 = Param_39_002b0342.TrimEnd(new char[] {'\u0009','\u000a','\u000b','\u000c','\u000d',' ','\u0085','\u00a0'});
        int Local_39_5 = Param_39_002b0342.IndexOfAny(new char[] {'\u0022','<','>','|','\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000a','\u000b','\u000c','\u000d','\u000e','\u000f','\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'});
        string Local_39_6 = Param_39_002b0342.Replace('/','\u005c');
        string Local_39_7 = Param_39_002b0342.Trim(new char[] {' '});
        int Local_39_8 = Param_39_002b0342.IndexOf('\u0000');
        string Local_39_9 = Param_39_002b0342.TrimEnd(new char[] {'\u0009','\u000a','\u000b','\u000c','\u000d',' ','\u0085','\u00a0'});
        string Expected_39 = "C:\u005cPROGRAM FILES (X86)\u005cMICROSOFT VISUAL STUDIO 14.0\u005cCOMMON7\u005cIDE\u005cEXTENSIONS\u005cWWNAJPN3.F2I\u005cMicrosoft.VisualStudio.Services.Client.dll";
        temp = (Expected_39 == Local_39_9);
        if(!temp) { Console.WriteLine(@"FAILED -> 39 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Equals-0x2719f722a9db418b-101 ***
        temp = false;
        // erid(0x800883910e753cd5
        bool Local_40_0 = System.String.Equals("TextEditor.CSharp.Line Numbers","TextEditor.TypeScript.TsLint.*");
        bool Expected_40 = false;
        temp = (Expected_40 == Local_40_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 40 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Equals-0x31099e767f1b38d4-101 ***
        temp = false;
        // erid(0x8001607ca0cdd143
        bool Local_42_0 = System.String.Equals("Microsoft.VisualStudio.Services.Location.AccessMapping","System.Data.Linq.Binary",(System.StringComparison)4);
        bool Expected_42 = false;
        temp = (Expected_42 == Local_42_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 42 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Concat-0x3335d27d98f1de45-101 ***
        temp = false;
        // erid(0x800a8da063ff1e68
        string Local_43_0 = System.String.Concat("ToolWindows\u005c","{5A4E9529-B6A0-46B5-BE4F-0F0B239BC0EB}","\u005cVisibility");
        string Expected_43 = "ToolWindows\u005c{5A4E9529-B6A0-46B5-BE4F-0F0B239BC0EB}\u005cVisibility";
        temp = (Expected_43 == Local_43_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 43 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Split-0x3c3fb92dff1cfaaa-101 ***
        temp = false;
        // erid(0x441f29a6f9de0fea
        object Param_44_01eee9c8 = (System.String)("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/:::https://management.core.windows.net/:::872cd9fa-d31f-45e0-9eab-6e460a02d1f1:::0");
        string[] Local_44_1 = ((System.String)Param_44_01eee9c8).Split(new string[] {":::"},(System.StringSplitOptions)0);
        string[] Expected_44 = new string[] {"https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/","https://management.core.windows.net/","872cd9fa-d31f-45e0-9eab-6e460a02d1f1","0"};
        temp = (Expected_44.Length == Local_44_1.Length); for(int i=0;i<Expected_44.Length;i++){if(Expected_44[i] != Local_44_1[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 44 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.CompareOrdinal-0x43089b4b08d0bb5c-101 ***
        temp = false;
        // erid(0x8019f2f531fbbc2a
        int Local_45_0 = System.String.CompareOrdinal("/MANIFEST/E68B206.BIN","/MANIFEST/E68B206.BIN");
        int Expected_45 = 0;
        temp = (Expected_45 == Local_45_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 45 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x801fd889611f040e
        int Local_46_0 = System.String.CompareOrdinal("/MANIFEST/D24C86CC.BIN","/MANIFEST/D15B2D4D.BIN");
        int Expected_46 = 1;
        temp = (Expected_46 == Local_46_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 46 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x8026cf46a5895ffe
        int Local_47_0 = System.String.CompareOrdinal("/MICROSOFT.VISUALSTUDIO.PLATFORM.WINDOWMANAGEMENT;COMPONENT/THEMES/DOCKGROUPADORNERSTYLE.XAML","/MICROSOFT.VISUALSTUDIO.PLATFORM.WINDOWMANAGEMENT;COMPONENT/THEMES/COMMONCONTROLSCHECKBOXSTYLE.XAML");
        int Expected_47 = 1;
        temp = (Expected_47 == Local_47_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 47 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x80615c664cbdbef2
        int Local_48_0 = System.String.CompareOrdinal("/MANIFEST/5D93F84F.BIN","/MANIFEST/9855CE92.BIN");
        int Expected_48 = -4;
        temp = (Expected_48 == Local_48_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 48 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.ToCharArray-0x4f242e67b7762ce6-101 ***
        temp = false;
        // erid(0x8d088a4ae52c095a
        object Param_49_01897f03 = (System.String)("ttnopnlopmmmh102");
        char[] Local_49_1 = ((System.String)Param_49_01897f03).ToCharArray();
        char[] Expected_49 = new char[] {'t','t','n','o','p','n','l','o','p','m','m','m','h','1','0','2'};
        temp = (Expected_49.Length == Local_49_1.Length); for(int i=0;i<Expected_49.Length;i++){if(Expected_49[i] != Local_49_1[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 49 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Concat-0x523df8405d9ee9ee-101 ***
        temp = false;
        // erid(0x84201beaeee3c96f
        string Local_50_0 = System.String.Concat("Write","4","_","InstallationTarget");
        string Expected_50 = "Write4_InstallationTarget";
        temp = (Expected_50 == Local_50_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 50 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x87fa48380d6bf16d
        string Local_51_0 = System.String.Concat("","http://schemas.microsoft.com/developer/vsx-schema/2011",":","PackageManifest");
        string Expected_51 = "http://schemas.microsoft.com/developer/vsx-schema/2011:PackageManifest";
        temp = (Expected_51 == Local_51_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 51 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.CompareOrdinal-0x621dbc7fe754a3b1-101 ***
        temp = false;
        // erid(0x8126a0016b36709f
        int Local_52_0 = System.String.CompareOrdinal("comments",0,"comments",0,8);
        int Expected_52 = 0;
        temp = (Expected_52 == Local_52_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 52 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x81a382700ef24b43
        int Local_53_0 = System.String.CompareOrdinal("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/:::https://management.core.windows.net/:::872cd9fa-d31f-45e0-9eab-6e460a02d1f1:::0",71,":::",0,3);
        int Expected_53 = 0;
        temp = (Expected_53 == Local_53_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 53 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x84497b48f7e8f892
        int Local_54_0 = System.String.CompareOrdinal("$(",0,"$(TargetPlatformIdentifier)",0,2);
        int Expected_54 = 0;
        temp = (Expected_54 == Local_54_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 54 "); errcount++;}
        // //Summary 1-classes 1-modules 

        temp = false;
        // erid(0x87673c7ad57a0b11
        int Local_55_0 = System.String.CompareOrdinal("$(",0,"Debug",0,2);
        int Expected_55 = -32;
        temp = (Expected_55 == Local_55_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 55 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Format-0x71ea9d81c3834393-101 ***
        temp = false;
        // erid(0x82ceb767497663a2
        System.String Param_56_00046df5 = (System.String)("Themes\u005c{de3dbbcd-f642-433c-8353-8f1df4370aba}");
        string Local_56_1 = Param_56_00046df5.ToString();
        string Local_56_2 = Param_56_00046df5.ToString();
        string Local_56_3 = Param_56_00046df5.ToString();
        string Local_56_4 = Param_56_00046df5.ToString();
        string Local_56_5 = Param_56_00046df5.ToString();
        string Local_56_6 = Param_56_00046df5.ToString();
        string Local_56_7 = Param_56_00046df5.ToString();
        string Local_56_8 = Param_56_00046df5.ToString();
        string Local_56_9 = Param_56_00046df5.ToString();
        string Local_56_10 = Param_56_00046df5.ToString();
        string Local_56_11 = Param_56_00046df5.ToString();
        string Local_56_12 = Param_56_00046df5.ToString();
        string Local_56_13 = Param_56_00046df5.ToString();
        string Local_56_14 = Param_56_00046df5.ToString();
        string Local_56_15 = Param_56_00046df5.ToString();
        string Local_56_16 = Param_56_00046df5.ToString();
        string Local_56_17 = Param_56_00046df5.ToString();
        string Local_56_18 = Param_56_00046df5.ToString();
        string Local_56_19 = Param_56_00046df5.ToString();
        string Local_56_20 = Param_56_00046df5.ToString();
        string Local_56_21 = Param_56_00046df5.ToString();
        string Local_56_22 = Param_56_00046df5.ToString();
        string Local_56_23 = Param_56_00046df5.ToString();
        string Local_56_24 = Param_56_00046df5.ToString();
        string Local_56_25 = Param_56_00046df5.ToString();
        string Local_56_26 = Param_56_00046df5.ToString();
        string Local_56_27 = Param_56_00046df5.ToString();
        string Local_56_28 = Param_56_00046df5.ToString();
        object Param_56_000470db = (System.String)("ProgressBar");
        string Local_56_30 = System.String.Format("{0}\u005c{1}",(object)Param_56_00046df5,Param_56_000470db);
        string Expected_56 = "Themes\u005c{de3dbbcd-f642-433c-8353-8f1df4370aba}\u005cProgressBar";
        temp = (Expected_56 == Local_56_30);
        if(!temp) { Console.WriteLine(@"FAILED -> 56 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Concat-0x72ae6765411ff31a-101 ***
        temp = false;
        // erid(0x829c1d6d83a38785
        string Local_57_0 = System.String.Concat(new string[] {"https://","","","","app.vsspsext.visualstudio.com",":443","","/_apis/Profile/Profiles/me/Avatar","?size=Small",""});
        string Expected_57 = "https://app.vsspsext.visualstudio.com:443/_apis/Profile/Profiles/me/Avatar?size=Small";
        temp = (Expected_57 == Local_57_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 57 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Compare-0x78f127d418523c57-101 ***
        temp = false;
        // erid(0x801318350923faf7
        int Local_58_0 = System.String.Compare("AreChildItemsDisplayed","",(System.StringComparison)5);
        int Expected_58 = 22;
        temp = (Expected_58 == Local_58_0);
        if(!temp) { Console.WriteLine(@"FAILED -> 58 "); errcount++;}
        // //Summary 1-classes 1-modules 


        //*** System.String.Split-0x798e00162bc557f5-101 ***
        temp = false;
        // erid(0x8040b1acd0a6fe61
        object Param_59_01efdf0b = (System.String)("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/:::499b84ac-1321-427f-aa17-267ca6975798:::872cd9fa-d31f-45e0-9eab-6e460a02d1f1:::0");
        string[] Local_59_1 = ((System.String)Param_59_01efdf0b).Split(new string[] {":::"},2147483647,(System.StringSplitOptions)0);
        string[] Expected_59 = new string[] {"https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/","499b84ac-1321-427f-aa17-267ca6975798","872cd9fa-d31f-45e0-9eab-6e460a02d1f1","0"};
        temp = (Expected_59.Length == Local_59_1.Length); for(int i=0;i<Expected_59.Length;i++){if(Expected_59[i] != Local_59_1[i]){temp = false; break;}}
        if(!temp) { Console.WriteLine(@"FAILED -> 59 "); errcount++;}
        // //Summary 1-classes 1-modules 

        Assert.Equal(errcount,0);
    }

}
