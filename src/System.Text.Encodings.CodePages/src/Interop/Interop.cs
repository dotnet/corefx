// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

internal unsafe static partial class Interop
{
    internal const uint CP_ACP = 0x0u;

    [global::System.Runtime.InteropServices.DllImport("api-ms-win-core-localization-l1-2-0.dll")]
    internal extern static int GetCPInfoExW(
               uint CodePage,
               uint dwFlags,
               CPINFOEXW* lpCPInfoEx);

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    internal static bool GetCPInfoExW(uint CodePage, uint dwFlags, out CPINFOEXW lpCPInfoEx)
    {
        // Setup
        CPINFOEXW* unsafe_lpCPInfoEx;
        int unsafe_retval;
        // Marshalling
        fixed (CPINFOEXW* pinned_unsafe_lpCPInfoEx = &(lpCPInfoEx))
        {
            unsafe_lpCPInfoEx = pinned_unsafe_lpCPInfoEx;
            // Call to native method
            unsafe_retval = GetCPInfoExW(CodePage, dwFlags, unsafe_lpCPInfoEx);
        }
        // Return
        return unsafe_retval != 0;
    }

    internal unsafe partial struct INLINEARRAY_BYTE_2
    {
        public byte this[uint index]
        {
            get
            {
                if (index < 0 || index >= 2)
                    throw new global::System.IndexOutOfRangeException();
                fixed (INLINEARRAY_BYTE_2* pThis = &(this))
                    return ((byte*)pThis)[index];
            }
            set
            {
                if (index < 0 || index >= 2)
                    throw new global::System.IndexOutOfRangeException();
                fixed (INLINEARRAY_BYTE_2* pThis = &(this))
                    ((byte*)pThis)[index] = value;
            }
        }
        public const int Length = 2; private byte _elem_0; private byte _elem_1;
    }
    internal unsafe partial struct INLINEARRAY_BYTE_12
    {
        public byte this[uint index]
        {
            get
            {
                if (index < 0 || index >= 12)
                    throw new global::System.IndexOutOfRangeException();
                fixed (INLINEARRAY_BYTE_12* pThis = &(this))
                    return ((byte*)pThis)[index];
            }
            set
            {
                if (index < 0
                            || index >= 12)
                    throw new global::System.IndexOutOfRangeException();
                fixed (INLINEARRAY_BYTE_12* pThis = &(this))
                    ((byte*)pThis)[index] = value;
            }
        }
        public const int Length = 12; private byte _elem_0; private byte _elem_1; private byte _elem_2; private byte _elem_3; private byte _elem_4; private byte _elem_5; private byte _elem_6; private byte _elem_7; private byte _elem_8; private byte _elem_9; private byte _elem_10; private byte _elem_11;
    }

    [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = global::System.Runtime.InteropServices.CharSet.Unicode)]
    internal unsafe partial struct INLINEARRAY_WCHAR_260
    {
        // Copies characters from this buffer, up to the first null or end of buffer, into a new string.
        public string CopyToString()
        {
            fixed (char* ptr = &(_elem_0))
            {
                char* end = ptr;
                char* limit = (ptr + 260);
                while (end < limit && (*(end)) != 0)
                {
                    end = end + 1;
                }
                return new string(ptr, 0, ((int)(end - ptr)));
            }
        }

        public char this[uint index]
        {
            get
            {
                if (index < 0 || index >= 260)
                {
                    throw new IndexOutOfRangeException();
                }
                fixed (INLINEARRAY_WCHAR_260* pThis = &(this))
                    return ((char*)pThis)[index];
            }
            set
            {
                if (index < 0 || index >= 260)
                    throw new IndexOutOfRangeException();
                fixed (INLINEARRAY_WCHAR_260* pThis = &(this))
                    ((char*)pThis)[index] = value;
            }
        }
        public const int Length = 260; private char _elem_0; private char _elem_1; private char _elem_2; private char _elem_3; private char _elem_4; private char _elem_5; private char _elem_6; private char _elem_7; private char _elem_8; private char _elem_9; private char _elem_10; private char _elem_11; private char _elem_12; private char _elem_13; private char _elem_14; private char _elem_15; private char _elem_16; private char _elem_17; private char _elem_18; private char _elem_19; private char _elem_20; private char _elem_21; private char _elem_22; private char _elem_23; private char _elem_24; private char _elem_25; private char _elem_26; private char _elem_27; private char _elem_28; private char _elem_29; private char _elem_30; private char _elem_31; private char _elem_32; private char _elem_33; private char _elem_34; private char _elem_35; private char _elem_36; private char _elem_37; private char _elem_38; private char _elem_39; private char _elem_40; private char _elem_41; private char _elem_42; private char _elem_43; private char _elem_44; private char _elem_45; private char _elem_46; private char _elem_47; private char _elem_48; private char _elem_49; private char _elem_50; private char _elem_51; private char _elem_52; private char _elem_53; private char _elem_54; private char _elem_55; private char _elem_56; private char _elem_57; private char _elem_58; private char _elem_59; private char _elem_60; private char _elem_61; private char _elem_62; private char _elem_63; private char _elem_64; private char _elem_65; private char _elem_66; private char _elem_67; private char _elem_68; private char _elem_69; private char _elem_70; private char _elem_71; private char _elem_72; private char _elem_73; private char _elem_74; private char _elem_75; private char _elem_76; private char _elem_77; private char _elem_78; private char _elem_79; private char _elem_80; private char _elem_81; private char _elem_82; private char _elem_83; private char _elem_84; private char _elem_85; private char _elem_86; private char _elem_87; private char _elem_88; private char _elem_89; private char _elem_90; private char _elem_91; private char _elem_92; private char _elem_93; private char _elem_94; private char _elem_95; private char _elem_96; private char _elem_97; private char _elem_98; private char _elem_99; private char _elem_100; private char _elem_101; private char _elem_102; private char _elem_103; private char _elem_104; private char _elem_105; private char _elem_106; private char _elem_107; private char _elem_108; private char _elem_109; private char _elem_110; private char _elem_111; private char _elem_112; private char _elem_113; private char _elem_114; private char _elem_115; private char _elem_116; private char _elem_117; private char _elem_118; private char _elem_119; private char _elem_120; private char _elem_121; private char _elem_122; private char _elem_123; private char _elem_124; private char _elem_125; private char _elem_126; private char _elem_127; private char _elem_128; private char _elem_129; private char _elem_130; private char _elem_131; private char _elem_132; private char _elem_133; private char _elem_134; private char _elem_135; private char _elem_136; private char _elem_137; private char _elem_138; private char _elem_139; private char _elem_140; private char _elem_141; private char _elem_142; private char _elem_143; private char _elem_144; private char _elem_145; private char _elem_146; private char _elem_147; private char _elem_148; private char _elem_149; private char _elem_150; private char _elem_151; private char _elem_152; private char _elem_153; private char _elem_154; private char _elem_155; private char _elem_156; private char _elem_157; private char _elem_158; private char _elem_159; private char _elem_160; private char _elem_161; private char _elem_162; private char _elem_163; private char _elem_164; private char _elem_165; private char _elem_166; private char _elem_167; private char _elem_168; private char _elem_169; private char _elem_170; private char _elem_171; private char _elem_172; private char _elem_173; private char _elem_174; private char _elem_175; private char _elem_176; private char _elem_177; private char _elem_178; private char _elem_179; private char _elem_180; private char _elem_181; private char _elem_182; private char _elem_183; private char _elem_184; private char _elem_185; private char _elem_186; private char _elem_187; private char _elem_188; private char _elem_189; private char _elem_190; private char _elem_191; private char _elem_192; private char _elem_193; private char _elem_194; private char _elem_195; private char _elem_196; private char _elem_197; private char _elem_198; private char _elem_199; private char _elem_200; private char _elem_201; private char _elem_202; private char _elem_203; private char _elem_204; private char _elem_205; private char _elem_206; private char _elem_207; private char _elem_208; private char _elem_209; private char _elem_210; private char _elem_211; private char _elem_212; private char _elem_213; private char _elem_214; private char _elem_215; private char _elem_216; private char _elem_217; private char _elem_218; private char _elem_219; private char _elem_220; private char _elem_221; private char _elem_222; private char _elem_223; private char _elem_224; private char _elem_225; private char _elem_226; private char _elem_227; private char _elem_228; private char _elem_229; private char _elem_230; private char _elem_231; private char _elem_232; private char _elem_233; private char _elem_234; private char _elem_235; private char _elem_236; private char _elem_237; private char _elem_238; private char _elem_239; private char _elem_240; private char _elem_241; private char _elem_242; private char _elem_243; private char _elem_244; private char _elem_245; private char _elem_246; private char _elem_247; private char _elem_248; private char _elem_249; private char _elem_250; private char _elem_251; private char _elem_252; private char _elem_253; private char _elem_254; private char _elem_255; private char _elem_256; private char _elem_257; private char _elem_258; private char _elem_259;
    }

    [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = global::System.Runtime.InteropServices.CharSet.Unicode)]
    internal unsafe partial struct CPINFOEXW
    {
        public uint MaxCharSize;
        public INLINEARRAY_BYTE_2 DefaultChar;
        public INLINEARRAY_BYTE_12 LeadByte;
        public char UnicodeDefaultChar;
        public uint CodePage;
        public INLINEARRAY_WCHAR_260 CodePageName;
    }
}
