// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.PaperKind.cs
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
// Author: Dennis Hayes (dennish@raytek.com)
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;

namespace System.Drawing.Printing
{
    [Serializable]
    public enum PaperKind
    {
        A2 = 66,
        A3 = 8,
        A3Extra = 63,
        A3ExtraTransverse = 68,
        A3Rotated = 76,
        A3Transverse = 67,
        A4 = 9,
        A4Extra = 53,
        A4Plus = 60,
        A4Rotated = 77,
        A4Small = 10,
        A4Transverse = 55,
        A5 = 11,
        A5Extra = 64,
        A5Rotated = 78,
        A5Transverse = 61,
        A6 = 70,
        A6Rotated = 83,
        APlus = 57,
        B4 = 12,
        B4Envelope = 33,
        B4JisRotated = 79,
        B5 = 13,
        B5Envelope = 34,
        B5Extra = 65,
        B5JisRotated = 80,
        B5Transverse = 62,
        B6Envelope = 35,
        B6Jis = 88,
        B6JisRotated = 89,
        BPlus = 58,
        C3Envelope = 29,
        C4Envelope = 30,
        C5Envelope = 28,
        C65Envelope = 32,
        C6Envelope = 31,
        CSheet = 24,
        Custom = 0,
        DLEnvelope = 27,
        DSheet = 25,
        ESheet = 26,
        Executive = 7,
        Folio = 14,
        GermanLegalFanfold = 41,
        GermanStandardFanfold = 40,
        InviteEnvelope = 47,
        IsoB4 = 42,
        ItalyEnvelope = 36,
        JapaneseDoublePostcard = 69,
        JapaneseDoublePostcardRotated = 82,
        JapaneseEnvelopeChouNumber3 = 73,
        JapaneseEnvelopeChouNumber3Rotated = 86,
        JapaneseEnvelopeChouNumber4 = 74,
        JapaneseEnvelopeChouNumber4Rotated = 87,
        JapaneseEnvelopeKakuNumber2 = 71,
        JapaneseEnvelopeKakuNumber2Rotated = 84,
        JapaneseEnvelopeKakuNumber3 = 72,
        JapaneseEnvelopeKakuNumber3Rotated = 85,
        JapaneseEnvelopeYouNumber4 = 91,
        JapaneseEnvelopeYouNumber4Rotated = 92,
        JapanesePostcard = 43,
        JapanesePostcardRotated = 81,
        Ledger = 4,
        Legal = 5,
        LegalExtra = 51,
        Letter = 1,
        LetterExtra = 50,
        LetterExtraTransverse = 56,
        LetterPlus = 59,
        LetterRotated = 75,
        LetterSmall = 2,
        LetterTransverse = 54,
        MonarchEnvelope = 37,
        Note = 18,
        Number10Envelope = 20,
        Number11Envelope = 21,
        Number12Envelope = 22,
        Number14Envelope = 23,
        Number9Envelope = 19,
        PersonalEnvelope = 38,
        Prc16K = 93,
        Prc16KRotated = 106,
        Prc32K = 94,
        Prc32KBig = 95,
        Prc32KBigRotated = 108,
        Prc32KRotated = 107,
        PrcEnvelopeNumber1 = 96,
        PrcEnvelopeNumber10 = 105,
        PrcEnvelopeNumber10Rotated = 118,
        PrcEnvelopeNumber1Rotated = 109,
        PrcEnvelopeNumber2 = 97,
        PrcEnvelopeNumber2Rotated = 110,
        PrcEnvelopeNumber3 = 98,
        PrcEnvelopeNumber3Rotated = 111,
        PrcEnvelopeNumber4 = 99,
        PrcEnvelopeNumber4Rotated = 112,
        PrcEnvelopeNumber5 = 100,
        PrcEnvelopeNumber5Rotated = 113,
        PrcEnvelopeNumber6 = 101,
        PrcEnvelopeNumber6Rotated = 114,
        PrcEnvelopeNumber7 = 102,
        PrcEnvelopeNumber7Rotated = 115,
        PrcEnvelopeNumber8 = 103,
        PrcEnvelopeNumber8Rotated = 116,
        PrcEnvelopeNumber9 = 104,
        PrcEnvelopeNumber9Rotated = 117,
        Quarto = 15,
        Standard10x11 = 45,
        Standard10x14 = 16,
        Standard11x17 = 17,
        Standard12x11 = 90,
        Standard15x11 = 46,
        Standard9x11 = 44,
        Statement = 6,
        Tabloid = 3,
        TabloidExtra = 52,
        USStandardFanfold = 39
    }
}
