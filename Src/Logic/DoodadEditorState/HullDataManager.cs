#region

using System.Collections.Generic;
using System.Linq;
using Drydock.Logic.DoodadEditorState.Tools;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class HullDataManager{
        #region Delegates

        public delegate void CurDeckChanged(int oldDeck, int newDeck);

        #endregion

        public readonly Vector3 CenterPoint;

        public readonly ObjectBuffer<QuadIdentifier>[] DeckBuffers;
        public readonly List<BoundingBox>[] DeckFloorBoundingBoxes;
        public readonly float DeckHeight;
        public readonly List<Vector3>[] FloorVertexes;
        public readonly ShipGeometryBuffer[] HullBuffers;
        public readonly int NumDecks;
        public readonly ObjectBuffer<ObjectIdentifier>[] WallBuffers;
        public readonly List<ObjectIdentifier>[] WallIdentifiers;
        public readonly float WallResolution;
        int _curDeck;

        public HullDataManager(HullGeometryInfo geometryInfo){
            NumDecks = geometryInfo.NumDecks;
            CurDeck = 0;
            VisibleDecks = NumDecks;
            HullBuffers = geometryInfo.HullWallTexBuffers;
            DeckBuffers = geometryInfo.DeckFloorBuffers;
            DeckFloorBoundingBoxes = geometryInfo.DeckFloorBoundingBoxes;
            FloorVertexes = geometryInfo.FloorVertexes;
            DeckHeight = geometryInfo.DeckHeight;
            WallResolution = geometryInfo.WallResolution;
            CenterPoint = geometryInfo.CenterPoint;


            WallBuffers = new ObjectBuffer<ObjectIdentifier>[NumDecks];
            for (int i = 0; i < WallBuffers.Count(); i++){
                int potentialWalls = geometryInfo.FloorVertexes[i].Count()*2;
                WallBuffers[i] = new ObjectBuffer<ObjectIdentifier>(potentialWalls, 10, 20, 30, "HullWallTex");
            }

            WallIdentifiers = new List<ObjectIdentifier>[NumDecks];
            for (int i = 0; i < WallIdentifiers.Length; i++){
                WallIdentifiers[i] = new List<ObjectIdentifier>();
            }

            //override default lighting
            foreach (var buffer in DeckBuffers){
                buffer.DiffuseDirection = new Vector3(0, 1, 0);
            }
            foreach (var buffer in HullBuffers){
                buffer.DiffuseDirection = new Vector3(0, -1, 1);
                buffer.CullMode = CullMode.None;
            }
        }

        public int VisibleDecks { get; private set; }

        public int CurDeck{
            get { return _curDeck; }
            set{
                //higher curdeck means a lower deck is displayed
                //low curdeck means higher deck displayed
                //highest deck is 0
                int diff = -(value - _curDeck);
                int oldDeck = _curDeck;
                VisibleDecks += diff;
                _curDeck = value;
                if (OnCurDeckChange != null){
                    OnCurDeckChange.Invoke(oldDeck, _curDeck);
                }
            }
        }

        public event CurDeckChanged OnCurDeckChange;

        public void MoveUpOneDeck(){
            if (CurDeck != 0){
                //todo: linq this
                var tempFloorBuff = DeckBuffers.Reverse().ToArray();
                var tempWallBuff = HullBuffers.Reverse().ToArray();
                var tempWWallBuff = WallBuffers.Reverse().ToArray();

                for (int i = 0; i < tempFloorBuff.Count(); i++){
                    if (tempFloorBuff[i].Enabled == false){
                        CurDeck--;
                        tempFloorBuff[i].Enabled = true;
                        tempWallBuff[i].CullMode = CullMode.None;
                        tempWWallBuff[i].Enabled = true;
                        break;
                    }
                }
            }
        }

        public void MoveDownOneDeck(){
            if (CurDeck < NumDecks){
                for (int i = 0; i < DeckBuffers.Count(); i++){
                    if (DeckBuffers[i].Enabled){
                        CurDeck++;
                        DeckBuffers[i].Enabled = false;
                        HullBuffers[i].CullMode = CullMode.CullClockwiseFace;
                        WallBuffers[i].Enabled = false;
                        break;
                    }
                }
            }
        }
    }
}