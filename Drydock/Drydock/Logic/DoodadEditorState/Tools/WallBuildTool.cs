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

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallBuildTool : IToolbarTool{
        readonly BoundingBox[][] _deckFloorBoundingboxes;
        readonly Vector3[][] _deckFloorVertexes;
        readonly WireframeBuffer _selectionBuff;
        readonly IntRefLambda _curDeck;
        readonly WireframeBuffer[] _guideDotBuffer;
        bool _isEnabled;

        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecks){
            _isEnabled = false;
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.FloorVertexes;
            _curDeck = new IntRefLambda(visibleDecks, input => hullInfo.NumDecks - input);
            visibleDecks.RefModCallback += VisibleDeckChange;

            _selectionBuff = new WireframeBuffer(2, 2, 1);
            var selectionIndicies = new[]{0, 1};
            _selectionBuff.Indexbuffer.SetData(selectionIndicies);
            _selectionBuff.IsEnabled = false;

            _guideDotBuffer = new WireframeBuffer[hullInfo.NumDecks+1];

            for (int i = 0; i < hullInfo.NumDecks+1; i++) {
                #region indicies
                int numBoxes = _deckFloorBoundingboxes[i].Count();
                _guideDotBuffer[i] = new WireframeBuffer(8*numBoxes, 8*numBoxes, 4*numBoxes);
                var guideDotIndicies = new int[8*numBoxes];
                for (int si = 0; si < 8*numBoxes; si += 1){
                    guideDotIndicies[si] = si;
                }
                _guideDotBuffer[i].Indexbuffer.SetData(guideDotIndicies);
                
                #endregion

                #region verticies

                var verts = new VertexPositionColor[_deckFloorBoundingboxes[i].Count() * 8];

                int vertIndex = 0;

                foreach (var boundingBox in _deckFloorBoundingboxes[i]) {
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
                _guideDotBuffer[i].Vertexbuffer.SetData(verts);
                #endregion

                _guideDotBuffer[i].IsEnabled = false;
            }

        }

        #region IToolbarTool Members

        void VisibleDeckChange(IntRef caller, int oldVal, int newVal){
            if (_isEnabled){
                foreach (var buffer in _guideDotBuffer){
                    buffer.IsEnabled = false;
                }

                _guideDotBuffer[_curDeck.Value].IsEnabled = true;
            }
        }

        public void UpdateInput(ref ControlState state) {
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
            for (int i = 0; i < _deckFloorBoundingboxes[_curDeck.Value].Length; i++) {
                if ((ndist = ray.Intersects(_deckFloorBoundingboxes[_curDeck.Value][i])) != null) {
                    var rayTermination = ray.Position + ray.Direction*(float) ndist;

                    var distList = new List<float>();

                    for (int point = 0; point < _deckFloorVertexes[_curDeck.Value].Count(); point++) {
                        distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[_curDeck.Value][point]));
                    }
                    float f = distList.Min();

                    int ptIdx = distList.IndexOf(f);

                    var verts = new VertexPositionColor[2];
                    verts[0] = new VertexPositionColor(
                        new Vector3(
                        _deckFloorVertexes[_curDeck.Value][ptIdx].X,
                        _deckFloorVertexes[_curDeck.Value][ptIdx].Y+0.03f,
                        _deckFloorVertexes[_curDeck.Value][ptIdx].Z
                        ), 
                        Color.White
                        );
                    verts[1] = new VertexPositionColor(
                        new Vector3(
                            _deckFloorVertexes[_curDeck.Value][ptIdx].X,
                            _deckFloorVertexes[_curDeck.Value][ptIdx].Y + 10f,
                            _deckFloorVertexes[_curDeck.Value][ptIdx].Z
                            ),
                        Color.White
                        );
                    _selectionBuff.Vertexbuffer.SetData(verts);
                    _selectionBuff.IsEnabled = true;
                    intersectionFound = true;
                    break;
                }
            }
            if(!intersectionFound){
                _selectionBuff.IsEnabled = false;
            }
        }

        public void UpdateLogic(double timeDelta){
        }

        public void Enable(){
            _isEnabled = true;
            _selectionBuff.IsEnabled = true;
            _guideDotBuffer[_curDeck.Value].IsEnabled = true;
        }

        public void Disable(){
            foreach (var buffer in _guideDotBuffer){
                buffer.IsEnabled = false;
            }
            _isEnabled = false;
            _selectionBuff.IsEnabled = false;
        }

        #endregion
    }
}