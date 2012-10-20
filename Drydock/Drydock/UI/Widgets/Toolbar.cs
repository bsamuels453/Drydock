#region

using System;
using System.Diagnostics;
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

        readonly IToolbarTool[] _buttonTools;

        readonly Point _position;
        readonly Point _buttonSize;
        readonly IToolbarTool _nullTool;
        readonly int _numButtons;
        readonly ToolbarOrientation _orientation;

        public Button[] ToolbarButtons; //be nice to find a way to make this readonly to public since properties cant do it
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

            _position = ctorData.Position;
            _buttonSize = ctorData.ButtonSize;
            _orientation = ctorData.Orientation;
            _numButtons = ctorData.NumButtons;

            #endregion

            #region create the buttons

            var buttonGen = new ButtonGenerator("DToolbarButton.json");
            ToolbarButtons = new Button[ctorData.NumButtons];

            int xPos = _position.X;
            int yPos = _position.Y;
            int xIncrement = 0, yIncrement = 0;
            if (_orientation == ToolbarOrientation.Horizontal)
                xIncrement = _buttonSize.X;
            else
                yIncrement = _buttonSize.Y;

            for (int i = 0; i < ctorData.NumButtons; i++){
                buttonGen.Identifier = i;
                buttonGen.X = xPos;
                buttonGen.Y = yPos;

                ToolbarButtons[i] = buttonGen.GenerateButton();

                xPos += xIncrement;
                yPos += yIncrement;
            }
            foreach (var button in ToolbarButtons){
                button.OnLeftClickDispatcher += HandleButtonClick;
            }

            #endregion

            #region finalize construction

            _nullTool = new NullTool();
            _activeTool = _nullTool;

            _buttonTools = new IToolbarTool[_numButtons];
            for (int i = 0; i < _numButtons; i++)
                _buttonTools[i] = _nullTool;

            #endregion
        }

        #region IInputUpdates Members

        public void UpdateInput(ref ControlState state){
            _activeTool.UpdateInput(ref state);
        }

        #endregion

        #region ILogicUpdates Members

        public void UpdateLogic(double timeDelta){
            _activeTool.UpdateLogic(timeDelta);
        }

        #endregion

        public void SetButtonTool(int buttonIdentifier, IToolbarTool tool){
            Debug.Assert(buttonIdentifier < _buttonTools.Length);
            _buttonTools[buttonIdentifier] = tool;
        }

        public void ClearTool(){
            _activeTool.Disable();
            _activeTool = _nullTool;
        }

        void HandleButtonClick(int identifier){
            Debug.Assert(identifier < _buttonTools.Length);
            _activeTool.Disable();
            _buttonTools[identifier].Enable();
            _activeTool = _buttonTools[identifier];
        }

        #region Nested type: ToolbarCtorData

        struct ToolbarCtorData{
            // ReSharper disable UnassignedField.Local
            //public Color BackgroundColor; //unimplemented
            public string[] ButtonIcons;
            public Point Position;
            public Point ButtonSize;
            public int NumButtons;
            //[JsonConverter(typeof(ToolbarOrientationConverter))]
            public ToolbarOrientation Orientation;
            // ReSharper restore UnassignedField.Local
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