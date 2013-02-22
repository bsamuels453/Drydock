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
    ///   this class handles the display of the prototype airship and all of its components
    /// </summary>
    internal class DoodadUI : IInputUpdates{
        public readonly ObjectBuffer<ObjectIdentifier>[] WallBuffers;
        public readonly List<ObjectIdentifier>[] WallPositions;
        readonly Button _deckDownButton;

        readonly ObjectBuffer<QuadIdentifier>[] _deckFloorBuffers;
        readonly Button _deckUpButton;
        readonly ShipGeometryBuffer[] _hullBuffers;

        readonly int _numDecks;
        readonly WireframeBuffer _selectionBuff;

        public IntRef VisibleDecks;

        public DoodadUI(HullGeometryInfo geometryInfo){
            //CullMode.CullClockwiseFace
            _hullBuffers = geometryInfo.HullWallTexBuffers;
            _deckFloorBuffers = geometryInfo.DeckFloorBuffers;
            _numDecks = geometryInfo.NumDecks;
            VisibleDecks = new IntRef();
            VisibleDecks.Value = _numDecks;
            WallPositions = new List<ObjectIdentifier>[_numDecks];
            for (int i = 0; i < WallPositions.Length; i++){
                WallPositions[i] = new List<ObjectIdentifier>();
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

            var buttonGen = new ButtonGenerator("ToolbarButton64.json");
            buttonGen.X = 50;
            buttonGen.Y = 50;
            buttonGen.TextureName = "DeckNavArrowUp";
            _deckUpButton = buttonGen.GenerateButton();
            buttonGen.Y = 50 + 64;
            buttonGen.TextureName = "DeckNavArrowDown";
            _deckDownButton = buttonGen.GenerateButton();
            _deckUpButton.OnLeftClickDispatcher += AddVisibleLevel;
            _deckDownButton.OnLeftClickDispatcher += RemoveVisibleLevel;

            WallBuffers = new ObjectBuffer<ObjectIdentifier>[_numDecks];
            for (int i = 0; i < WallBuffers.Count(); i++){
                int potentialWalls = geometryInfo.FloorVertexes[i].Count()*2;
                WallBuffers[i] = new ObjectBuffer<ObjectIdentifier>(potentialWalls, 10, 20, 30, "HullWallTex");
            }
        }

        #region IInputUpdates Members

        public void UpdateInput(ref ControlState state){
        }

        #endregion

        void AddVisibleLevel(int identifier){
            if (VisibleDecks.Value != (_numDecks-1)){
                //todo: linq this
                var tempFloorBuff = _deckFloorBuffers.Reverse().ToArray();
                var tempWallBuff = _hullBuffers.Reverse().ToArray();
                var tempWWallBuff = WallBuffers.Reverse().ToArray();

                for (int i = 0; i < tempFloorBuff.Count(); i++){
                    if (tempFloorBuff[i].Enabled == false){
                        VisibleDecks.Value++;
                        tempFloorBuff[i].Enabled = true;
                        tempWallBuff[i].CullMode = CullMode.None;
                        tempWWallBuff[i].Enabled = true;
                        break;
                    }
                }
            }
        }

        void RemoveVisibleLevel(int identifier){
            if (VisibleDecks.Value != 0){
                for (int i = 0; i < _deckFloorBuffers.Count(); i++){
                    if (_deckFloorBuffers[i].Enabled){
                        VisibleDecks.Value--;
                        _deckFloorBuffers[i].Enabled = false;
                        _hullBuffers[i].CullMode = CullMode.CullClockwiseFace;
                        WallBuffers[i].Enabled = false;
                        break;
                    }
                }
            }
        }
    }
}