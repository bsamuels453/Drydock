using System;
using System.IO;
using Drydock.UI.Widgets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Drydock.UI{
    //return (float) d/(float) Math.Pow(10, Depth);
    internal enum DepthLevel{ //can have 10 levels max
        Highlight,
        High,
        Medium,
        Low,
        Border,
        Base,
        Background
    }

    class DepthLevelConverter : StringEnumConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if ((string)reader.Value == "Highlight") {
                return DepthLevel.Highlight;
            }
            if ((string)reader.Value == "High") {
                return DepthLevel.High;
            }
            if ((string)reader.Value == "Medium") {
                return DepthLevel.Medium;
            }
            if ((string)reader.Value == "Low") {
                return DepthLevel.Low;
            }
            if ((string)reader.Value == "Border") {
                return DepthLevel.Border;
            }
            if ((string)reader.Value == "Base") {
                return DepthLevel.Base;
            }
            if ((string)reader.Value == "Background") {
                return DepthLevel.Background;
            }
            throw new InvalidDataException("Invalid depth value '" + (string)reader.Value + "' is not defined");
        }

        public override bool CanConvert(Type objectType) {
            if (objectType == typeof(DepthLevel)){
                return true;
            }
            return false;
        }
    }
}