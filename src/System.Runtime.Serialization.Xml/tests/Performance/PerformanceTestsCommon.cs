﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xunit.Performance;
using System.Xml;
using SerializationTypes;

namespace System.Runtime.Serialization
{
    #region Performance tests configuration types

    public enum TestType
    {
        SimpleType,
        ISerializable,
        Dictionary,
        ListOfInt,
        String,
        ByteArray,
        XmlElement
    }

    public interface IPerfTestSerializer
    {
        void Init(object obj);
        void Serialize(object obj, Stream s);
        void Deserialize(Stream s);
    }

    public abstract class SerializerFactory
    {
        public abstract IPerfTestSerializer GetSerializer();
    }

    #endregion

    public class PerformanceTestCommon
    {

        #region Performance test configuration

        public static IEnumerable<object[]> PerformanceMemberData()
        {
            yield return new object[] { 100, TestType.ByteArray, 1024 };
            yield return new object[] { 5, TestType.ByteArray, 1024 * 1024 };
            yield return new object[] { 10000, TestType.String, 128 };
            yield return new object[] { 10000, TestType.String, 1024 };
            yield return new object[] { 1000, TestType.ListOfInt, 128 };
            yield return new object[] { 1000, TestType.ListOfInt, 1024 };
            yield return new object[] { 1, TestType.ListOfInt, 1024 * 1024 };
            yield return new object[] { 1000, TestType.Dictionary, 128 };
            yield return new object[] { 100, TestType.Dictionary, 1024 };
            yield return new object[] { 10, TestType.SimpleType, 1 };
            yield return new object[] { 1, TestType.SimpleType, 15 };
            yield return new object[] { 10000, TestType.ISerializable, -1 };
            yield return new object[] { 10000, TestType.XmlElement, -1 };
        }

        #endregion

        #region Methods to create object for serialization tests

        public static SimpleTypeWihtMoreProperties CreateObject(int height, int parentId, int currentId, int collectionSize, int childListSize)
        {
            int index = parentId * childListSize + (currentId + 1);
            var obj = new SimpleTypeWihtMoreProperties()
            {
                IntProperty = index,
                StringProperty = index + " string value",
                EnumProperty = (MyEnum)(index % (Enum.GetNames(typeof(MyEnum)).Length)),
                CollectionProperty = new List<string>(),
                SimpleTypeList = new List<SimpleTypeWihtMoreProperties>()
            };
            for (int i = 0; i < collectionSize; ++i)
            {
                obj.CollectionProperty.Add(index + "." + i);
            }
            if (height > 1)
            {
                for (int i = 0; i < childListSize; ++i)
                {
                    obj.SimpleTypeList.Add(CreateObject(height - 1, index, i, collectionSize, childListSize));
                }
            }
            return obj;
        }

        public static byte[] CreateByteArray(int size)
        {
            byte[] obj = new byte[size];
            for (int i = 0; i < obj.Length; ++i)
            {
                obj[i] = (byte)(i % 256);
            }
            return obj;
        }

        public static Dictionary<int, string> CreateDictionaryOfIntString(int count)
        {
            Dictionary<int, string> dictOfIntString = new Dictionary<int, string>();
            for (int i = 0; i < count; ++i)
            {
                dictOfIntString[i] = i.ToString();
            }

            return dictOfIntString;
        }

        public static List<int> CreateListOfInt(int count)
        {
            return Enumerable.Range(0, count).ToList();
        }

        public static object GetSerializationObject(TestType testType, int testSize)
        {
            Object obj = null;
            switch (testType)
            {
                case TestType.ByteArray:
                    obj = CreateByteArray(testSize);
                    break;
                case TestType.Dictionary:
                    obj = CreateDictionaryOfIntString(testSize);
                    break;
                case TestType.ISerializable:
                    obj = new ClassImplementingIXmlSerialiable() { StringValue = "Hello world" };
                    break;
                case TestType.ListOfInt:
                    obj = CreateListOfInt(testSize);
                    break;
                case TestType.SimpleType:
                    obj = CreateObject(testSize, 0, -1, 7, 2);
                    break;
                case TestType.String:
                    obj = new string('k', testSize);
                    break;
                case TestType.XmlElement:
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(@"<html></html>");
                    XmlElement xmlElement = xmlDoc.CreateElement("Element");
                    xmlElement.InnerText = "Element innertext";
                    obj = xmlElement;
                    break;
            }
            return obj;
        }

        #endregion

        #region Methods to run serialization performance tests

        public static void RunSerializationPerformanceTest(int iterations, TestType testType, int testSize, SerializerFactory serializerFactory)
        {
            var obj = GetSerializationObject(testType, testSize);

            var serializer = serializerFactory.GetSerializer();
            serializer.Init(obj);

            using (var stream = new MemoryStream())
            {
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            serializer.Serialize(obj, stream);
                        }
                    }
                }
            }
        }

        public static void RunDeSerializationPerformanceTest(int iterations, TestType testType, int testSize, SerializerFactory serializerFactory)
        {
            var obj = GetSerializationObject(testType, testSize);

            var serializer = serializerFactory.GetSerializer();
            serializer.Init(obj);

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(obj, stream);
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            serializer.Deserialize(stream);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
