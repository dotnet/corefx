// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace System.Data
{
    /// <summary>
    /// This a locale mapper for non windows
    /// </summary>
    internal class LocaleMapper
    {
        private static readonly Dictionary<int, LocaleCodePage> s_mapper = new Dictionary<int, LocaleCodePage>(431) {
            #region <<Locale Mapper>>
                {1, new LocaleCodePage("ar", 1256)},
                {2, new LocaleCodePage("bg", 1251)},
                {3, new LocaleCodePage("ca", 1252)},
                {4, new LocaleCodePage("zh-CHS", 936)},
                {5, new LocaleCodePage("cs", 1250)},
                {6, new LocaleCodePage("da", 1252)},
                {7, new LocaleCodePage("de", 1252)},
                {8, new LocaleCodePage("el", 1253)},
                {9, new LocaleCodePage("en", 1252)},
                {10, new LocaleCodePage("es", 1252)},
                {11, new LocaleCodePage("fi", 1252)},
                {12, new LocaleCodePage("fr", 1252)},
                {13, new LocaleCodePage("he", 1255)},
                {14, new LocaleCodePage("hu", 1250)},
                {15, new LocaleCodePage("is", 1252)},
                {16, new LocaleCodePage("it", 1252)},
                {17, new LocaleCodePage("ja", 932)},
                {18, new LocaleCodePage("ko", 949)},
                {19, new LocaleCodePage("nl", 1252)},
                {20, new LocaleCodePage("no", 1252)},
                {21, new LocaleCodePage("pl", 1250)},
                {22, new LocaleCodePage("pt", 1252)},
                {23, new LocaleCodePage("rm", 1252)},
                {24, new LocaleCodePage("ro", 1250)},
                {25, new LocaleCodePage("ru", 1251)},
                {26, new LocaleCodePage("hr", 1250)},
                {27, new LocaleCodePage("sk", 1250)},
                {28, new LocaleCodePage("sq", 1250)},
                {29, new LocaleCodePage("sv", 1252)},
                {30, new LocaleCodePage("th", 874)},
                {31, new LocaleCodePage("tr", 1254)},
                {32, new LocaleCodePage("ur", 1256)},
                {33, new LocaleCodePage("id", 1252)},
                {34, new LocaleCodePage("uk", 1251)},
                {35, new LocaleCodePage("be", 1251)},
                {36, new LocaleCodePage("sl", 1250)},
                {37, new LocaleCodePage("et", 1257)},
                {38, new LocaleCodePage("lv", 1257)},
                {39, new LocaleCodePage("lt", 1257)},
                {40, new LocaleCodePage("tg", 1251)},
                {41, new LocaleCodePage("fa", 1256)},
                {42, new LocaleCodePage("vi", 1258)},
                {43, new LocaleCodePage("hy", 0)},
                {44, new LocaleCodePage("az", 1254)},
                {45, new LocaleCodePage("eu", 1252)},
                {46, new LocaleCodePage("hsb", 1252)},
                {47, new LocaleCodePage("mk", 1251)},
                {48, new LocaleCodePage("st", 0)},
                {49, new LocaleCodePage("ts", 0)},
                {50, new LocaleCodePage("tn", 1252)},
                {52, new LocaleCodePage("xh", 1252)},
                {53, new LocaleCodePage("zu", 1252)},
                {54, new LocaleCodePage("af", 1252)},
                {55, new LocaleCodePage("ka", 0)},
                {56, new LocaleCodePage("fo", 1252)},
                {57, new LocaleCodePage("hi", 0)},
                {58, new LocaleCodePage("mt", 0)},
                {59, new LocaleCodePage("se", 1252)},
                {60, new LocaleCodePage("ga", 1252)},
                {62, new LocaleCodePage("ms", 1252)},
                {63, new LocaleCodePage("kk", 0)},
                {64, new LocaleCodePage("ky", 1251)},
                {65, new LocaleCodePage("sw", 1252)},
                {66, new LocaleCodePage("tk", 1250)},
                {67, new LocaleCodePage("uz", 1254)},
                {68, new LocaleCodePage("tt", 1251)},
                {69, new LocaleCodePage("bn", 0)},
                {70, new LocaleCodePage("pa", 0)},
                {71, new LocaleCodePage("gu", 0)},
                {72, new LocaleCodePage("or", 0)},
                {73, new LocaleCodePage("ta", 0)},
                {74, new LocaleCodePage("te", 0)},
                {75, new LocaleCodePage("kn", 0)},
                {76, new LocaleCodePage("ml", 0)},
                {77, new LocaleCodePage("as", 0)},
                {78, new LocaleCodePage("mr", 0)},
                {79, new LocaleCodePage("sa", 0)},
                {80, new LocaleCodePage("mn", 1251)},
                {81, new LocaleCodePage("bo", 0)},
                {82, new LocaleCodePage("cy", 1252)},
                {83, new LocaleCodePage("km", 0)},
                {84, new LocaleCodePage("lo", 0)},
                {85, new LocaleCodePage("my", 0)},
                {86, new LocaleCodePage("gl", 1252)},
                {87, new LocaleCodePage("kok", 0)},
                {89, new LocaleCodePage("sd", 1256)},
                {90, new LocaleCodePage("syr", 0)},
                {91, new LocaleCodePage("si", 0)},
                {92, new LocaleCodePage("chr", 0)},
                {93, new LocaleCodePage("iu", 1252)},
                {94, new LocaleCodePage("am", 0)},
                {95, new LocaleCodePage("tzm", 1252)},
                {97, new LocaleCodePage("ne", 0)},
                {98, new LocaleCodePage("fy", 1252)},
                {99, new LocaleCodePage("ps", 0)},
                {100, new LocaleCodePage("fil", 1252)},
                {101, new LocaleCodePage("dv", 0)},
                {103, new LocaleCodePage("ff", 1252)},
                {104, new LocaleCodePage("ha", 1252)},
                {106, new LocaleCodePage("yo", 1252)},
                {107, new LocaleCodePage("quz", 1252)},
                {108, new LocaleCodePage("nso", 1252)},
                {109, new LocaleCodePage("ba", 1251)},
                {110, new LocaleCodePage("lb", 1252)},
                {111, new LocaleCodePage("kl", 1252)},
                {112, new LocaleCodePage("ig", 1252)},
                {114, new LocaleCodePage("om", 0)},
                {115, new LocaleCodePage("ti", 0)},
                {116, new LocaleCodePage("gn", 1252)},
                {117, new LocaleCodePage("haw", 1252)},
                {119, new LocaleCodePage("so", 0)},
                {120, new LocaleCodePage("ii", 0)},
                {122, new LocaleCodePage("arn", 1252)},
                {124, new LocaleCodePage("moh", 1252)},
                {126, new LocaleCodePage("br", 1252)},
                {127, new LocaleCodePage("", 1252)},
                {128, new LocaleCodePage("ug", 1256)},
                {129, new LocaleCodePage("mi", 0)},
                {130, new LocaleCodePage("oc", 1252)},
                {131, new LocaleCodePage("co", 1252)},
                {132, new LocaleCodePage("gsw", 1252)},
                {133, new LocaleCodePage("sah", 1251)},
                {134, new LocaleCodePage("qut", 1252)},
                {135, new LocaleCodePage("rw", 1252)},
                {136, new LocaleCodePage("wo", 1252)},
                {140, new LocaleCodePage("prs", 1256)},
                {145, new LocaleCodePage("gd", 1252)},
                {146, new LocaleCodePage("ku", 1256)},
                {1025, new LocaleCodePage("ar-SA", 1256)},
                {1026, new LocaleCodePage("bg-BG", 1251)},
                {1027, new LocaleCodePage("ca-ES", 1252)},
                {1028, new LocaleCodePage("zh-TW", 950)},
                {1029, new LocaleCodePage("cs-CZ", 1250)},
                {1030, new LocaleCodePage("da-DK", 1252)},
                {1031, new LocaleCodePage("de-DE", 1252)},
                {1032, new LocaleCodePage("el-GR", 1253)},
                {1033, new LocaleCodePage("en-US", 1252)},
                {1034, new LocaleCodePage("es-ES", 1252)},
                {1035, new LocaleCodePage("fi-FI", 1252)},
                {1036, new LocaleCodePage("fr-FR", 1252)},
                {1037, new LocaleCodePage("he-IL", 1255)},
                {1038, new LocaleCodePage("hu-HU", 1250)},
                {1039, new LocaleCodePage("is-IS", 1252)},
                {1040, new LocaleCodePage("it-IT", 1252)},
                {1041, new LocaleCodePage("ja-JP", 932)},
                {1042, new LocaleCodePage("ko-KR", 949)},
                {1043, new LocaleCodePage("nl-NL", 1252)},
                {1044, new LocaleCodePage("nb-NO", 1252)},
                {1045, new LocaleCodePage("pl-PL", 1250)},
                {1046, new LocaleCodePage("pt-BR", 1252)},
                {1047, new LocaleCodePage("rm-CH", 1252)},
                {1048, new LocaleCodePage("ro-RO", 1250)},
                {1049, new LocaleCodePage("ru-RU", 1251)},
                {1050, new LocaleCodePage("hr-HR", 1250)},
                {1051, new LocaleCodePage("sk-SK", 1250)},
                {1052, new LocaleCodePage("sq-AL", 1250)},
                {1053, new LocaleCodePage("sv-SE", 1252)},
                {1054, new LocaleCodePage("th-TH", 874)},
                {1055, new LocaleCodePage("tr-TR", 1254)},
                {1056, new LocaleCodePage("ur-PK", 1256)},
                {1057, new LocaleCodePage("id-ID", 1252)},
                {1058, new LocaleCodePage("uk-UA", 1251)},
                {1059, new LocaleCodePage("be-BY", 1251)},
                {1060, new LocaleCodePage("sl-SI", 1250)},
                {1061, new LocaleCodePage("et-EE", 1257)},
                {1062, new LocaleCodePage("lv-LV", 1257)},
                {1063, new LocaleCodePage("lt-LT", 1257)},
                {1064, new LocaleCodePage("tg-Cyrl-TJ", 1251)},
                {1065, new LocaleCodePage("fa-IR", 1256)},
                {1066, new LocaleCodePage("vi-VN", 1258)},
                {1067, new LocaleCodePage("hy-AM", 0)},
                {1068, new LocaleCodePage("az-Latn-AZ", 1254)},
                {1069, new LocaleCodePage("eu-ES", 1252)},
                {1070, new LocaleCodePage("hsb-DE", 1252)},
                {1071, new LocaleCodePage("mk-MK", 1251)},
                {1072, new LocaleCodePage("st-ZA", 0)},
                {1073, new LocaleCodePage("ts-ZA", 0)},
                {1074, new LocaleCodePage("tn-ZA", 1252)},
                {1076, new LocaleCodePage("xh-ZA", 1252)},
                {1077, new LocaleCodePage("zu-ZA", 1252)},
                {1078, new LocaleCodePage("af-ZA", 1252)},
                {1079, new LocaleCodePage("ka-GE", 0)},
                {1080, new LocaleCodePage("fo-FO", 1252)},
                {1081, new LocaleCodePage("hi-IN", 0)},
                {1082, new LocaleCodePage("mt-MT", 0)},
                {1083, new LocaleCodePage("se-NO", 1252)},
                {1086, new LocaleCodePage("ms-MY", 1252)},
                {1087, new LocaleCodePage("kk-KZ", 0)},
                {1088, new LocaleCodePage("ky-KG", 1251)},
                {1089, new LocaleCodePage("sw-KE", 1252)},
                {1090, new LocaleCodePage("tk-TM", 1250)},
                {1091, new LocaleCodePage("uz-Latn-UZ", 1254)},
                {1092, new LocaleCodePage("tt-RU", 1251)},
                {1093, new LocaleCodePage("bn-IN", 0)},
                {1094, new LocaleCodePage("pa-IN", 0)},
                {1095, new LocaleCodePage("gu-IN", 0)},
                {1096, new LocaleCodePage("or-IN", 0)},
                {1097, new LocaleCodePage("ta-IN", 0)},
                {1098, new LocaleCodePage("te-IN", 0)},
                {1099, new LocaleCodePage("kn-IN", 0)},
                {1100, new LocaleCodePage("ml-IN", 0)},
                {1101, new LocaleCodePage("as-IN", 0)},
                {1102, new LocaleCodePage("mr-IN", 0)},
                {1103, new LocaleCodePage("sa-IN", 0)},
                {1104, new LocaleCodePage("mn-MN", 1251)},
                {1105, new LocaleCodePage("bo-CN", 0)},
                {1106, new LocaleCodePage("cy-GB", 1252)},
                {1107, new LocaleCodePage("km-KH", 0)},
                {1108, new LocaleCodePage("lo-LA", 0)},
                {1109, new LocaleCodePage("my-MM", 0)},
                {1110, new LocaleCodePage("gl-ES", 1252)},
                {1111, new LocaleCodePage("kok-IN", 0)},
                {1114, new LocaleCodePage("syr-SY", 0)},
                {1115, new LocaleCodePage("si-LK", 0)},
                {1116, new LocaleCodePage("chr-Cher-US", 0)},
                {1117, new LocaleCodePage("iu-Cans-CA", 0)},
                {1118, new LocaleCodePage("am-ET", 0)},
                {1121, new LocaleCodePage("ne-NP", 0)},
                {1122, new LocaleCodePage("fy-NL", 1252)},
                {1123, new LocaleCodePage("ps-AF", 0)},
                {1124, new LocaleCodePage("fil-PH", 1252)},
                {1125, new LocaleCodePage("dv-MV", 0)},
                {1128, new LocaleCodePage("ha-Latn-NG", 1252)},
                {1130, new LocaleCodePage("yo-NG", 1252)},
                {1131, new LocaleCodePage("quz-BO", 1252)},
                {1132, new LocaleCodePage("nso-ZA", 1252)},
                {1133, new LocaleCodePage("ba-RU", 1251)},
                {1134, new LocaleCodePage("lb-LU", 1252)},
                {1135, new LocaleCodePage("kl-GL", 1252)},
                {1136, new LocaleCodePage("ig-NG", 1252)},
                {1138, new LocaleCodePage("om-ET", 0)},
                {1139, new LocaleCodePage("ti-ET", 0)},
                {1140, new LocaleCodePage("gn-PY", 1252)},
                {1141, new LocaleCodePage("haw-US", 1252)},
                {1143, new LocaleCodePage("so-SO", 0)},
                {1144, new LocaleCodePage("ii-CN", 0)},
                {1146, new LocaleCodePage("arn-CL", 1252)},
                {1148, new LocaleCodePage("moh-CA", 1252)},
                {1150, new LocaleCodePage("br-FR", 1252)},
                {1152, new LocaleCodePage("ug-CN", 1256)},
                {1153, new LocaleCodePage("mi-NZ", 0)},
                {1154, new LocaleCodePage("oc-FR", 1252)},
                {1155, new LocaleCodePage("co-FR", 1252)},
                {1156, new LocaleCodePage("gsw-FR", 1252)},
                {1157, new LocaleCodePage("sah-RU", 1251)},
                {1158, new LocaleCodePage("qut-GT", 1252)},
                {1159, new LocaleCodePage("rw-RW", 1252)},
                {1160, new LocaleCodePage("wo-SN", 1252)},
                {1164, new LocaleCodePage("prs-AF", 1256)},
                {1169, new LocaleCodePage("gd-GB", 1252)},
                {1170, new LocaleCodePage("ku-Arab-IQ", 1256)},
                {1281, new LocaleCodePage("qps-ploc", 1250)},
                {1534, new LocaleCodePage("qps-ploca", 932)},
                {2049, new LocaleCodePage("ar-IQ", 1256)},
                {2051, new LocaleCodePage("ca-ES-valencia", 1252)},
                {2052, new LocaleCodePage("zh-CN", 936)},
                {2055, new LocaleCodePage("de-CH", 1252)},
                {2057, new LocaleCodePage("en-GB", 1252)},
                {2058, new LocaleCodePage("es-MX", 1252)},
                {2060, new LocaleCodePage("fr-BE", 1252)},
                {2064, new LocaleCodePage("it-CH", 1252)},
                {2067, new LocaleCodePage("nl-BE", 1252)},
                {2068, new LocaleCodePage("nn-NO", 1252)},
                {2070, new LocaleCodePage("pt-PT", 1252)},
                {2072, new LocaleCodePage("ro-MD", 0)},
                {2074, new LocaleCodePage("sr-Latn-CS", 1250)},
                {2077, new LocaleCodePage("sv-FI", 1252)},
                {2080, new LocaleCodePage("ur-IN", 0)},
                {2092, new LocaleCodePage("az-Cyrl-AZ", 1251)},
                {2094, new LocaleCodePage("dsb-DE", 1252)},
                {2098, new LocaleCodePage("tn-BW", 1252)},
                {2107, new LocaleCodePage("se-SE", 1252)},
                {2108, new LocaleCodePage("ga-IE", 1252)},
                {2110, new LocaleCodePage("ms-BN", 1252)},
                {2115, new LocaleCodePage("uz-Cyrl-UZ", 1251)},
                {2117, new LocaleCodePage("bn-BD", 0)},
                {2118, new LocaleCodePage("pa-Arab-PK", 1256)},
                {2121, new LocaleCodePage("ta-LK", 0)},
                {2128, new LocaleCodePage("mn-Mong-CN", 0)},
                {2137, new LocaleCodePage("sd-Arab-PK", 1256)},
                {2141, new LocaleCodePage("iu-Latn-CA", 1252)},
                {2143, new LocaleCodePage("tzm-Latn-DZ", 1252)},
                {2145, new LocaleCodePage("ne-IN", 0)},
                {2151, new LocaleCodePage("ff-Latn-SN", 1252)},
                {2155, new LocaleCodePage("quz-EC", 1252)},
                {2163, new LocaleCodePage("ti-ER", 0)},
                {2305, new LocaleCodePage("qps-Latn-x-sh", 1252)},
                {2559, new LocaleCodePage("qps-plocm", 1256)},
                {3073, new LocaleCodePage("ar-EG", 1256)},
                {3076, new LocaleCodePage("zh-HK", 950)},
                {3079, new LocaleCodePage("de-AT", 1252)},
                {3081, new LocaleCodePage("en-AU", 1252)},
                {3082, new LocaleCodePage("es-ES", 1252)},
                {3084, new LocaleCodePage("fr-CA", 1252)},
                {3098, new LocaleCodePage("sr-Cyrl-CS", 1251)},
                {3131, new LocaleCodePage("se-FI", 1252)},
                {3152, new LocaleCodePage("mn-Mong-MN", 0)},
                {3179, new LocaleCodePage("quz-PE", 1252)},
                {4097, new LocaleCodePage("ar-LY", 1256)},
                {4100, new LocaleCodePage("zh-SG", 936)},
                {4103, new LocaleCodePage("de-LU", 1252)},
                {4105, new LocaleCodePage("en-CA", 1252)},
                {4106, new LocaleCodePage("es-GT", 1252)},
                {4108, new LocaleCodePage("fr-CH", 1252)},
                {4122, new LocaleCodePage("hr-BA", 1250)},
                {4155, new LocaleCodePage("smj-NO", 1252)},
                {4191, new LocaleCodePage("tzm-Tfng-MA", 0)},
                {5121, new LocaleCodePage("ar-DZ", 1256)},
                {5124, new LocaleCodePage("zh-MO", 950)},
                {5127, new LocaleCodePage("de-LI", 1252)},
                {5129, new LocaleCodePage("en-NZ", 1252)},
                {5130, new LocaleCodePage("es-CR", 1252)},
                {5132, new LocaleCodePage("fr-LU", 1252)},
                {5146, new LocaleCodePage("bs-Latn-BA", 1250)},
                {5179, new LocaleCodePage("smj-SE", 1252)},
                {6145, new LocaleCodePage("ar-MA", 1256)},
                {6153, new LocaleCodePage("en-IE", 1252)},
                {6154, new LocaleCodePage("es-PA", 1252)},
                {6156, new LocaleCodePage("fr-MC", 1252)},
                {6170, new LocaleCodePage("sr-Latn-BA", 1250)},
                {6203, new LocaleCodePage("sma-NO", 1252)},
                {7169, new LocaleCodePage("ar-TN", 1256)},
                {7177, new LocaleCodePage("en-ZA", 1252)},
                {7178, new LocaleCodePage("es-DO", 1252)},
                {7194, new LocaleCodePage("sr-Cyrl-BA", 1251)},
                {7227, new LocaleCodePage("sma-SE", 1252)},
                {8192, new LocaleCodePage("en-US", 1252)},
                {8193, new LocaleCodePage("ar-OM", 1256)},
                {8201, new LocaleCodePage("en-JM", 1252)},
                {8202, new LocaleCodePage("es-VE", 1252)},
                {8204, new LocaleCodePage("fr-RE", 0)},
                {8218, new LocaleCodePage("bs-Cyrl-BA", 1251)},
                {8251, new LocaleCodePage("sms-FI", 1252)},
                {9216, new LocaleCodePage("en-US", 1252)},
                {9217, new LocaleCodePage("ar-YE", 1256)},
                {9225, new LocaleCodePage("en-029", 1252)},
                {9226, new LocaleCodePage("es-CO", 1252)},
                {9228, new LocaleCodePage("fr-CD", 0)},
                {9242, new LocaleCodePage("sr-Latn-RS", 1250)},
                {9275, new LocaleCodePage("smn-FI", 1252)},
                {10240, new LocaleCodePage("en-US", 1252)},
                {10241, new LocaleCodePage("ar-SY", 1256)},
                {10249, new LocaleCodePage("en-BZ", 1252)},
                {10250, new LocaleCodePage("es-PE", 1252)},
                {10252, new LocaleCodePage("fr-SN", 0)},
                {10266, new LocaleCodePage("sr-Cyrl-RS", 1251)},
                {11264, new LocaleCodePage("en-US", 1252)},
                {11265, new LocaleCodePage("ar-JO", 1256)},
                {11273, new LocaleCodePage("en-TT", 1252)},
                {11274, new LocaleCodePage("es-AR", 1252)},
                {11276, new LocaleCodePage("fr-CM", 0)},
                {11290, new LocaleCodePage("sr-Latn-ME", 1250)},
                {12288, new LocaleCodePage("en-US", 1252)},
                {12289, new LocaleCodePage("ar-LB", 1256)},
                {12297, new LocaleCodePage("en-ZW", 1252)},
                {12298, new LocaleCodePage("es-EC", 1252)},
                {12300, new LocaleCodePage("fr-CI", 0)},
                {12314, new LocaleCodePage("sr-Cyrl-ME", 1251)},
                {13312, new LocaleCodePage("en-US", 1252)},
                {13313, new LocaleCodePage("ar-KW", 1256)},
                {13321, new LocaleCodePage("en-PH", 1252)},
                {13322, new LocaleCodePage("es-CL", 1252)},
                {13324, new LocaleCodePage("fr-ML", 0)},
                {14336, new LocaleCodePage("en-US", 1252)},
                {14337, new LocaleCodePage("ar-AE", 1256)},
                {14346, new LocaleCodePage("es-UY", 1252)},
                {14348, new LocaleCodePage("fr-MA", 0)},
                {15360, new LocaleCodePage("en-US", 1252)},
                {15361, new LocaleCodePage("ar-BH", 1256)},
                {15369, new LocaleCodePage("en-HK", 0)},
                {15370, new LocaleCodePage("es-PY", 1252)},
                {15372, new LocaleCodePage("fr-HT", 0)},
                {16384, new LocaleCodePage("en-US", 1252)},
                {16385, new LocaleCodePage("ar-QA", 1256)},
                {16393, new LocaleCodePage("en-IN", 1252)},
                {16394, new LocaleCodePage("es-BO", 1252)},
                {17408, new LocaleCodePage("en-US", 1252)},
                {17417, new LocaleCodePage("en-MY", 1252)},
                {17418, new LocaleCodePage("es-SV", 1252)},
                {18432, new LocaleCodePage("en-US", 1252)},
                {18441, new LocaleCodePage("en-SG", 1252)},
                {18442, new LocaleCodePage("es-HN", 1252)},
                {19456, new LocaleCodePage("en-US", 1252)},
                {19466, new LocaleCodePage("es-NI", 1252)},
                {20490, new LocaleCodePage("es-PR", 1252)},
                {21514, new LocaleCodePage("es-US", 1252)},
                {22538, new LocaleCodePage("es-419", 0)},
                {25626, new LocaleCodePage("bs-Cyrl", 1251)},
                {26650, new LocaleCodePage("bs-Latn", 1250)},
                {27674, new LocaleCodePage("sr-Cyrl", 1251)},
                {28698, new LocaleCodePage("sr-Latn", 1250)},
                {28731, new LocaleCodePage("smn", 1252)},
                {29740, new LocaleCodePage("az-Cyrl", 1251)},
                {29755, new LocaleCodePage("sms", 1252)},
                {30724, new LocaleCodePage("zh", 936)},
                {30740, new LocaleCodePage("nn", 1252)},
                {30746, new LocaleCodePage("bs", 1250)},
                {30764, new LocaleCodePage("az-Latn", 1254)},
                {30779, new LocaleCodePage("sma", 1252)},
                {30787, new LocaleCodePage("uz-Cyrl", 1251)},
                {30800, new LocaleCodePage("mn-Cyrl", 1251)},
                {30813, new LocaleCodePage("iu-Cans", 0)},
                {30815, new LocaleCodePage("tzm-Tfng", 0)},
                {31748, new LocaleCodePage("zh-CHT", 950)},
                {31764, new LocaleCodePage("nb", 1252)},
                {31770, new LocaleCodePage("sr", 1250)},
                {31784, new LocaleCodePage("tg-Cyrl", 1251)},
                {31790, new LocaleCodePage("dsb", 1252)},
                {31803, new LocaleCodePage("smj", 1252)},
                {31811, new LocaleCodePage("uz-Latn", 1254)},
                {31814, new LocaleCodePage("pa-Arab", 1256)},
                {31824, new LocaleCodePage("mn-Mong", 0)},
                {31833, new LocaleCodePage("sd-Arab", 1256)},
                {31836, new LocaleCodePage("chr-Cher", 0)},
                {31837, new LocaleCodePage("iu-Latn", 1252)},
                {31839, new LocaleCodePage("tzm-Latn", 1252)},
                {31847, new LocaleCodePage("ff-Latn", 1252)},
                {31848, new LocaleCodePage("ha-Latn", 1252)},
                {31890, new LocaleCodePage("ku-Arab", 1256)},
                {65663, new LocaleCodePage("x-IV", 1252)},
                {66567, new LocaleCodePage("de-DE", 1252)},
                {66574, new LocaleCodePage("hu-HU", 1250)},
                {66615, new LocaleCodePage("ka-GE", 0)},
                {133124, new LocaleCodePage("zh-CN", 936)},
                {135172, new LocaleCodePage("zh-SG", 936)},
                {136196, new LocaleCodePage("zh-MO", 950)},
                {197636, new LocaleCodePage("zh-TW", 950)},
                {263172, new LocaleCodePage("zh-TW", 950)},
                {263185, new LocaleCodePage("ja-JP", 932)},
                {265220, new LocaleCodePage("zh-HK", 950)},
                {267268, new LocaleCodePage("zh-MO", 950)}
            #endregion
        };

        public static string LcidToLocaleNameInternal(int lcid)
        {
            return s_mapper[lcid].LocaleName;
        }

        public static int LocaleNameToAnsiCodePage(string localeName)
        {
            return s_mapper.FirstOrDefault(t => t.Value.LocaleName == localeName).Value.CodePage;
        }

        public static int GetLcidForLocaleName(string localeName)
        {
            return s_mapper.FirstOrDefault(t => t.Value.LocaleName == localeName).Key;
        }
    }

    internal class LocaleCodePage
    {
        internal readonly string LocaleName;
        internal readonly int CodePage;

        internal LocaleCodePage(string localeName, int codePage)
        {
            this.LocaleName = localeName;
            this.CodePage = codePage;
        }
    }
}
