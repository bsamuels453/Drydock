#region

using System;
using System.IO;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace Drydock.UI.Widgets{
    internal class Toolbar : ILogicUpdates, IInputUpdates{
        #region ToolbarOrientation enum

        public enum ToolbarOrientation{
            Horizontal,
            Vertical
        }

        #endregion

        readonly Rectangle _dimensions;
        readonly int _numButtons;
        readonly ToolbarOrientation _orientation;

        public Button[] ToolbarButtons;
        IToolbarTool _activeTool;

        public Toolbar(string path){
            var sr = new StreamReader(path);
            var str = sr.ReadToEnd();
            var ctorData = JsonConvert.DeserializeObject<ToolbarCtorData>(str);

            #region some validity checks

            if (ctorData.ButtonIcons == null)
                throw new InvalidDataException("ButtonIcons invalid");
            if (ctorData.NumButtons == 0)
                throw new InvalidDataException("NumButtons invalid");
            if (ctorData.ButtonIcons.Length != ctorData.NumButtons)
                throw new InvalidDataException("NumButtons is not equal to the number of ButtonIcons");

            #endregion

            #region set ctor data

            _dimensions = ctorData.Dimensions;
            _orientation = ctorData.Orientation;
            _numButtons = ctorData.NumButtons;

            #endregion

            #region create the buttons

            var buttonGen = new ButtonGenerator("DToolbarButton.json");
            ToolbarButtons = new Button[ctorData.NumButtons];

            int xPos = _dimensions.X;
            int yPos = _dimensions.Y;
            int xIncrement = 0, yIncrement = 0;
            if (_orientation == ToolbarOrientation.Horizontal)
                xIncrement = _dimensions.Width/_numButtons;
            else
                yIncrement = _dimensions.Height/_numButtons;

            for (int i = 0; i < ctorData.NumButtons; i++){
                buttonGen.X = xPos;
                buttonGen.Y = yPos;

                ToolbarButtons[i] = buttonGen.GenerateButton();

                xPos += xIncrement;
                yPos += yIncrement;
            }

            ToolbarButtons[0].Sprite = "wallbuildicon";

            #endregion
        }

        #region IInputUpdates Members

        public void UpdateInput(ref ControlState state){
            throw new NotImplementedException();
        }

        #endregion

        #region ILogicUpdates Members

        public void UpdateLogic(double timeDelta){
            throw new NotImplementedException();
        }

        #endregion

        //we need to use a special version of Rectangle here because the XNA one isn't deserializing properly

        #region Nested type: SpecRectangle

        struct SpecRectangle{
            public int Height;
            public int Width;
            public int X;
            public int Y;

            public static implicit operator Rectangle(SpecRectangle f){
                return new Rectangle(f.X, f.Y, f.Width, f.Height);
            }
        }

        #endregion

        #region Nested type: ToolbarCtorData

        struct ToolbarCtorData{
            public Color BackgroundColor; //unimplemented
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