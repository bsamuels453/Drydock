#region

using System;
using System.IO;
using Drydock.Logic.DoodadEditorState.Tools;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace Drydock.UI.Widgets{
    internal class Toolbar{
        #region ToolbarOrientation enum

        public enum ToolbarOrientation{
            Horizontal,
            Vertical
        }

        #endregion

        IToolbarTool _activeTool;

        public Toolbar(string path){
            var sr = new StreamReader(path);
            var str = sr.ReadToEnd();
            var ctorData = JsonConvert.DeserializeObject<ToolbarCtorData>(str);


            int f = 5;
        }

        //we need to use a special version of Rectangle here because the XNA one isn't deserializing properly

        #region Nested type: SpecRectangle

        struct SpecRectangle{
            public int Height;
            public int Width;
            public int X;
            public int Y;

            public static explicit operator Rectangle(SpecRectangle f){
                return new Rectangle(f.X, f.Y, f.Width, f.Height);
            }
        }

        #endregion

        #region Nested type: ToolbarCtorData

        struct ToolbarCtorData{
            public Color BackgroundColor;
            public string[] ButtonIcons;
            public SpecRectangle Dimensions;
            public int NumButtons;
            [JsonConverter(typeof (ToolbarOrientationConverter))] public ToolbarOrientation Orientation;
        }

        #endregion

        #region Nested type: ToolbarOrientationConverter

        class ToolbarOrientationConverter : StringEnumConverter{
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
                if ((string) reader.Value == "Horizontal"){
                    return ToolbarOrientation.Horizontal;
                }
                if ((string) reader.Value == "Vertical"){
                    return ToolbarOrientation.Vertical;
                }
                throw new InvalidDataException("Invalid orientation value '" + (string) reader.Value + "' is not defined");
            }

            public override bool CanConvert(Type objectType){
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}