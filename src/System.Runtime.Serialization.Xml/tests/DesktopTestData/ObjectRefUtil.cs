using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DesktopTestData
{
    public class ObjectRefUtil
    {
        public static Dictionary<DataContract, List<RefData>> GetReferenceCounts(object data)
        {
            Dictionary<DataContract, List<RefData>> nonRefdValues = new Dictionary<DataContract, List<RefData>>();
            return GetReferenceCounts(data, ref nonRefdValues);
        }

        public static Dictionary<DataContract, List<RefData>> GetReferenceCounts(object data, ref Dictionary<DataContract, List<RefData>> nonRefdValues)
        {
            Dictionary<DataContract, List<RefData>> alreadyRefdValues = new Dictionary<DataContract, List<RefData>>();
            Type type = data.GetType();
            DataContract dataContract = DataContract.GetDataContract(type, supportCollectionDataContract);
            refStack.Clear();
            FindAndAddRefd(data, dataContract, ref alreadyRefdValues, ref nonRefdValues);
            return alreadyRefdValues;
        }

        public static bool IsEqual(Dictionary<DataContract, List<RefData>> alreadyRefdValues1, Dictionary<DataContract, List<RefData>> alreadyRefdValues2)
        {
            if (alreadyRefdValues1.Count != alreadyRefdValues2.Count) return false;

            foreach (KeyValuePair<DataContract, List<RefData>> kp in alreadyRefdValues1)
            {
                if (alreadyRefdValues2.ContainsKey(kp.Key))
                {
                    if (alreadyRefdValues2[kp.Key].Count != kp.Value.Count)
                    {
                        return false;
                    }
                    for (int i = 0; i < kp.Value.Count; i++)
                    {
                        if (alreadyRefdValues2[kp.Key][i].RefCount != kp.Value[i].RefCount)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        static Stack<RefData> refStack = new Stack<RefData>();
        /// <summary>
        /// This method will be called recursively through the helper methods.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataContract"></param>
        /// <param name="alreadyRefdValues"></param>
        /// <param name="nonRefdValues"></param>
        static void FindAndAddRefd(object data, DataContract dataContract, ref Dictionary<DataContract, List<RefData>> alreadyRefdValues
                                        , ref Dictionary<DataContract, List<RefData>> nonRefdValues)
        {
            RefData refData = new RefData(data);

            FindRefUpdateRef(refData, dataContract, ref alreadyRefdValues, ref nonRefdValues);

            if (refStack.Contains(refData))
            {
                return; // Stop Cycles
            }
            else
            {
                refStack.Push(refData);
            }

            FindRefHandleMembers(data, dataContract, ref alreadyRefdValues, ref nonRefdValues);

            refStack.Pop();
        }

        #region Helper Methods for FindAndAddRefd
        public static bool supportCollectionDataContract = true;
        static void FindRefUpdateRef(RefData refData, DataContract dataContract, ref Dictionary<DataContract, List<RefData>> alreadyRefdValues
                                        , ref Dictionary<DataContract, List<RefData>> nonRefdValues)
        {
            if (dataContract.IsReference)
            {
                if (alreadyRefdValues.ContainsKey(dataContract))
                {
                    if (!alreadyRefdValues[dataContract].Contains(refData))
                    {
                        alreadyRefdValues[dataContract].Add(refData);
                    }
                    else
                    {
                        alreadyRefdValues[dataContract][alreadyRefdValues[dataContract].IndexOf(refData)].RefCount++;
                    }
                }
                else
                {
                    List<RefData> list = new List<RefData>();
                    list.Add(refData);
                    alreadyRefdValues.Add(dataContract, list);
                }
            }
            else if (!(dataContract is PrimitiveDataContract)) // Dont track references for Primitives
            {
                if (nonRefdValues.ContainsKey(dataContract))
                {
                    if (!nonRefdValues[dataContract].Contains(refData))
                    {
                        nonRefdValues[dataContract].Add(refData);
                    }
                    else
                    {
                        nonRefdValues[dataContract][nonRefdValues[dataContract].IndexOf(refData)].RefCount++;
                    }
                }
                else
                {
                    List<RefData> list = new List<RefData>();
                    list.Add(refData);
                    nonRefdValues.Add(dataContract, list);
                }
            }
        }
        static void FindRefHandleMembers(object data, DataContract dataContract, ref Dictionary<DataContract, List<RefData>> alreadyRefdValues
                                        , ref Dictionary<DataContract, List<RefData>> nonRefdValues)
        {
            if (typeof(IXmlSerializable).IsAssignableFrom(dataContract.UnderlyingType))
            {
                // DO Nothing
            }
            else if (dataContract is ClassDataContract)
            {
                ClassDataContract classContract = dataContract as ClassDataContract;

                foreach (DataMember member in classContract.Members)
                {
                    // Not using member.MemberTypeContract to accomodate polymorphic cases with known types
                    object memberData = member.GetMemberValue(data);
                    if (memberData != null)
                    {
                        FindAndAddRefd(memberData, DataContract.GetDataContract(memberData.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                    }
                }
            }
            else if (dataContract is ArrayDataContract)
            {
                ArrayDataContract arrayContract = dataContract as ArrayDataContract;
                foreach (object obj in (IEnumerable)data)
                {
                    if (obj != null)
                    {
                        // Not taking the itemContract to accomodate polymorphic cases with known types
                        FindAndAddRefd(obj, DataContract.GetDataContract(obj.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                    }
                }
            }
            else if (dataContract is CollectionDataContract)
            {
                FindRefHandleCollectionDataContractMembers(data, dataContract, ref alreadyRefdValues, ref nonRefdValues);
            }
            else if (dataContract is EnumDataContract || dataContract is PrimitiveDataContract)
            {
                //Nothing to do
            }
            else
            {
                throw new Exception("TestDriver Exception: Type Not Supported");
            }
        }

        static void FindRefHandleCollectionDataContractMembers(object data, DataContract dataContract, ref Dictionary<DataContract, List<RefData>> alreadyRefdValues
                                        , ref Dictionary<DataContract, List<RefData>> nonRefdValues)
        {
            CollectionDataContract collectionContract = dataContract as CollectionDataContract;
            if (!collectionContract.IsDictionary)
            {
                foreach (object obj in (IEnumerable)data)
                {
                    if (obj != null)
                    {
                        // Not taking the itemContract to accomodate polymorphic cases with known types
                        FindAndAddRefd(obj, DataContract.GetDataContract(obj.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                    }
                }
            }
            else
            {
                IDictionary dictionary = data as IDictionary;
                if (dictionary != null)
                {

                    foreach (object key in dictionary.Keys)
                    {
                        if (key != null)
                        {
                            FindAndAddRefd(key, DataContract.GetDataContract(key.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                        }
                    }
                    foreach (object value in dictionary.Values)
                    {
                        if (value != null)
                        {
                            FindAndAddRefd(value, DataContract.GetDataContract(value.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                        }
                    }
                }
                else
                {
                    if (collectionContract.GetEnumeratorMethod != null)
                    {
                        object dictEnumObj = null;
                        try
                        {
                            dictEnumObj = collectionContract.GetEnumeratorMethod.Invoke(data, new object[] { });
                        }
                        catch (Exception) { }
                        IDictionaryEnumerator dictEnum = dictEnumObj as IDictionaryEnumerator;
                        if (dictEnum != null)
                        {
                            while (dictEnum.MoveNext())
                            {
                                FindAndAddRefd(dictEnum.Key, DataContract.GetDataContract(dictEnum.Key.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                            }
                            dictEnum.Reset();
                            while (dictEnum.MoveNext())
                            {
                                if (dictEnum.Value != null)
                                {
                                    FindAndAddRefd(dictEnum.Value, DataContract.GetDataContract(dictEnum.Value.GetType(), supportCollectionDataContract), ref alreadyRefdValues, ref nonRefdValues);
                                }
                            }

                        }
                    }
                    else
                    {
                        throw new Exception("TestDriver Exception: Dictionary CollectionDataCotnract Type Not Supported");
                    }
                }

            }
        }

        #endregion

    }
}
