#region

using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Drydock.Utilities;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Drydock.Render;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallBuildTool : IToolbarTool{
        readonly IntRefLambda _curDeck;
        readonly BoundingBox[][] _deckFloorBoundingboxes;
        readonly Vector3[][] _deckFloorVertexes;
        readonly WireframeBuffer[] _guideGridBuffers;
        readonly WireframeBuffer _selectionBuff;
        readonly ObjectBuffer[] _wallBuffers;
        readonly List<WallIdentifier>[] _wallIdentifiers;
        readonly ObjectBuffer _tempWallBuffer;

        bool _isEnabled;
        bool _cursorActive;
        bool _isDrawing;
        Vector3 _cursorPosition;
        Vector3 _strokeOrigin;
        Vector3 _strokeEnd;

        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer[] wallBuffers, List<WallIdentifier>[] wallIdentifiers ){
            _isEnabled = false;
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.FloorVertexes;
            _wallBuffers = wallBuffers;
            _wallIdentifiers = wallIdentifiers;
            _tempWallBuffer = new ObjectBuffer(hullInfo.FloorVertexes[0].Count() * 2, 10, 20, 30, "brown");

            _curDeck = new IntRefLambda(visibleDecksRef, input => hullInfo.NumDecks - input);

            visibleDecksRef.RefModCallback += VisibleDeckChange;
            _selectionBuff = new WireframeBuffer(2, 2, 1);
            var selectionIndicies = new[]{0, 1};
            _selectionBuff.Indexbuffer.SetData(selectionIndicies);
            _selectionBuff.IsEnabled = false;

            _guideGridBuffers = new WireframeBuffer[hullInfo.NumDecks + 1];

            for (int i = 0; i < hullInfo.NumDecks + 1; i++){
                #region indicies

                int numBoxes = _deckFloorBoundingboxes[i].Count();
                _guideGridBuffers[i] = new WireframeBuffer(8*numBoxes, 8*numBoxes, 4*numBoxes);
                var guideDotIndicies = new int[8*numBoxes];
                for (int si = 0; si < 8*numBoxes; si += 1){
                    guideDotIndicies[si] = si;
                }
                _guideGridBuffers[i].Indexbuffer.SetData(guideDotIndicies);

                #endregion

                #region verticies

                var verts = new VertexPositionColor[_deckFloorBoundingboxes[i].Count()*8];

                int vertIndex = 0;

                foreach (var boundingBox in _deckFloorBoundingboxes[i]){
                    Vector3 v1, v2, v3, v4;
                    //v4  v3
                    //
                    //v1  v2
                    v1 = boundingBox.Min;
                    v2 = new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z);
                    v3 = boundingBox.Max;
                    v4 = new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z);

                    v1.Y += 0.03f;
                    v2.Y += 0.03f;
                    v3.Y += 0.03f;
                    v4.Y += 0.03f;


                    verts[vertIndex] = new VertexPositionColor(v1, Color.Gray);
                    verts[vertIndex + 1] = new VertexPositionColor(v2, Color.Gray);
                    verts[vertIndex + 2] = new VertexPositionColor(v2, Color.Gray);
                    verts[vertIndex + 3] = new VertexPositionColor(v3, Color.Gray);

                    verts[vertIndex + 4] = new VertexPositionColor(v3, Color.Gray);
                    verts[vertIndex + 5] = new VertexPositionColor(v4, Color.Gray);
                    verts[vertIndex + 6] = new VertexPositionColor(v4, Color.Gray);
                    verts[vertIndex + 7] = new VertexPositionColor(v1, Color.Gray);

                    vertIndex += 8;
                }
                _guideGridBuffers[i].Vertexbuffer.SetData(verts);

                #endregion

                _guideGridBuffers[i].IsEnabled = false;
            }
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
            var prevCursorPosition = _cursorPosition;
            #region update cursor

            var nearMouse = new Vector3(state.MousePos.X, state.MousePos.Y, 0);
            var farMouse = new Vector3(state.MousePos.X, state.MousePos.Y, 1);

            //transform the mouse into world space
            var nearPoint = Singleton.Device.Viewport.Unproject(
                nearMouse,
                Singleton.ProjectionMatrix,
                state.ViewMatrix,
                Matrix.Identity
                );

            var farPoint = Singleton.Device.Viewport.Unproject(
                farMouse,
                Singleton.ProjectionMatrix,
                state.ViewMatrix,
                Matrix.Identity
                );

            var direction = farPoint - nearPoint;
            direction.Normalize();
            var ray = new Ray(nearPoint, direction);

            
            float? ndist;
            bool intersectionFound = false;
            for (int i = 0; i < _deckFloorBoundingboxes[_curDeck.Value].Length; i++){
                if ((ndist = ray.Intersects(_deckFloorBoundingboxes[_curDeck.Value][i])) != null){
                    _cursorActive = true;
                    var rayTermination = ray.Position + ray.Direction*(float) ndist;

                    var distList = new List<float>();

                    for (int point = 0; point < _deckFloorVertexes[_curDeck.Value].Count(); point++){
                        distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[_curDeck.Value][point]));
                    }
                    float f = distList.Min();

                    int ptIdx = distList.IndexOf(f);
                    _cursorPosition = _deckFloorVertexes[_curDeck.Value][ptIdx];
                    var verts = new VertexPositionColor[2];
                    verts[0] = new VertexPositionColor(
                        new Vector3(
                            _cursorPosition.X,
                            _cursorPosition.Y + 0.03f,
                            _cursorPosition.Z
                            ),
                        Color.White
                        );
                    verts[1] = new VertexPositionColor(
                        new Vector3(
                            _cursorPosition.X,
                            _cursorPosition.Y + 10f,
                            _cursorPosition.Z
                            ),
                        Color.White
                        );
                    _selectionBuff.Vertexbuffer.SetData(verts);
                    _selectionBuff.IsEnabled = true;
                    intersectionFound = true;
                    break;
                }
            }
            if (!intersectionFound){
                _selectionBuff.IsEnabled = false;
                _cursorActive = false;
            }

            #endregion

            if (
                state.LeftButtonState != state.PrevState.LeftButtonState &&
                state.LeftButtonState == ButtonState.Pressed
                && _cursorActive
                ){
                    _strokeOrigin = _cursorPosition;
                    _isDrawing = true;
            }

            if (prevCursorPosition != _cursorPosition && _cursorActive && _isDrawing) {
                
                
                
                
                
                
                VertexPositionNormalTexture[] v;
                int[] p;
                MeshHelper.GenerateCube(out v, out p, _cursorPosition, 5, 5, 5);
                _wallBuffers[_curDeck.Value].AddObject(_cursorPosition, p, v);
            }

            if (_cursorActive && _isDrawing && state.LeftButtonState == ButtonState.Released){
                _isDrawing = false;
            }

        }

        public void UpdateLogic(double timeDelta){}

        public void Enable(){
            _isEnabled = true;
            _selectionBuff.IsEnabled = true;
            _guideGridBuffers[_curDeck.Value].IsEnabled = true;
        }

        public void Disable(){
            foreach (var buffer in _guideGridBuffers){
                buffer.IsEnabled = false;
            }
            _isEnabled = false;
            _selectionBuff.IsEnabled = false;
        }

        #endregion

        void VisibleDeckChange(IntRef caller, int oldVal, int newVal){
            if (_isEnabled){
                foreach (var buffer in _guideGridBuffers){
                    buffer.IsEnabled = false;
                }

                _guideGridBuffers[_curDeck.Value].IsEnabled = true;
            }
        }

        void GenerateWallsFromStroke(){

        }

    }
    struct WallIdentifier {
        public Vector3 StartPoint;
        public Vector3 EndPoint;

        #region equality operators
        public static bool operator ==(WallIdentifier wallid1, WallIdentifier wallid2) {
            if (wallid1.StartPoint == wallid2.StartPoint && wallid1.EndPoint == wallid2.EndPoint)
                return true;
            if (wallid1.StartPoint == wallid2.EndPoint && wallid2.EndPoint == wallid1.StartPoint)
                return true;
            return false;
        }

        public static bool operator !=(WallIdentifier wallid1, WallIdentifier wallid2) {
            if (wallid1.StartPoint == wallid2.StartPoint && wallid1.EndPoint == wallid2.EndPoint)
                return false;
            if (wallid1.StartPoint == wallid2.EndPoint && wallid2.EndPoint == wallid1.StartPoint)
                return false;
            return true;
        }

        public bool Equals(WallIdentifier other){
            if (this.StartPoint == other.StartPoint && this.EndPoint == other.EndPoint)
                return true;
            if (this.StartPoint == other.EndPoint && other.EndPoint == this.StartPoint)
                return true;
            return false;
        }

        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (WallIdentifier)) return false;
            return Equals((WallIdentifier) obj);
        }
        #endregion

        public override int GetHashCode() {
            unchecked {
// ReSharper disable NonReadonlyFieldInGetHashCode
                return (StartPoint.GetHashCode() * 397) ^ EndPoint.GetHashCode();
// ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }
    }
}