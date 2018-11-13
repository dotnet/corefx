﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Xunit;

namespace System.Tests
{
    public partial class DoubleTests
    {
        [Theory]
        [InlineData(double.NegativeInfinity, false)]    // Negative Infinity
        [InlineData(double.MinValue, true)]             // Min Negative Normal
        [InlineData(-2.2250738585072014E-308, true)]    // Max Negative Normal
        [InlineData(-2.2250738585072009E-308, true)]    // Min Negative Subnormal
        [InlineData(-4.94065645841247E-324, true)]      // Max Negative Subnormal
        [InlineData(-0.0, true)]                        // Negative Zero
        [InlineData(double.NaN, false)]                 // NaN
        [InlineData(0.0, true)]                         // Positive Zero
        [InlineData(4.94065645841247E-324, true)]       // Min Positive Subnormal
        [InlineData(2.2250738585072009E-308, true)]     // Max Positive Subnormal
        [InlineData(2.2250738585072014E-308, true)]     // Min Positive Normal
        [InlineData(double.MaxValue, true)]             // Max Positive Normal
        [InlineData(double.PositiveInfinity, false)]    // Positive Infinity
        public static void IsFinite(double d, bool expected)
        {
            Assert.Equal(expected, double.IsFinite(d));
        }

        [Theory]
        [InlineData(double.NegativeInfinity, true)]     // Negative Infinity
        [InlineData(double.MinValue, true)]             // Min Negative Normal
        [InlineData(-2.2250738585072014E-308, true)]    // Max Negative Normal
        [InlineData(-2.2250738585072009E-308, true)]    // Min Negative Subnormal
        [InlineData(-4.94065645841247E-324, true)]      // Max Negative Subnormal
        [InlineData(-0.0, true)]                        // Negative Zero
        [InlineData(double.NaN, true)]                  // NaN
        [InlineData(0.0, false)]                        // Positive Zero
        [InlineData(4.94065645841247E-324, false)]      // Min Positive Subnormal
        [InlineData(2.2250738585072009E-308, false)]    // Max Positive Subnormal
        [InlineData(2.2250738585072014E-308, false)]    // Min Positive Normal
        [InlineData(double.MaxValue, false)]            // Max Positive Normal
        [InlineData(double.PositiveInfinity, false)]    // Positive Infinity
        public static void IsNegative(double d, bool expected)
        {
            Assert.Equal(expected, double.IsNegative(d));
        }

        [Theory]
        [InlineData(double.NegativeInfinity, false)]    // Negative Infinity
        [InlineData(double.MinValue, true)]             // Min Negative Normal
        [InlineData(-2.2250738585072014E-308, true)]    // Max Negative Normal
        [InlineData(-2.2250738585072009E-308, false)]   // Min Negative Subnormal
        [InlineData(-4.94065645841247E-324, false)]     // Max Negative Subnormal
        [InlineData(-0.0, false)]                       // Negative Zero
        [InlineData(double.NaN, false)]                 // NaN
        [InlineData(0.0, false)]                        // Positive Zero
        [InlineData(4.94065645841247E-324, false)]      // Min Positive Subnormal
        [InlineData(2.2250738585072009E-308, false)]    // Max Positive Subnormal
        [InlineData(2.2250738585072014E-308, true)]     // Min Positive Normal
        [InlineData(double.MaxValue, true)]             // Max Positive Normal
        [InlineData(double.PositiveInfinity, false)]    // Positive Infinity
        public static void IsNormal(double d, bool expected)
        {
            Assert.Equal(expected, double.IsNormal(d));
        }

        [Theory]
        [InlineData(double.NegativeInfinity, false)]    // Negative Infinity
        [InlineData(double.MinValue, false)]            // Min Negative Normal
        [InlineData(-2.2250738585072014E-308, false)]   // Max Negative Normal
        [InlineData(-2.2250738585072009E-308, true)]    // Min Negative Subnormal
        [InlineData(-4.94065645841247E-324, true)]      // Max Negative Subnormal
        [InlineData(-0.0, false)]                       // Negative Zero
        [InlineData(double.NaN, false)]                 // NaN
        [InlineData(0.0, false)]                        // Positive Zero
        [InlineData(4.94065645841247E-324, true)]       // Min Positive Subnormal
        [InlineData(2.2250738585072009E-308, true)]     // Max Positive Subnormal
        [InlineData(2.2250738585072014E-308, false)]    // Min Positive Normal
        [InlineData(double.MaxValue, false)]            // Max Positive Normal
        [InlineData(double.PositiveInfinity, false)]    // Positive Infinity
        public static void IsSubnormal(double d, bool expected)
        {
            Assert.Equal(expected, double.IsSubnormal(d));
        }

        public static IEnumerable<object[]> Parse_ValidWithOffsetCount_TestData()
        {
            foreach (object[] inputs in Parse_Valid_TestData())
            {
                yield return new object[] { inputs[0], 0, ((string)inputs[0]).Length, inputs[1], inputs[2], inputs[3] };
            }

            const NumberStyles DefaultStyle = NumberStyles.Float | NumberStyles.AllowThousands;
            yield return new object[] { "-123", 0, 3, DefaultStyle, null, (double)-12 };
            yield return new object[] { "-123", 1, 3, DefaultStyle, null, (double)123 };
            yield return new object[] { "1E23", 0, 3, DefaultStyle, null, 1E2 };
            yield return new object[] { "(123)", 1, 3, NumberStyles.AllowParentheses, new NumberFormatInfo() { NumberDecimalSeparator = "." }, 123 };
            yield return new object[] { "-Infinity", 1, 8, NumberStyles.Any, NumberFormatInfo.InvariantInfo, double.PositiveInfinity };
        }

        [Theory]
        [MemberData(nameof(Parse_ValidWithOffsetCount_TestData))]
        public static void Parse_Span_Valid(string value, int offset, int count, NumberStyles style, IFormatProvider provider, double expected)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            double result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None)
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.True(double.TryParse(value.AsSpan(offset, count), out result));
                    Assert.Equal(expected, result);

                    Assert.Equal(expected, double.Parse(value.AsSpan(offset, count)));
                }

                Assert.Equal(expected, double.Parse(value.AsSpan(offset, count), provider: provider));
            }

            Assert.Equal(expected, double.Parse(value.AsSpan(offset, count), style, provider));

            Assert.True(double.TryParse(value.AsSpan(offset, count), style, provider, out result));
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Span_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            if (value != null)
            {
                Assert.Throws(exceptionType, () => double.Parse(value.AsSpan(), style, provider));

                Assert.False(double.TryParse(value.AsSpan(), style, provider, out double result));
                Assert.Equal(0, result);
            }
        }

        /// <summary>
        /// Test some specific floating-point literals that have been shown to be problematic
        /// in some implementation.
        /// </summary>
        [Theory]
        // https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx
        [InlineData("0.6822871999174", 0x3FE5D54BF743FD1Bul)]
        // Common problem numbers
        [InlineData("2.2250738585072011e-308", 0x000FFFFFFFFFFFFFul)]
        [InlineData("2.2250738585072012e-308", 0x0010000000000000ul)]
        // http://www.exploringbinary.com/bigcomp-deciding-truncated-near-halfway-conversions/
        [InlineData("1.3694713649464322631e-11", 0x3DAE1D703BB5749Dul)]
        [InlineData("9.3170532238714134438e+16", 0x4374B021AFD9F651ul)]
        // https://connect.microsoft.com/VisualStudio/feedback/details/914964/double-round-trip-conversion-via-a-string-is-not-safe
        [InlineData("0.84551240822557006", 0x3FEB0E7009B61CE0ul)]
        // This value has a non-terminating binary fraction.  It has a 0 at bit 54
        // followed by 120 ones.
        // http://www.exploringbinary.com/a-bug-in-the-bigcomp-function-of-david-gays-strtod/
        [InlineData("1.8254370818746402660437411213933955878019332885742187", 0x3FFD34FD8378EA83ul)]
        // http://www.exploringbinary.com/decimal-to-floating-point-needs-arbitrary-precision
        [InlineData("7.8459735791271921e+65", 0x4D9DCD0089C1314Eul)]
        [InlineData("3.08984926168550152811e-32", 0x39640DE48676653Bul)]
        // other values from https://github.com/dotnet/roslyn/issues/4221
        [InlineData("0.6822871999174000000", 0x3FE5D54BF743FD1Bul)]
        [InlineData("0.6822871999174000001", 0x3FE5D54BF743FD1Bul)]
        // A handful of selected values for which double.Parse has been observed to produce an incorrect result
        [InlineData("88.7448699245e+188", 0x675fde6aee647ed2ul)]
        [InlineData("02.0500496671303857e-88", 0x2dba19a3cf32cd7ful)]
        [InlineData("1.15362842193e193", 0x68043a6fcda86331ul)]
        [InlineData("65.9925719e-190", 0x18dd672e04e96a79ul)]
        [InlineData("0.4619024460e-158", 0x1f103c1e5abd7c87ul)]
        [InlineData("61.8391820096448e147", 0x5ed35849885ad12bul)]
        [InlineData("0.2e+127", 0x5a27a2ecc414a03ful)]
        [InlineData("0.8e+127", 0x5a47a2ecc414a03ful)]
        [InlineData("35.27614496323756e-307", 0x0083d141335ce6c7ul)]
        [InlineData("3.400034617466e190", 0x677e863a216ab367ul)]
        [InlineData("0.78393577148319e-254", 0x0b2d6d5379bc932dul)]
        [InlineData("0.947231e+100", 0x54b152a10483a38ful)]
        [InlineData("4.e126", 0x5a37a2ecc414a03ful)]
        [InlineData("6.235271922748e-165", 0x1dd6faeba4fc9f91ul)]
        [InlineData("3.497444198362024e-82", 0x2f053b8036dd4203ul)]
        [InlineData("8.e+126", 0x5a47a2ecc414a03ful)]
        [InlineData("10.027247729e+91", 0x53089cc2d930ed3ful)]
        [InlineData("4.6544819e-192", 0x18353c5d35ceaaadul)]
        [InlineData("5.e+125", 0x5a07a2ecc414a03ful)]
        [InlineData("6.96768e68", 0x4e39d8352ee997f9ul)]
        [InlineData("0.73433e-145", 0x21cd57b723dc17bful)]
        [InlineData("31.076044256878e259", 0x76043627aa7248dful)]
        [InlineData("0.8089124675e-201", 0x162fb3bf98f037f7ul)]
        [InlineData("88.7453407049700914e-144", 0x227150a674c218e3ul)]
        [InlineData("32.401089401e-65", 0x32c10fa88084d643ul)]
        [InlineData("0.734277884753e-209", 0x14834fdfb6248755ul)]
        [InlineData("8.3435e+153", 0x5fe3e9c5b617dc39ul)]
        [InlineData("30.379e-129", 0x25750ec799af9efful)]
        [InlineData("78.638509299e141", 0x5d99cb8c0a72cd05ul)]
        [InlineData("30.096884930e-42", 0x3784f976b4d47d63ul)]
        // values from http://www.icir.org/vern/papers/testbase-report.pdf table 1 (less than half ULP - round down)
        [InlineData("69e+267", 0x77C0B7CB60C994DAul)]
        [InlineData("999e-026", 0x3B282782AFE1869Eul)]
        [InlineData("7861e-034", 0x39AFE3544145E9D8ul)]
        [InlineData("75569e-254", 0x0C35A462D91C6AB3ul)]
        [InlineData("928609e-261", 0x0AFBE2DD66200BEFul)]
        [InlineData("9210917e+080", 0x51FDA232347E6032ul)]
        [InlineData("84863171e+114", 0x59406E98F5EC8F37ul)]
        [InlineData("653777767e+273", 0x7A720223F2B3A881ul)]
        [InlineData("5232604057e-298", 0x041465B896C24520ul)]
        [InlineData("27235667517e-109", 0x2B77D41824D64FB2ul)]
        [InlineData("653532977297e-123", 0x28D925A0AABCDC68ul)]
        [InlineData("3142213164987e-294", 0x057D3409DFBCA26Ful)]
        [InlineData("46202199371337e-072", 0x33D28F9EDFBD341Ful)]
        [InlineData("231010996856685e-073", 0x33C28F9EDFBD341Ful)]
        [InlineData("9324754620109615e+212", 0x6F43AE60753AF6CAul)]
        [InlineData("78459735791271921e+049", 0x4D9DCD0089C1314Eul)]
        [InlineData("272104041512242479e+200", 0x6D13BBB4BF05F087ul)]
        [InlineData("6802601037806061975e+198", 0x6CF3BBB4BF05F087ul)]
        [InlineData("20505426358836677347e-221", 0x161012954B6AABBAul)]
        [InlineData("836168422905420598437e-234", 0x13B20403A628A9CAul)]
        [InlineData("4891559871276714924261e+222", 0x7286ECAF7694A3C7ul)]
        // values from http://www.icir.org/vern/papers/testbase-report.pdf table 2 (greater than half ULP - round up)
        [InlineData("85e-037", 0x38A698CCDC60015Aul)]
        [InlineData("623e+100", 0x554640A62F3A83DFul)]
        [InlineData("3571e+263", 0x77462644C61D41AAul)]
        [InlineData("81661e+153", 0x60B7CA8E3D68578Eul)]
        [InlineData("920657e-023", 0x3C653A9985DBDE6Cul)]
        [InlineData("4603285e-024", 0x3C553A9985DBDE6Cul)]
        [InlineData("87575437e-309", 0x016E07320602056Cul)]
        [InlineData("245540327e+122", 0x5B01B6231E18C5CBul)]
        [InlineData("6138508175e+120", 0x5AE1B6231E18C5CBul)]
        [InlineData("83356057653e+193", 0x6A4544E6DAEE2A18ul)]
        [InlineData("619534293513e+124", 0x5C210C20303FE0F1ul)]
        [InlineData("2335141086879e+218", 0x6FC340A1C932C1EEul)]
        [InlineData("36167929443327e-159", 0x21BCE77C2B3328FCul)]
        [InlineData("609610927149051e-255", 0x0E104273B18918B1ul)]
        [InlineData("3743626360493413e-165", 0x20E8823A57ADBEF9ul)]
        [InlineData("94080055902682397e-242", 0x11364981E39E66CAul)]
        [InlineData("899810892172646163e+283", 0x7E6ADF51FA055E03ul)]
        [InlineData("7120190517612959703e+120", 0x5CC3220DCD5899FDul)]
        [InlineData("25188282901709339043e-252", 0x0FA4059AF3DB2A84ul)]
        [InlineData("308984926168550152811e-052", 0x39640DE48676653Bul)]
        [InlineData("6372891218502368041059e+064", 0x51C067047DBB38FEul)]
        // http://www.exploringbinary.com/incorrect-decimal-to-floating-point-conversion-in-sqlite/
        [InlineData("1e-23", 0x3B282DB34012B251ul)]
        [InlineData("8.533e+68", 0x4E3FA69165A8EEA2ul)]
        [InlineData("4.1006e-184", 0x19DBE0D1C7EA60C9ul)]
        [InlineData("9.998e+307", 0x7FE1CC0A350CA87Bul)]
        [InlineData("9.9538452227e-280", 0x0602117AE45CDE43ul)]
        [InlineData("6.47660115e-260", 0x0A1FDD9E333BADADul)]
        [InlineData("7.4e+47", 0x49E033D7ECA0ADEFul)]
        [InlineData("5.92e+48", 0x4A1033D7ECA0ADEFul)]
        [InlineData("7.35e+66", 0x4DD172B70EABABA9ul)]
        [InlineData("8.32116e+55", 0x4B8B2628393E02CDul)]
        public static void Parse_TestTroublesome(string s, ulong expectedBits)
        {
            CheckOneDouble(s, expectedBits);
        }

        /// <summary>
        /// Test round tripping for some specific floating-point values constructed to test the edge cases of conversion implementations.
        /// </summary>
        [Theory]
        [InlineData("0.0", 0x0000000000000000ul)]
        [InlineData("1.0e-99999999999999999999", 0x0000000000000000ul)]
        [InlineData("0e-99999999999999999999", 0x0000000000000000ul)]
        [InlineData("0e99999999999999999999", 0x0000000000000000ul)]
        // Verify small and large exactly representable integers:
        [InlineData("1", 0x3ff0000000000000)]
        [InlineData("2", 0x4000000000000000)]
        [InlineData("3", 0x4008000000000000)]
        [InlineData("4", 0x4010000000000000)]
        [InlineData("5", 0x4014000000000000)]
        [InlineData("6", 0x4018000000000000)]
        [InlineData("7", 0x401C000000000000)]
        [InlineData("8", 0x4020000000000000)]
        [InlineData("9007199254740984", 0x433ffffffffffff8)]
        [InlineData("9007199254740985", 0x433ffffffffffff9)]
        [InlineData("9007199254740986", 0x433ffffffffffffa)]
        [InlineData("9007199254740987", 0x433ffffffffffffb)]
        [InlineData("9007199254740988", 0x433ffffffffffffc)]
        [InlineData("9007199254740989", 0x433ffffffffffffd)]
        [InlineData("9007199254740990", 0x433ffffffffffffe)]
        [InlineData("9007199254740991", 0x433fffffffffffff)] // 2^53 - 1
        // Verify the smallest and largest denormal values:
        [InlineData("5.0e-324", 0x0000000000000001)]
        [InlineData("1.0e-323", 0x0000000000000002)]
        [InlineData("1.5e-323", 0x0000000000000003)]
        [InlineData("2.0e-323", 0x0000000000000004)]
        [InlineData("2.5e-323", 0x0000000000000005)]
        [InlineData("3.0e-323", 0x0000000000000006)]
        [InlineData("3.5e-323", 0x0000000000000007)]
        [InlineData("4.0e-323", 0x0000000000000008)]
        [InlineData("4.5e-323", 0x0000000000000009)]
        [InlineData("5.0e-323", 0x000000000000000a)]
        [InlineData("5.5e-323", 0x000000000000000b)]
        [InlineData("6.0e-323", 0x000000000000000c)]
        [InlineData("6.5e-323", 0x000000000000000d)]
        [InlineData("7.0e-323", 0x000000000000000e)]
        [InlineData("7.5e-323", 0x000000000000000f)]
        [InlineData("2.2250738585071935e-308", 0x000ffffffffffff0)]
        [InlineData("2.2250738585071940e-308", 0x000ffffffffffff1)]
        [InlineData("2.2250738585071945e-308", 0x000ffffffffffff2)]
        [InlineData("2.2250738585071950e-308", 0x000ffffffffffff3)]
        [InlineData("2.2250738585071955e-308", 0x000ffffffffffff4)]
        [InlineData("2.2250738585071960e-308", 0x000ffffffffffff5)]
        [InlineData("2.2250738585071964e-308", 0x000ffffffffffff6)]
        [InlineData("2.2250738585071970e-308", 0x000ffffffffffff7)]
        [InlineData("2.2250738585071974e-308", 0x000ffffffffffff8)]
        [InlineData("2.2250738585071980e-308", 0x000ffffffffffff9)]
        [InlineData("2.2250738585071984e-308", 0x000ffffffffffffa)]
        [InlineData("2.2250738585071990e-308", 0x000ffffffffffffb)]
        [InlineData("2.2250738585071994e-308", 0x000ffffffffffffc)]
        [InlineData("2.2250738585072000e-308", 0x000ffffffffffffd)]
        [InlineData("2.2250738585072004e-308", 0x000ffffffffffffe)]
        [InlineData("2.2250738585072010e-308", 0x000fffffffffffff)]
        // Test cases from Rick Regan's article, "Incorrectly Rounded Conversions in Visual C++":
        //
        //     http://www.exploringbinary.com/incorrectly-rounded-conversions-in-visual-c-plus-plus/
        // 
        // Example 1:
        [InlineData("9214843084008499", 0x43405e6cec57761a)]
        // Example 2:
        [InlineData("0.500000000000000166533453693773481063544750213623046875", 0x3fe0000000000002)]
        // Example 3 (2^-1 + 2^-53 + 2^-54):
        [InlineData("30078505129381147446200", 0x44997a3c7271b021)]
        // Example 4:
        [InlineData("1777820000000000000001", 0x4458180d5bad2e3e)]
        // Example 5 (2^-1 + 2^-53 + 2^-54 + 2^-66):
        [InlineData("0.500000000000000166547006220929549868969843373633921146392822265625", 0x3fe0000000000002)]
        // Example 6 (2^-1 + 2^-53 + 2^-54 + 2^-65):
        [InlineData("0.50000000000000016656055874808561867439493653364479541778564453125", 0x3fe0000000000002)]
        // Example 7:
        [InlineData("0.3932922657273", 0x3fd92bb352c4623a)]
        // The following test cases are taken from other articles on Rick Regan's
        // Exploring Binary blog.  These are conversions that other implementations
        // were found to perform incorrectly.
        // http://www.exploringbinary.com/nondeterministic-floating-point-conversions-in-java/
        // http://www.exploringbinary.com/incorrectly-rounded-subnormal-conversions-in-java/
        // Example 1 (2^-1047 + 2^-1075, half-ulp above a power of two):
        [InlineData("6.6312368714697582767853966302759672433990999473553031442499717587" +
                    "362866301392654396180682007880487441059604205526018528897150063763" +
                    "256665955396033303618005191075917832333584923372080578494993608994" +
                    "251286407188566165030934449228547591599881603044399098682919739314" +
                    "266256986631577498362522745234853124423586512070512924530832781161" +
                    "439325697279187097860044978723221938561502254152119972830784963194" +
                    "121246401117772161481107528151017752957198119743384519360959074196" +
                    "224175384736794951486324803914359317679811223967034438033355297560" +
                    "033532098300718322306892013830155987921841729099279241763393155074" +
                    "022348361207309147831684007154624400538175927027662135590421159867" +
                    "638194826541287705957668068727833491469671712939495988506756821156" +
                    "96218943412532098591327667236328125E-316", 0x0000000008000000)]
        // Example 2 (2^-1058 - 2^-1075, half-ulp below a power of two):
        [InlineData("3.2378839133029012895883524125015321748630376694231080599012970495" +
                    "523019706706765657868357425877995578606157765598382834355143910841" +
                    "531692526891905643964595773946180389283653051434639551003566966656" +
                    "292020173313440317300443693602052583458034314716600326995807313009" +
                    "548483639755486900107515300188817581841745696521731104736960227499" +
                    "346384253806233697747365600089974040609674980283891918789639685754" +
                    "392222064169814626901133425240027243859416510512935526014211553334" +
                    "302252372915238433223313261384314778235911424088000307751706259156" +
                    "707286570031519536642607698224949379518458015308952384398197084033" +
                    "899378732414634842056080000272705311068273879077914449185347715987" +
                    "501628125488627684932015189916680282517302999531439241685457086639" +
                    "13273994694463908672332763671875E-319", 0x0000000000010000)]
        // Example 3 (2^-1027 + 2^-1066 + 2^-1075, half-ulp above a non-power of two):
        [InlineData("6.9533558078476771059728052155218916902221198171459507544162056079" +
                    "800301315496366888061157263994418800653863998640286912755395394146" +
                    "528315847956685600829998895513577849614468960421131982842131079351" +
                    "102171626549398024160346762138294097205837595404767869364138165416" +
                    "212878432484332023692099166122496760055730227032447997146221165421" +
                    "888377703760223711720795591258533828013962195524188394697705149041" +
                    "926576270603193728475623010741404426602378441141744972109554498963" +
                    "891803958271916028866544881824524095839813894427833770015054620157" +
                    "450178487545746683421617594966617660200287528887833870748507731929" +
                    "971029979366198762266880963149896457660004790090837317365857503352" +
                    "620998601508967187744019647968271662832256419920407478943826987518" +
                    "09812609536720628966577351093292236328125E-310", 0x0000800000000100)]
        // Example 4 (2^-1058 + 2^-1063 + 2^-1075, half-ulp below a non-power of two):
        [InlineData("3.3390685575711885818357137012809439119234019169985217716556569973" +
                    "284403145596153181688491490746626090999981130094655664268081703784" +
                    "340657229916596426194677060348844249897410807907667784563321682004" +
                    "646515939958173717821250106683466529959122339932545844611258684816" +
                    "333436749050742710644097630907080178565840197768788124253120088123" +
                    "262603630354748115322368533599053346255754042160606228586332807443" +
                    "018924703005556787346899784768703698535494132771566221702458461669" +
                    "916553215355296238706468887866375289955928004361779017462862722733" +
                    "744717014529914330472578638646014242520247915673681950560773208853" +
                    "293843223323915646452641434007986196650406080775491621739636492640" +
                    "497383622906068758834568265867109610417379088720358034812416003767" +
                    "05491726170293986797332763671875E-319", 0x0000000000010800)]
        // http://www.exploringbinary.com/gays-strtod-returns-zero-for-inputs-just-above-2-1075/
        // A number between 2^-2074 and 2^-1075, just slightly larger than 2^-1075.
        // It has bit 1075 set (the denormal rounding bit), followed by 2506 zeroes,
        // followed by one bits.  It should round up to 2^-1074.
        [InlineData("2.470328229206232720882843964341106861825299013071623822127928412503" +
                    "37753635104375932649918180817996189898282347722858865463328355177969" +
                    "89819938739800539093906315035659515570226392290858392449105184435931" +
                    "80284993653615250031937045767824921936562366986365848075700158576926" +
                    "99037063119282795585513329278343384093519780155312465972635795746227" +
                    "66465272827220056374006485499977096599470454020828166226237857393450" +
                    "73633900796776193057750674017632467360096895134053553745851666113422" +
                    "37666786041621596804619144672918403005300575308490487653917113865916" +
                    "46239524912623653881879636239373280423891018672348497668235089863388" +
                    "58792562830275599565752445550725518931369083625477918694866799496832" +
                    "40497058210285131854513962138377228261454376934125320985913276672363" +
                    "28125001e-324", 0x0000000000000001)]
        public static void Parse_TestSpecific(string s, ulong expectedBits)
        {
            CheckOneDouble(s, expectedBits);
        }

        /// <summary>
        /// Test round tripping for some specific floating-point values constructed to test the edge cases of conversion implementations.
        /// </summary>
        [Theory]
        // Verify the smallest denormals:
        [InlineData(0x0000000000000001ul, 0x0000000000000100ul)]
        // Verify the largest denormals and the smallest normals:
        [InlineData(0x000fffffffffff00ul, 0x0010000000000100ul)]
        // Verify the largest normals:
        [InlineData(0x7fefffffffffff00ul, 0x7ff0000000000000ul)]
        public static void Parse_TestSpecificRanges(ulong start, ulong end)
        {
            for (ulong i = start; i != end; i++)
            {
                TestRoundTripDouble(i);
            }
        }

        [Theory]
        // Verify all representable powers of two and nearby values:
        [InlineData(2, -1022, 1024)]
        // Verify all representable powers of ten and nearby values:
        [InlineData(10, -323, 309)]
        public static void Parse_TestSpecificPowers(int b, int start, int end)
        {
            for (int i = start; i != end; i++)
            {
                double d = Math.Pow(b, i);
                ulong bits = (ulong)BitConverter.DoubleToInt64Bits(d);

                TestRoundTripDouble(bits - 1);
                TestRoundTripDouble(bits);
                TestRoundTripDouble(bits + 1);
            }
        }

        private static void CheckOneDouble(string s, ulong expectedBits)
        {
            CheckOneDouble(s, BitConverter.Int64BitsToDouble((long)(expectedBits)));
        }

        private static void CheckOneDouble(string s, double expected)
        {
            if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double actual))
            {
                // If we fail to parse, set actual to NaN to ensure the comparison below will fail
                actual = double.NaN;
            }

            Assert.True(actual.Equals(expected), $"Expected {InvariantToString(expected)}, Actual {InvariantToString(actual)}");
        }

        private static string InvariantToString(object o)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:G17}", o);
        }

        private static void TestRoundTripDouble(double d)
        {
            string s = InvariantToString(d);
            CheckOneDouble(s, d);
        }

        private static void TestRoundTripDouble(ulong bits)
        {
            double d = BitConverter.Int64BitsToDouble((long)(bits));

            if (double.IsFinite(d))
            {
                string s = InvariantToString(d);
                CheckOneDouble(s, bits);
            }
        }

        [Fact]
        public static void TryFormat()
        {
            RemoteInvoke(() =>
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                foreach (var testdata in ToString_TestData())
                {
                    double localI = (double)testdata[0];
                    string localFormat = (string)testdata[1];
                    IFormatProvider localProvider = (IFormatProvider)testdata[2];
                    string localExpected = (string)testdata[3];

                    try
                    {
                        char[] actual;
                        int charsWritten;

                        // Just right
                        actual = new char[localExpected.Length];
                        Assert.True(localI.TryFormat(actual.AsSpan(), out charsWritten, localFormat, localProvider));
                        Assert.Equal(localExpected.Length, charsWritten);
                        Assert.Equal(localExpected, new string(actual));

                        // Longer than needed
                        actual = new char[localExpected.Length + 1];
                        Assert.True(localI.TryFormat(actual.AsSpan(), out charsWritten, localFormat, localProvider));
                        Assert.Equal(localExpected.Length, charsWritten);
                        Assert.Equal(localExpected, new string(actual, 0, charsWritten));

                        // Too short
                        if (localExpected.Length > 0)
                        {
                            actual = new char[localExpected.Length - 1];
                            Assert.False(localI.TryFormat(actual.AsSpan(), out charsWritten, localFormat, localProvider));
                            Assert.Equal(0, charsWritten);
                        }
                    }
                    catch (Exception exc)
                    {
                        throw new Exception($"Failed on `{localI}`, `{localFormat}`, `{localProvider}`, `{localExpected}`. {exc}");
                    }
                }

                return SuccessExitCode;
            }).Dispose();
        }
    }
}
