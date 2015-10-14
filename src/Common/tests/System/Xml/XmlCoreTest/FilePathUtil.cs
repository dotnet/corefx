﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.IO;
using OLEDB.Test.ModuleCore;
using System.Collections.Generic;
using System.Reflection;

namespace XmlCoreTest.Common
{
    public static partial class FilePathUtil
    {
        public static string GetDataPath()
        {
            return GetVariableValue("DataPath");
        }

        public static string GetHttpDataPath()
        {
            return GetVariableValue("HttpDataPath");
        }

        public static string GetFileDataPath()
        {
            return GetVariableValue("FileDataPath");
        }

        public static string GetStandardPath()
        {
            return Path.Combine(GetDataPath(), "StandardTests");
        }

        public static string GetFileStandardPath()
        {
            return GetFileDataPath() + "/" + "StandardTests";
        }

        public static string GetHttpStandardPath()
        {
            return GetHttpDataPath() + "/" + "StandardTests";
        }

        public static string GetTestDataPath()
        {
            return Path.Combine(GetDataPath(), "TestData");
        }

        public static string GetFileTestDataPath()
        {
            return GetFileDataPath() + "/" + "TestData";
        }

        public static string GetHttpTestDataPath()
        {
            return GetHttpDataPath() + "/" + "TestData";
        }

        public static string GetVariableValue(string name)
        {
            string value = CModInfo.Options[name] as string;

            if (value == null || value.Equals(String.Empty))
            {
                value = "";
            }
            return value;
        }

        public static string ExpandVariables(string inputPath)
        {
            StringBuilder resultPath = new StringBuilder();

            int lastPosition = 0, variableStart;
            while (lastPosition < inputPath.Length)
            {
                variableStart = inputPath.IndexOf('$', lastPosition);
                if (variableStart == -1)
                {
                    resultPath.Append(inputPath.Substring(lastPosition));
                    break;
                }
                if (variableStart == inputPath.Length - 1 || inputPath[variableStart + 1] != '(')
                {
                    resultPath.Append(inputPath.Substring(lastPosition, variableStart - lastPosition) + '$');
                    lastPosition = variableStart + 1;
                }
                else
                {
                    int variableEnd = inputPath.IndexOf(')', variableStart);
                    if (variableEnd == -1)
                    {
                        resultPath.Append(inputPath.Substring(lastPosition));
                        break;
                    }
                    string variableValue = GetVariableValue(inputPath.Substring(variableStart + 2, variableEnd - variableStart - 2));
                    if (variableValue != null)
                    {
                        resultPath.Append(variableValue);
                    }
                    lastPosition = variableEnd + 1;
                }
            }

            return resultPath.ToString();
        }

        private static MyDict<string, Stream> s_XmlFileInMemoryCache = null;
        private static MyDict<string, Stream> s_XmlFileInMemoryCacheBackup = null;

        static void initXmlFileCacheIfNotYet()
        {
            if (null == s_XmlFileInMemoryCache)
            {
                s_XmlFileInMemoryCache = new MyDict<string, Stream>();
                s_XmlFileInMemoryCacheBackup = new MyDict<string, Stream>();

                foreach (var file in GetDataFiles())
                {
                    addBytes(file.Item1, file.Item2);
                }
            }
        }

        public static void cacheXml(string filename, string content)
        {
            initXmlFileCacheIfNotYet();
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(content);
            sw.Flush();
            s_XmlFileInMemoryCache[NormalizeFilePath(filename)] = ms;

            MemoryStream msbak = new MemoryStream();
            ms.Position = 0;
            ms.CopyTo(msbak);
            s_XmlFileInMemoryCacheBackup[NormalizeFilePath(filename)] = msbak;
        }

        public static Stream getStream(string filename)
        {
            if (String.IsNullOrWhiteSpace(filename))
                return null;

            initXmlFileCacheIfNotYet();

            string normalizedFileName = NormalizeFilePath(filename);

            Stream s = s_XmlFileInMemoryCache[normalizedFileName];
            if (s == null)
            {
                throw new FileNotFoundException("File Not Found: " + filename);
            }
            if (s.CanSeek)
            {
                s.Position = 0;
                return s;
            }
            else
            {
                Stream msbak = s_XmlFileInMemoryCacheBackup[normalizedFileName];
                MemoryStream msnew = new MemoryStream();
                msbak.Position = 0;
                msbak.CopyTo(msnew);

                s_XmlFileInMemoryCache[normalizedFileName] = msnew;
                msnew.Position = 0;
                return msnew;
            }
        }

        public static void addBytes(string filename, byte[] bytes)
        {
            if (null == filename)
                return;

            initXmlFileCacheIfNotYet();

            var ms = new MemoryStream(bytes);
            s_XmlFileInMemoryCache[NormalizeFilePath(filename)] = ms;

            MemoryStream msbak = new MemoryStream();
            ms.Position = 0;
            ms.CopyTo(msbak);
            s_XmlFileInMemoryCacheBackup[NormalizeFilePath(filename)] = msbak;
        }

        public static void addStream(string filename, Stream s)
        {
            if (null == filename)
                return;

            initXmlFileCacheIfNotYet();

            s_XmlFileInMemoryCache[NormalizeFilePath(filename)] = s;

            MemoryStream msbak = new MemoryStream();
            s.Position = 0;
            s.CopyTo(msbak);
            s_XmlFileInMemoryCacheBackup[NormalizeFilePath(filename)] = msbak;
        }

        static string NormalizeFilePath(string path)
        {
            return path.Replace('/', '\\').ToLower().Trim();
        }
    }
}
