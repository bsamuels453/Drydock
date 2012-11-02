#region

using System;
using System.Diagnostics;
using System.IO;
using Drydock.Control;
using Drydock.UI.Components;
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

        readonly Point _buttonSize;
        readonly IToolbarTool[] _buttonTools;
        readonly IToolbarTool _nullTool;
        readonly int _numButtons;
        readonly ToolbarOrientation _orientation;
        readonly Point _position;

        public Button[] ToolbarButtons;
        IToolbarTool _activeTool;

        bool _isEnabled;
        public bool IsEnabled {
            get { return _isEnabled; }
            set{
                _isEnabled = value;
                ClearActiveTool();
                foreach (var button in ToolbarButtons){
                    button.IsEnabled = value;
                }
            }
        }

        public Toolbar(string path){
            var sr = new StreamReader(path);
            var str = sr.ReadToEnd();
            var ctorData = JsonConvert.DeserializeObject<ToolbarCtorData>(str);
            _isEnabled = true;

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

            var buttonGen = new ButtonGenerator(ctorData.ButtonTemplate);
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
                buttonGen.TextureName = ctorData.ButtonIcons[i];

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
            if (IsEnabled){
                _activeTool.UpdateInput(ref state);
            }
        }

        #endregion

        #region ILogicUpdates Members

        public void UpdateLogic(double timeDelta){
            if (IsEnabled){
                _activeTool.UpdateLogic(timeDelta);
            }
        }

        #endregion

        public void BindButtonToTool(int buttonIdentifier, IToolbarTool tool){
            Debug.Assert(buttonIdentifier < _buttonTools.Length);
            _buttonTools[buttonIdentifier] = tool;
        }

        public void ClearActiveTool(){
            _activeTool.Disable();
            _activeTool = _nullTool;
            foreach (var button in ToolbarButtons) {
                button.GetComponent<HighlightComponent>("ClickHoldEffect").UnprocHighlight();
                button.GetComponent<HighlightComponent>("HoverMask").IsEnabled = true;
                button.GetComponent<HighlightComponent>("HoverMask").UnprocHighlight();
            }
        }

        void HandleButtonClick(int identifier){
            if (IsEnabled){
                Debug.Assert(identifier < _buttonTools.Length);

                ClearActiveTool();
                _buttonTools[identifier].Enable();
                ToolbarButtons[identifier].GetComponent<HighlightComponent>("ClickHoldEffect").ProcHighlight();
                ToolbarButtons[identifier].GetComponent<HighlightComponent>("HoverMask").IsEnabled = false;
                _activeTool = _buttonTools[identifier];
            }
        }

        #region Nested type: ToolbarCtorData

        struct ToolbarCtorData{
            //public Color BackgroundColor; //unimplemented
#pragma warning disable 649
            public string[] ButtonIcons;
            public Point Position;
            public Point ButtonSize;
            public int NumButtons;
            //[JsonConverter(typeof(ToolbarOrientationConverter))]
            public ToolbarOrientation Orientation;
            public string ButtonTemplate;
#pragma warning restore 649
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