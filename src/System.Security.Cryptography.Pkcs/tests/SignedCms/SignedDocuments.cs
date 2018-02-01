// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Test.Cryptography;

namespace System.Security.Cryptography.Pkcs.Tests
{
    internal static class SignedDocuments
    {
        internal static readonly byte[] RsaPssDocument = (
            "308204EC06092A864886F70D010702A08204DD308204D9020103310D300B0609" +
            "608648016503040201301F06092A864886F70D010701A0120410546869732069" +
            "73206120746573740D0AA08202CA308202C63082022EA003020102020900F399" +
            "4D1706DEC3F8300D06092A864886F70D01010B0500307B310B30090603550406" +
            "130255533113301106035504080C0A57617368696E67746F6E3110300E060355" +
            "04070C075265646D6F6E6431183016060355040A0C0F4D6963726F736F667420" +
            "436F72702E31173015060355040B0C0E2E4E4554204672616D65776F726B3112" +
            "301006035504030C096C6F63616C686F7374301E170D31363033303230323337" +
            "35345A170D3137303330323032333735345A307B310B30090603550406130255" +
            "533113301106035504080C0A57617368696E67746F6E3110300E06035504070C" +
            "075265646D6F6E6431183016060355040A0C0F4D6963726F736F667420436F72" +
            "702E31173015060355040B0C0E2E4E4554204672616D65776F726B3112301006" +
            "035504030C096C6F63616C686F73743081A0300D06092A864886F70D01010105" +
            "0003818E0030818A02818200BCACB1A5349D7B35A580AC3B3998EB15EBF900EC" +
            "B329BF1F75717A00B2199C8A18D791B592B7EC52BD5AF2DB0D3B635F0595753D" +
            "FF7BA7C9872DBF7E3226DEF44A07CA568D1017992C2B41BFE5EC3570824CF1F4" +
            "B15919FED513FDA56204AF2034A2D08FF04C2CCA49D168FA03FA2FA32FCCD348" +
            "4C15F0A2E5467C76FC760B55090203010001A350304E301D0603551D0E041604" +
            "141063CAB14FB14C47DC211C0E0285F3EE5946BF2D301F0603551D2304183016" +
            "80141063CAB14FB14C47DC211C0E0285F3EE5946BF2D300C0603551D13040530" +
            "030101FF300D06092A864886F70D01010B050003818200435774FB66802AB3CE" +
            "2F1392C079483B48CC8913E0BF3B7AD88351E4C15B55CAD3061AA5875900C56B" +
            "2E7E84BB49CA2A0C1895BD60149C6A0AE983E48370E2144052943B066BD85F70" +
            "543CF6F2F255C028AE1DC8FB898AD3DCA97BF1D607370287077A4C147268C911" +
            "8CF9CAD318D2830D3468727E0A3247B3FEB8D87A7DE4F1E2318201D4308201D0" +
            "02010380141063CAB14FB14C47DC211C0E0285F3EE5946BF2D300B0609608648" +
            "016503040201A081E4301806092A864886F70D010903310B06092A864886F70D" +
            "010701301C06092A864886F70D010905310F170D313731303236303130363235" +
            "5A302F06092A864886F70D0109043122042007849DC26FCBB2F3BD5F57BDF214" +
            "BAE374575F1BD4E6816482324799417CB379307906092A864886F70D01090F31" +
            "6C306A300B060960864801650304012A300B0609608648016503040116300B06" +
            "09608648016503040102300A06082A864886F70D0307300E06082A864886F70D" +
            "030202020080300D06082A864886F70D0302020140300706052B0E030207300D" +
            "06082A864886F70D0302020128303D06092A864886F70D01010A3030A00D300B" +
            "0609608648016503040201A11A301806092A864886F70D010108300B06096086" +
            "48016503040201A20302015F048181B93E81D141B3C9F159AB0021910635DC72" +
            "E8E860BE43C28E5D53243D6DC247B7D4F18C20195E80DEDCC75B29C43CE5047A" +
            "D775B65BFC93589BD748B950C68BADDF1A4673130302BBDA8667D5DDE5EA91EC" +
            "CB13A9B4C04F1C4842FEB1697B7669C7692DD3BDAE13B5AA8EE3EB5679F3729D" +
            "1DC4F2EB9DC89B7E8773F2F8C6108C05").HexToByteArray();

        public static byte[] RsaPkcs1OneSignerIssuerAndSerialNumber = (
            "3082033706092A864886F70D010702A082032830820324020101310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6EA08202103082020C30820179A0030201020210" +
            "5D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500301E311C30" +
            "1A060355040313135253414B65795472616E736665724361706931301E170D31" +
            "35303431353037303030305A170D3235303431353037303030305A301E311C30" +
            "1A060355040313135253414B65795472616E73666572436170693130819F300D" +
            "06092A864886F70D010101050003818D0030818902818100AA272700586C0CC4" +
            "1B05C65C7D846F5A2BC27B03E301C37D9BFF6D75B6EB6671BA9596C5C63BA2B1" +
            "AF5C318D9CA39E7400D10C238AC72630579211B86570D1A1D44EC86AA8F6C9D2" +
            "B4E283EA3535923F398A312A23EAEACD8D34FAACA965CD910B37DA4093EF76C1" +
            "3B337C1AFAB7D1D07E317B41A336BAA4111299F99424408D0203010001A35330" +
            "51304F0603551D0104483046801015432DB116B35D07E4BA89EDB2469D7AA120" +
            "301E311C301A060355040313135253414B65795472616E736665724361706931" +
            "82105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500038181" +
            "0081E5535D8ECEEF265ACBC82F6C5F8BC9D84319265F3CCF23369FA533C8DC19" +
            "38952C5931662D9ECD8B1E7B81749E48468167E2FCE3D019FA70D54646975B6D" +
            "C2A3BA72D5A5274C1866DA6D7A5DF47938E034A075D11957D653B5C78E5291E4" +
            "401045576F6D4EDA81BEF3C369AF56121E49A083C8D1ADB09F291822E99A4296" +
            "463181D73081D40201013032301E311C301A060355040313135253414B657954" +
            "72616E73666572436170693102105D2FFFF863BABC9B4D3C80AB178A4CCA3009" +
            "06052B0E03021A0500300D06092A864886F70D01010105000481805A1717621D" +
            "450130B3463662160EEC06F7AE77E017DD95F294E97A0BDD433FE6B2CCB34FAA" +
            "C33AEA50BFD7D9E78DC7174836284619F744278AE77B8495091E096EEF682D9C" +
            "A95F6E81C7DDCEDDA6A12316B453C894B5000701EB09DF57A53B733A4E80DA27" +
            "FA710870BD88C86E2FDB9DCA14D18BEB2F0C87E9632ABF02BE2FE3").HexToByteArray();

        public static byte[] CounterSignedRsaPkcs1OneSigner = (
            "3082044906092A864886F70D010702A082043A30820436020101310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6EA08202103082020C30820179A0030201020210" +
            "5D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500301E311C30" +
            "1A060355040313135253414B65795472616E736665724361706931301E170D31" +
            "35303431353037303030305A170D3235303431353037303030305A301E311C30" +
            "1A060355040313135253414B65795472616E73666572436170693130819F300D" +
            "06092A864886F70D010101050003818D0030818902818100AA272700586C0CC4" +
            "1B05C65C7D846F5A2BC27B03E301C37D9BFF6D75B6EB6671BA9596C5C63BA2B1" +
            "AF5C318D9CA39E7400D10C238AC72630579211B86570D1A1D44EC86AA8F6C9D2" +
            "B4E283EA3535923F398A312A23EAEACD8D34FAACA965CD910B37DA4093EF76C1" +
            "3B337C1AFAB7D1D07E317B41A336BAA4111299F99424408D0203010001A35330" +
            "51304F0603551D0104483046801015432DB116B35D07E4BA89EDB2469D7AA120" +
            "301E311C301A060355040313135253414B65795472616E736665724361706931" +
            "82105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500038181" +
            "0081E5535D8ECEEF265ACBC82F6C5F8BC9D84319265F3CCF23369FA533C8DC19" +
            "38952C5931662D9ECD8B1E7B81749E48468167E2FCE3D019FA70D54646975B6D" +
            "C2A3BA72D5A5274C1866DA6D7A5DF47938E034A075D11957D653B5C78E5291E4" +
            "401045576F6D4EDA81BEF3C369AF56121E49A083C8D1ADB09F291822E99A4296" +
            "46318201E8308201E40201013032301E311C301A060355040313135253414B65" +
            "795472616E73666572436170693102105D2FFFF863BABC9B4D3C80AB178A4CCA" +
            "300906052B0E03021A0500300D06092A864886F70D01010105000481805A1717" +
            "621D450130B3463662160EEC06F7AE77E017DD95F294E97A0BDD433FE6B2CCB3" +
            "4FAAC33AEA50BFD7D9E78DC7174836284619F744278AE77B8495091E096EEF68" +
            "2D9CA95F6E81C7DDCEDDA6A12316B453C894B5000701EB09DF57A53B733A4E80" +
            "DA27FA710870BD88C86E2FDB9DCA14D18BEB2F0C87E9632ABF02BE2FE3A18201" +
            "0C3082010806092A864886F70D0109063181FA3081F702010380146B4A6B92FD" +
            "ED07EE0119F3674A96D1A70D2A588D300906052B0E03021A0500A03F30180609" +
            "2A864886F70D010903310B06092A864886F70D010701302306092A864886F70D" +
            "010904311604148C054D6DF2B08E69A86D8DB23C1A509123F9DBA4300D06092A" +
            "864886F70D0101010500048180962518DEF789B0886C7E6295754ECDBDC4CB9D" +
            "153ECE5EBBE7A82142B92C30DDBBDFC22B5B954F5D844CBAEDCA9C4A068B2483" +
            "0E2A96141A5D0320B69EA5DFCFEA441E162D04506F8FFA79D7312524F111A9B9" +
            "B0184007139F94E46C816E0E33F010AEB949F5D884DC8987765002F7A643F34B" +
            "7654E3B2FD5FB34A420279B1EA").HexToByteArray();

        public static byte[] NoSignatureSignedWithAttributesAndCounterSignature = (
            "3082042406092A864886F70D010702A082041530820411020101310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6EA08202103082020C30820179A0030201020210" +
            "5D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500301E311C30" +
            "1A060355040313135253414B65795472616E736665724361706931301E170D31" +
            "35303431353037303030305A170D3235303431353037303030305A301E311C30" +
            "1A060355040313135253414B65795472616E73666572436170693130819F300D" +
            "06092A864886F70D010101050003818D0030818902818100AA272700586C0CC4" +
            "1B05C65C7D846F5A2BC27B03E301C37D9BFF6D75B6EB6671BA9596C5C63BA2B1" +
            "AF5C318D9CA39E7400D10C238AC72630579211B86570D1A1D44EC86AA8F6C9D2" +
            "B4E283EA3535923F398A312A23EAEACD8D34FAACA965CD910B37DA4093EF76C1" +
            "3B337C1AFAB7D1D07E317B41A336BAA4111299F99424408D0203010001A35330" +
            "51304F0603551D0104483046801015432DB116B35D07E4BA89EDB2469D7AA120" +
            "301E311C301A060355040313135253414B65795472616E736665724361706931" +
            "82105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500038181" +
            "0081E5535D8ECEEF265ACBC82F6C5F8BC9D84319265F3CCF23369FA533C8DC19" +
            "38952C5931662D9ECD8B1E7B81749E48468167E2FCE3D019FA70D54646975B6D" +
            "C2A3BA72D5A5274C1866DA6D7A5DF47938E034A075D11957D653B5C78E5291E4" +
            "401045576F6D4EDA81BEF3C369AF56121E49A083C8D1ADB09F291822E99A4296" +
            "46318201C3308201BF020101301C3017311530130603550403130C44756D6D79" +
            "205369676E6572020100300906052B0E03021A0500A05D301806092A864886F7" +
            "0D010903310B06092A864886F70D010701301C06092A864886F70D010905310F" +
            "170D3137313130313137313731375A302306092A864886F70D01090431160414" +
            "A5F085E7F326F3D6CA3BFD6280A3DE8EBC2EA60E300C06082B06010505070602" +
            "050004148B70D20D0477A35CD84AB962C10DC52FBA6FAD6BA182010C30820108" +
            "06092A864886F70D0109063181FA3081F702010380146B4A6B92FDED07EE0119" +
            "F3674A96D1A70D2A588D300906052B0E03021A0500A03F301806092A864886F7" +
            "0D010903310B06092A864886F70D010701302306092A864886F70D0109043116" +
            "0414833378066BDCCBA7047EF6919843D181A57D6479300D06092A864886F70D" +
            "01010105000481802155D226DD744166E582D040E60535210195050EA00F2C17" +
            "9897198521DABD0E6B27750FD8BA5F9AAF58B4863B6226456F38553A22453CAF" +
            "0A0F106766C7AB6F3D6AFD106753DC50F8A6E4F9E5508426D236C2DBB4BCB816" +
            "2FA42E995CBA16A340FD7C793569DF1B71368E68253299BC74E38312B40B8F52" +
            "EAEDE10DF414A522").HexToByteArray();

        public static byte[] NoSignatureWithNoAttributes = (
            "30819B06092A864886F70D010702A0818D30818A020101310B300906052B0E03" +
            "021A0500302406092A864886F70D010701A01704154D6963726F736F66742043" +
            "6F72706F726174696F6E31523050020101301C3017311530130603550403130C" +
            "44756D6D79205369676E6572020100300906052B0E03021A0500300C06082B06" +
            "01050507060205000414A5F085E7F326F3D6CA3BFD6280A3DE8EBC2EA60E").HexToByteArray();

        public static byte[] RsaCapiTransfer1_NoEmbeddedCert = (
            "3082016606092A864886F70D010702A082015730820153020103310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6E318201193082011502010380146B4A6B92FDED" +
            "07EE0119F3674A96D1A70D2A588D300906052B0E03021A0500A05D301806092A" +
            "864886F70D010903310B06092A864886F70D010701301C06092A864886F70D01" +
            "0905310F170D3137313130323135333430345A302306092A864886F70D010904" +
            "31160414A5F085E7F326F3D6CA3BFD6280A3DE8EBC2EA60E300D06092A864886" +
            "F70D01010105000481800EDE3870B8A80B45A21BAEC4681D059B46502E1B1AA6" +
            "B8920CF50D4D837646A55559B4C05849126C655D95FF3C6C1B420E07DC42629F" +
            "294EE69822FEA56F32D41B824CBB6BF809B7583C27E77B7AC58DFC925B1C60EA" +
            "4A67AA84D73FC9E9191D33B36645F17FD6748A2D8B12C6C384C3C734D2727338" +
            "6211E4518FE2B4ED0147").HexToByteArray();

        public static byte[] OneRsaSignerTwoRsaCounterSigners = (
            "3082075106092A864886F70D010702A08207423082073E020101310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6EA08203F9308201E530820152A0030201020210" +
            "D5B5BC1C458A558845BFF51CB4DFF31C300906052B0E03021D05003011310F30" +
            "0D060355040313064D794E616D65301E170D3130303430313038303030305A17" +
            "0D3131303430313038303030305A3011310F300D060355040313064D794E616D" +
            "6530819F300D06092A864886F70D010101050003818D0030818902818100B11E" +
            "30EA87424A371E30227E933CE6BE0E65FF1C189D0D888EC8FF13AA7B42B68056" +
            "128322B21F2B6976609B62B6BC4CF2E55FF5AE64E9B68C78A3C2DACC916A1BC7" +
            "322DD353B32898675CFB5B298B176D978B1F12313E3D865BC53465A11CCA1068" +
            "70A4B5D50A2C410938240E92B64902BAEA23EB093D9599E9E372E48336730203" +
            "010001A346304430420603551D01043B3039801024859EBF125E76AF3F0D7979" +
            "B4AC7A96A1133011310F300D060355040313064D794E616D658210D5B5BC1C45" +
            "8A558845BFF51CB4DFF31C300906052B0E03021D0500038181009BF6E2CF830E" +
            "D485B86D6B9E8DFFDCD65EFC7EC145CB9348923710666791FCFA3AB59D689FFD" +
            "7234B7872611C5C23E5E0714531ABADB5DE492D2C736E1C929E648A65CC9EB63" +
            "CD84E57B5909DD5DDF5DBBBA4A6498B9CA225B6E368B94913BFC24DE6B2BD9A2" +
            "6B192B957304B89531E902FFC91B54B237BB228BE8AFCDA264763082020C3082" +
            "0179A00302010202105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03" +
            "021D0500301E311C301A060355040313135253414B65795472616E7366657243" +
            "61706931301E170D3135303431353037303030305A170D323530343135303730" +
            "3030305A301E311C301A060355040313135253414B65795472616E7366657243" +
            "6170693130819F300D06092A864886F70D010101050003818D00308189028181" +
            "00AA272700586C0CC41B05C65C7D846F5A2BC27B03E301C37D9BFF6D75B6EB66" +
            "71BA9596C5C63BA2B1AF5C318D9CA39E7400D10C238AC72630579211B86570D1" +
            "A1D44EC86AA8F6C9D2B4E283EA3535923F398A312A23EAEACD8D34FAACA965CD" +
            "910B37DA4093EF76C13B337C1AFAB7D1D07E317B41A336BAA4111299F9942440" +
            "8D0203010001A3533051304F0603551D0104483046801015432DB116B35D07E4" +
            "BA89EDB2469D7AA120301E311C301A060355040313135253414B65795472616E" +
            "73666572436170693182105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B" +
            "0E03021D05000381810081E5535D8ECEEF265ACBC82F6C5F8BC9D84319265F3C" +
            "CF23369FA533C8DC1938952C5931662D9ECD8B1E7B81749E48468167E2FCE3D0" +
            "19FA70D54646975B6DC2A3BA72D5A5274C1866DA6D7A5DF47938E034A075D119" +
            "57D653B5C78E5291E4401045576F6D4EDA81BEF3C369AF56121E49A083C8D1AD" +
            "B09F291822E99A42964631820307308203030201013032301E311C301A060355" +
            "040313135253414B65795472616E73666572436170693102105D2FFFF863BABC" +
            "9B4D3C80AB178A4CCA300906052B0E03021A0500300D06092A864886F70D0101" +
            "0105000481805A1717621D450130B3463662160EEC06F7AE77E017DD95F294E9" +
            "7A0BDD433FE6B2CCB34FAAC33AEA50BFD7D9E78DC7174836284619F744278AE7" +
            "7B8495091E096EEF682D9CA95F6E81C7DDCEDDA6A12316B453C894B5000701EB" +
            "09DF57A53B733A4E80DA27FA710870BD88C86E2FDB9DCA14D18BEB2F0C87E963" +
            "2ABF02BE2FE3A182022B3082010806092A864886F70D0109063181FA3081F702" +
            "010380146B4A6B92FDED07EE0119F3674A96D1A70D2A588D300906052B0E0302" +
            "1A0500A03F301806092A864886F70D010903310B06092A864886F70D01070130" +
            "2306092A864886F70D010904311604148C054D6DF2B08E69A86D8DB23C1A5091" +
            "23F9DBA4300D06092A864886F70D0101010500048180962518DEF789B0886C7E" +
            "6295754ECDBDC4CB9D153ECE5EBBE7A82142B92C30DDBBDFC22B5B954F5D844C" +
            "BAEDCA9C4A068B24830E2A96141A5D0320B69EA5DFCFEA441E162D04506F8FFA" +
            "79D7312524F111A9B9B0184007139F94E46C816E0E33F010AEB949F5D884DC89" +
            "87765002F7A643F34B7654E3B2FD5FB34A420279B1EA3082011B06092A864886" +
            "F70D0109063182010C3082010802010130253011310F300D060355040313064D" +
            "794E616D650210D5B5BC1C458A558845BFF51CB4DFF31C300906052B0E03021A" +
            "0500A03F301806092A864886F70D010903310B06092A864886F70D0107013023" +
            "06092A864886F70D010904311604148C054D6DF2B08E69A86D8DB23C1A509123" +
            "F9DBA4300D06092A864886F70D01010105000481801AA282DBED4D862D7CEA30" +
            "F803E790BDB0C97EE852778CEEDDCD94BB9304A1552E60A8D36052AC8C2D2875" +
            "5F3B2F473824100AB3A6ABD4C15ABD77E0FFE13D0DF253BCD99C718FA673B6CB" +
            "0CBBC68CE5A4AC671298C0A07C7223522E0E7FFF15CEDBAB55AAA99588517674" +
            "671691065EB083FB729D1E9C04B2BF99A9953DAA5E").HexToByteArray();

        public static readonly byte[] RsaPkcs1CounterSignedWithNoSignature = (
            "308203E106092A864886F70D010702A08203D2308203CE020101310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6EA08202103082020C30820179A0030201020210" +
            "5D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500301E311C30" +
            "1A060355040313135253414B65795472616E736665724361706931301E170D31" +
            "35303431353037303030305A170D3235303431353037303030305A301E311C30" +
            "1A060355040313135253414B65795472616E73666572436170693130819F300D" +
            "06092A864886F70D010101050003818D0030818902818100AA272700586C0CC4" +
            "1B05C65C7D846F5A2BC27B03E301C37D9BFF6D75B6EB6671BA9596C5C63BA2B1" +
            "AF5C318D9CA39E7400D10C238AC72630579211B86570D1A1D44EC86AA8F6C9D2" +
            "B4E283EA3535923F398A312A23EAEACD8D34FAACA965CD910B37DA4093EF76C1" +
            "3B337C1AFAB7D1D07E317B41A336BAA4111299F99424408D0203010001A35330" +
            "51304F0603551D0104483046801015432DB116B35D07E4BA89EDB2469D7AA120" +
            "301E311C301A060355040313135253414B65795472616E736665724361706931" +
            "82105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E03021D0500038181" +
            "0081E5535D8ECEEF265ACBC82F6C5F8BC9D84319265F3CCF23369FA533C8DC19" +
            "38952C5931662D9ECD8B1E7B81749E48468167E2FCE3D019FA70D54646975B6D" +
            "C2A3BA72D5A5274C1866DA6D7A5DF47938E034A075D11957D653B5C78E5291E4" +
            "401045576F6D4EDA81BEF3C369AF56121E49A083C8D1ADB09F291822E99A4296" +
            "46318201803082017C0201013032301E311C301A060355040313135253414B65" +
            "795472616E73666572436170693102105D2FFFF863BABC9B4D3C80AB178A4CCA" +
            "300906052B0E03021A0500300D06092A864886F70D01010105000481805A1717" +
            "621D450130B3463662160EEC06F7AE77E017DD95F294E97A0BDD433FE6B2CCB3" +
            "4FAAC33AEA50BFD7D9E78DC7174836284619F744278AE77B8495091E096EEF68" +
            "2D9CA95F6E81C7DDCEDDA6A12316B453C894B5000701EB09DF57A53B733A4E80" +
            "DA27FA710870BD88C86E2FDB9DCA14D18BEB2F0C87E9632ABF02BE2FE3A181A5" +
            "3081A206092A864886F70D010906318194308191020101301C30173115301306" +
            "03550403130C44756D6D79205369676E6572020100300906052B0E03021A0500" +
            "A03F301806092A864886F70D010903310B06092A864886F70D01070130230609" +
            "2A864886F70D010904311604148C054D6DF2B08E69A86D8DB23C1A509123F9DB" +
            "A4300C06082B060105050706020500041466124B3D99FE06A19BBD3C83C593AB" +
            "55D875E28B").HexToByteArray();

        public static readonly byte[] UnsortedSignerInfos = (
            "30820B1E06092A864886F70D010702A0820B0F30820B0B020103310B30090605" +
            "2B0E03021A0500301006092A864886F70D010701A003040107A0820540308202" +
            "0C30820179A00302010202105D2FFFF863BABC9B4D3C80AB178A4CCA30090605" +
            "2B0E03021D0500301E311C301A060355040313135253414B65795472616E7366" +
            "65724361706931301E170D3135303431353037303030305A170D323530343135" +
            "3037303030305A301E311C301A060355040313135253414B65795472616E7366" +
            "6572436170693130819F300D06092A864886F70D010101050003818D00308189" +
            "02818100AA272700586C0CC41B05C65C7D846F5A2BC27B03E301C37D9BFF6D75" +
            "B6EB6671BA9596C5C63BA2B1AF5C318D9CA39E7400D10C238AC72630579211B8" +
            "6570D1A1D44EC86AA8F6C9D2B4E283EA3535923F398A312A23EAEACD8D34FAAC" +
            "A965CD910B37DA4093EF76C13B337C1AFAB7D1D07E317B41A336BAA4111299F9" +
            "9424408D0203010001A3533051304F0603551D0104483046801015432DB116B3" +
            "5D07E4BA89EDB2469D7AA120301E311C301A060355040313135253414B657954" +
            "72616E73666572436170693182105D2FFFF863BABC9B4D3C80AB178A4CCA3009" +
            "06052B0E03021D05000381810081E5535D8ECEEF265ACBC82F6C5F8BC9D84319" +
            "265F3CCF23369FA533C8DC1938952C5931662D9ECD8B1E7B81749E48468167E2" +
            "FCE3D019FA70D54646975B6DC2A3BA72D5A5274C1866DA6D7A5DF47938E034A0" +
            "75D11957D653B5C78E5291E4401045576F6D4EDA81BEF3C369AF56121E49A083" +
            "C8D1ADB09F291822E99A4296463082032C30820214A003020102020900E0D8AB" +
            "6819D7306E300D06092A864886F70D01010B0500303831363034060355040313" +
            "2D54776F2074686F7573616E6420666F7274792065696768742062697473206F" +
            "662052534120676F6F646E657373301E170D3137313130333233353131355A17" +
            "0D3138313130333233353131355A3038313630340603550403132D54776F2074" +
            "686F7573616E6420666F7274792065696768742062697473206F662052534120" +
            "676F6F646E65737330820122300D06092A864886F70D01010105000382010F00" +
            "3082010A028201010096C114A5898D09133EF859F89C1D848BA8CB5258793E05" +
            "B92D499C55EEFACE274BBBC26803FB813B9C11C6898153CC1745DED2C4D2672F" +
            "807F0B2D957BC4B65EBC9DDE26E2EA7B2A6FE9A7C4D8BD1EF6032B8F0BB6AA33" +
            "C8B57248B3D5E3901D8A38A283D7E25FF8E6F522381EE5484234CFF7B30C1746" +
            "35418FA89E14C468AD89DCFCBBB535E5AF53510F9EA7F9DA8C1B53375B6DAB95" +
            "A291439A5648726EE1012E41388E100691642CF6917F5569D8351F2782F435A5" +
            "79014E8448EEA0C4AECAFF2F476799D88457E2C8BCB56E5E128782B4FE26AFF0" +
            "720D91D52CCAFE344255808F5271D09F784F787E8323182080915BE0AE15A71D" +
            "66476D0F264DD084F30203010001A3393037301D0603551D0E04160414745B5F" +
            "12EF962E84B897E246D399A2BADEA9C5AC30090603551D1304023000300B0603" +
            "551D0F040403020780300D06092A864886F70D01010B0500038201010087A15D" +
            "F37FBD6E9DED7A8FFF25E60B731F635469BA01DD14BC03B2A24D99EFD8B894E9" +
            "493D63EC88C496CB04B33DF25222544F23D43F4023612C4D97B719C1F9431E4D" +
            "B7A580CDF66A3E5F0DAF89A267DD187ABFFB08361B1F79232376AA5FC5AD384C" +
            "C2F98FE36C1CEA0B943E1E3961190648889C8ABE8397A5A338843CBFB1D8B212" +
            "BE46685ACE7B80475CC7C97FC0377936ABD5F664E9C09C463897726650711A11" +
            "10FA9866BC1C278D95E5636AB96FAE95CCD67FD572A8C727E2C03E7B24245731" +
            "8BEC1BE52CA5BD9454A0A41140AE96ED1C56D220D1FD5DD3B1B4FB2AA0E04FC9" +
            "4F7E3C7D476F298962245563953AD7225EDCEAC8B8509E49292E62D8BF318205" +
            "A1308202FB0201038014745B5F12EF962E84B897E246D399A2BADEA9C5AC3009" +
            "06052B0E03021A0500300D06092A864886F70D0101010500048201005E03C5E2" +
            "E736792EFB1C8632C3A864AA6F0E930717FE02C755C0F94DC671244A371926F6" +
            "09878DC8CBFCBA6F83A841B24F48952DA5344F2210BFE9B744E3367B1F8399C8" +
            "96F675923A57E084EBD7DC76A24A1530CD513F0DF6A7703246BF335CC3D09776" +
            "442942150F1C31B9B212AF48850B44B95EB5BD64105F09723EF6AD4711FD81CD" +
            "1FC0418E68EA4428CED9E184126761BF2B25756B6D9BC1A0530E56D38F2A0B78" +
            "3F21D6A5C0703C38F29A2B701B13CAFFCA1DC21C39059E4388E54AEA2519C4E8" +
            "83C7A6BD78200DCB931CA6AB3D18DBBF46A5444C89B6DFE2F48F32C44BA9C030" +
            "F399AC677AA323203137D33CEBFBF1BBF9A506309953B23C4100CA7CA18201C0" +
            "308201BC06092A864886F70D010906318201AD308201A9020101304530383136" +
            "30340603550403132D54776F2074686F7573616E6420666F7274792065696768" +
            "742062697473206F662052534120676F6F646E657373020900E0D8AB6819D730" +
            "6E300906052B0E03021A0500A03F301806092A864886F70D010903310B06092A" +
            "864886F70D010701302306092A864886F70D0109043116041481BF56A6550A60" +
            "A649B0D97971C49897635953D0300D06092A864886F70D010101050004820100" +
            "6E41B7585FEB419005362FEAAAAFB2059E98F8905221A7564F7B0B5510CB221D" +
            "F3DD914A4CD441EAC1C6746A6EC4FC8399C12A61C6B0F50DDA090F564F3D65B2" +
            "6D4BDBC1CE3D39CF47CF33B0D269D15A9FAF2169C60887C3E2CC9828B5E16D45" +
            "DC27A94BAF8D6650EE63D2DBB7DA319B3F61DD18E28AF6FE6DF2CC15C2910BD6" +
            "0B7E038F2C6E8BAEC35CBBBF9484D4C76ECE041DF534B8713B6537854EFE6D58" +
            "41768CCBB9A3B729FDDAE07780CB143A3EE5972DCDDF60A38C65CD3FFF35D1B6" +
            "B76227C1B53831773DA441603F4FB5764D33AADE102F9B85D2CDAEC0E3D6C6E8" +
            "C24C434BFAA3E12E02202142784ED0EB2D9CDCC276D21474747DCD3E4F4D54FC" +
            "3081D40201013032301E311C301A060355040313135253414B65795472616E73" +
            "666572436170693102105D2FFFF863BABC9B4D3C80AB178A4CCA300906052B0E" +
            "03021A0500300D06092A864886F70D01010105000481805EB33C6A9ED5B62240" +
            "90C431E79F51D70B4F2A7D31ED4ED8C3465F6E01281C3FFA44116238B2D168D8" +
            "9154136DDB8B4EB31EA685FB719B7384510F5EF077A10DE6A5CA86F4F6D28B58" +
            "79AFD6CFF0BDA005C2D7CFF53620D28988CBAA44F18AA2D50229FA930B0A7262" +
            "D780DFDEC0334A97DF872F1D95087DC11A881568AF5B88308201C70201013045" +
            "3038313630340603550403132D54776F2074686F7573616E6420666F72747920" +
            "65696768742062697473206F662052534120676F6F646E657373020900E0D8AB" +
            "6819D7306E300906052B0E03021A0500A05D301806092A864886F70D01090331" +
            "0B06092A864886F70D010701301C06092A864886F70D010905310F170D313731" +
            "3130393136303934315A302306092A864886F70D010904311604145D1BE7E9DD" +
            "A1EE8896BE5B7E34A85EE16452A7B4300D06092A864886F70D01010105000482" +
            "01000BB9410F23CFD9C1FCB16179612DB871224F5B88A8E2C012DCDBB3699780" +
            "A3311FD330FFDD6DF1434C52DADD6E07D81FEF145B806E71AF471223914B98CD" +
            "588CCCDFB50ABE3D991B11D62BD83DE158A9001BAED3549BC49B8C204D25C17B" +
            "D042756B026692959E321ACC1AFE6BF52C9356FD49936116D2B3D1F6569F8A8B" +
            "F0FBB2E403AD5788681F3AD131E57390ACB9B8C2EA0BE717F22EFE577EFB1063" +
            "6AC465469191B7E4B3F03CF8DC6C310A20D2B0891BC27350C7231BC2EAABF129" +
            "83755B4C0EDF8A0EE99A615D4E8B381C67A7CDB1405D98C2A6285FEDCED5A65F" +
            "C45C31CD33E3CEB96223DB45E9156B9BD7C8E442C40ED1BB6866C03548616061" +
            "3DAF").HexToByteArray();

        public static byte[] OneDsa1024 = (
            "3082044206092A864886F70D010702A08204333082042F020103310B30090605" +
            "2B0E03021A0500302406092A864886F70D010701A01704154D6963726F736F66" +
            "7420436F72706F726174696F6EA08203913082038D3082034AA0030201020209" +
            "00AB740A714AA83C92300B060960864801650304030230818D310B3009060355" +
            "040613025553311330110603550408130A57617368696E67746F6E3110300E06" +
            "0355040713075265646D6F6E64311E301C060355040A13154D6963726F736F66" +
            "7420436F72706F726174696F6E3120301E060355040B13172E4E455420467261" +
            "6D65776F726B2028436F7265465829311530130603550403130C313032342D62" +
            "697420445341301E170D3135313132353134343030335A170D31353132323531" +
            "34343030335A30818D310B300906035504061302555331133011060355040813" +
            "0A57617368696E67746F6E3110300E060355040713075265646D6F6E64311E30" +
            "1C060355040A13154D6963726F736F667420436F72706F726174696F6E312030" +
            "1E060355040B13172E4E4554204672616D65776F726B2028436F726546582931" +
            "1530130603550403130C313032342D62697420445341308201B73082012C0607" +
            "2A8648CE3804013082011F02818100AEE3309FC7C9DB750D4C3797D333B3B9B2" +
            "34B462868DB6FFBDED790B7FC8DDD574C2BD6F5E749622507AB2C09DF5EAAD84" +
            "859FC0706A70BB8C9C8BE22B4890EF2325280E3A7F9A3CE341DBABEF6058D063" +
            "EA6783478FF8B3B7A45E0CA3F7BAC9995DCFDDD56DF168E91349130F719A4E71" +
            "7351FAAD1A77EAC043611DC5CC5A7F021500D23428A76743EA3B49C62EF0AA17" +
            "314A85415F0902818100853F830BDAA738465300CFEE02418E6B07965658EAFD" +
            "A7E338A2EB1531C0E0CA5EF1A12D9DDC7B550A5A205D1FF87F69500A4E4AF575" +
            "9F3F6E7F0C48C55396B738164D9E35FB506BD50E090F6A497C70E7E868C61BD4" +
            "477C1D62922B3DBB40B688DE7C175447E2E826901A109FAD624F1481B276BF63" +
            "A665D99C87CEE9FD06330381840002818025B8E7078E149BAC35266762362002" +
            "9F5E4A5D4126E336D56F1189F9FF71EA671B844EBD351514F27B69685DDF716B" +
            "32F102D60EA520D56F544D19B2F08F5D9BDDA3CBA3A73287E21E559E6A075861" +
            "94AFAC4F6E721EDCE49DE0029627626D7BD30EEB337311DB4FF62D7608997B6C" +
            "C32E9C42859820CA7EF399590D5A388C48A330302E302C0603551D1104253023" +
            "87047F00000187100000000000000000000000000000000182096C6F63616C68" +
            "6F7374300B0609608648016503040302033000302D021500B9316CC7E05C9F79" +
            "197E0B41F6FD4E3FCEB72A8A0214075505CCAECB18B7EF4C00F9C069FA3BC780" +
            "14DE31623060020103801428A2CB1D204C2656A79C931EFAE351AB548248D030" +
            "0906052B0E03021A0500300906072A8648CE380403042F302D021476DCB780CE" +
            "D5B308A3630726A85DB97FBC50DFD1021500CDF2649B50500BB7428B9DCA6BEF" +
            "2C7E7EF1B79C").HexToByteArray();
    }
}
