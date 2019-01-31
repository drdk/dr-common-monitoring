using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DR.Common.Monitoring.Web.Test
{
    internal static class Helper
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
            }
        };

        public static string ToJson<T>(this T x)
        {
            return JsonConvert.SerializeObject(x, JsonSerializerSettings);
        }

        public static T FromJsonTo<T>(this string x)
        {
            return JsonConvert.DeserializeObject<T>(x);
        }

        public static string ToXml<T>(this T x)
        {
            var xs = new XmlSerializer(typeof(T));

            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings {Indent = true}))
                {
                    xs.Serialize(xw, x);
                    return sw.ToString();
                }
            }
        }
    }
}
