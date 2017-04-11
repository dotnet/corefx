using System;
using System.Data;
using System.Xml;

namespace DesktopTestData
{
    public static class Util
    {
        #region Resource Helpers - Not the exception resource string, the resource files embedded in the assembly
        public static bool CompareDataSets(System.Data.DataSet i, System.Data.DataSet j)
        {
            return i.GetXml().Equals(j.GetXml());
        }

        public static bool CompareDataTable(DataTable input, DataTable output)
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            input.WriteXml(writer, true);
            string inputXml = writer.ToString();
            writer = new System.IO.StringWriter();
            output.WriteXml(writer, true);
            string outputXml = writer.ToString();

            XmlDocument xd1 = new XmlDocument();
            xd1.LoadXml(inputXml);
            inputXml = xd1.InnerXml;

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(outputXml);
            outputXml = xd.InnerXml;
            return inputXml.Equals(outputXml);
        }
        #endregion
    }

    public static class CreatorSettings
    {
        public static int MaxArrayLength = 10;
        public static int MaxListLength = 10;
        public static int MaxStringLength = 100;
        public static bool CreateOnlyAsciiChars = false;
        public static bool DontCreateSurrogateChars = false;
        public static bool CreateDateTimeWithSubMilliseconds = true;
        public static bool NormalizeEndOfLineOnXmlNodes = false;
        public static double NullValueProbability = 0.01;
        public static bool SetPOCONonPublicSetters = true;
        public static InstanceCreatorSurrogate CreatorSurrogate = null;
    }

    public abstract class InstanceCreatorSurrogate
    {
        /// <summary>
        /// Checks whether this surrogate can create instances of a given type.
        /// </summary>
        /// <param name="type">The type which needs to be created.</param>
        /// <returns>A true value if this surrogate can create the given type; a
        /// false value otherwise.</returns>
        public abstract bool CanCreateInstanceOf(Type type);

        /// <summary>
        /// Creates an instance of the given type.
        /// </summary>
        /// <param name="type">The type to create an instance for.</param>
        /// <param name="rndGen">A Random generator to assist in creating the instance.</param>
        /// <returns>An instance of the given type.</returns>
        public abstract object CreateInstanceOf(Type type, Random rndGen);
    }
}
