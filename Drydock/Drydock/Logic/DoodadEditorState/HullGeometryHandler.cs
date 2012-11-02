#region

using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Logic.DoodadEditorState.Tools;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    /// <summary>
    /// this class handles the display of the prototype airship and all of its components
    /// </summary>
    internal class HullGeometryHandler : IInputUpdates{
        readonly Button _deckDownButton;

        readonly ShipGeometryBuffer[] _deckFloorBuffers;
        readonly Button _deckUpButton;
        readonly ShipGeometryBuffer[] _hullBuffers;

        readonly int _numDecks;
        readonly WireframeBuffer _selectionBuff;

        public readonly ObjectBuffer[] WallBuffers;
        public readonly List<WallIdentifier>[] WallPositions;
        public IntRef VisibleDecks;

        public HullGeometryHandler(HullGeometryInfo geometryInfo){
            //CullMode.CullClockwiseFace
            _hullBuffers = geometryInfo.HullWallBuffers;
            _deckFloorBuffers = geometryInfo.DeckFloorBuffers;
            _numDecks = geometryInfo.NumDecks;
            VisibleDecks = new IntRef();
            VisibleDecks.Value = _numDecks;
            WallPositions = new List<WallIdentifier>[_numDecks+1];
            for (int i = 0; i < WallPositions.Length; i++){
                WallPositions[i] = new List<WallIdentifier>();
            }

            _selectionBuff = new WireframeBuffer(12, 12, 6);
            _selectionBuff.Indexbuffer.SetData(new[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11});

            //override default lighting
            foreach (var buffer in _deckFloorBuffers){
                buffer.DiffuseDirection = new Vector3(0, 1, 0);
            }
            foreach (var buffer in _hullBuffers){
                buffer.DiffuseDirection = new Vector3(0, -1, 1);
                buffer.CullMode = CullMode.None;
            }

            var buttonGen = new ButtonGenerator("ToolbarButton.json");
            buttonGen.X = 50;
            buttonGen.Y = 50;
            buttonGen.TextureName = "uparrow";
            _deckUpButton = buttonGen.GenerateButton();
            buttonGen.Y = 50 + 64;
            buttonGen.TextureName = "downarrow";
            _deckDownButton = buttonGen.GenerateButton();
            _deckUpButton.OnLeftClickDispatcher += AddVisibleLevel;
            _deckDownButton.OnLeftClickDispatcher += RemoveVisibleLevel;

            WallBuffers = new ObjectBuffer[_numDecks + 1];
            for (int i = 0; i < WallBuffers.Count(); i++){
                int potentialWalls = geometryInfo.FloorVertexes[i].Count()*2;
                WallBuffers[i] = new ObjectBuffer(potentialWalls, 10, 20, 30, "whiteborder");
            }
        }

        #region IInputUpdates Members

        public void UpdateInput(ref ControlState state) { }

        #endregion

        void AddVisibleLevel(int identifier){
            if (VisibleDecks.Value != _numDecks){
                //todo: linq this
                var tempFloorBuff = _deckFloorBuffers.Reverse().ToArray();
                var tempWallBuff = _hullBuffers.Reverse().ToArray();
                var tempWWallBuff = WallBuffers.Reverse().ToArray();

                for (int i = 0; i < tempFloorBuff.Count(); i++){
                    if (tempFloorBuff[i].IsEnabled == false){
                        VisibleDecks.Value++;
                        tempFloorBuff[i].IsEnabled = true;
                        tempWallBuff[i].CullMode = CullMode.None;
                        tempWWallBuff[i].IsEnabled = true;
                        break;
                    }
                }
            }
        }

        void RemoveVisibleLevel(int identifier){
            if (VisibleDecks.Value != 0){
                for (int i = 0; i < _deckFloorBuffers.Count(); i++){
                    if (_deckFloorBuffers[i].IsEnabled){
                        VisibleDecks.Value--;
                        _deckFloorBuffers[i].IsEnabled = false;
                        _hullBuffers[i].CullMode = CullMode.CullClockwiseFace;
                        WallBuffers[i].IsEnabled = false;
                        break;
                    }
                }
            }
        }
    }
}