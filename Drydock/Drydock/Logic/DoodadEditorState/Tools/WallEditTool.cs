using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Drydock.Utilities;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic.DoodadEditorState.Tools{
    abstract class WallEditTool : IToolbarTool{
        protected readonly IntRefLambda CurDeck;
        protected readonly ObjectBuffer[] WallBuffers;
        protected readonly List<WallIdentifier>[] WallIdentifiers;
        protected readonly float WallResolution;
        protected readonly float WallHeight;

        #region fields for maintaining the wall editor environment

        readonly BoundingBox[][] _deckFloorBoundingboxes;
        readonly Vector3[][] _deckFloorVertexes;
        readonly WireframeBuffer[] _guideGridBuffers;

        #endregion

        #region fields for the cursor and temp walls constructed by it

        readonly WireframeBuffer _cursorBuff;

        #endregion

        #region tool state fields

        bool _cursorActive;
        Vector3 _cursorPosition;
        bool _isDrawing;
        bool _isEnabled;
        protected Vector3 StrokeEnd;
        protected Vector3 StrokeOrigin;

        #endregion

        protected WallEditTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer[] wallBuffers, List<WallIdentifier>[] wallIdentifiers){
            #region set fields

            _isEnabled = false;
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.FloorVertexes;
            WallBuffers = wallBuffers;
            WallIdentifiers = wallIdentifiers;
            WallResolution = hullInfo.WallResolution;
            WallHeight = hullInfo.DeckHeight - 0.01f;
            CurDeck = new IntRefLambda(visibleDecksRef, input => hullInfo.NumDecks - input);

            #endregion

            #region set buffer stuff

            _cursorBuff = new WireframeBuffer(2, 2, 1);
            var selectionIndicies = new[]{0, 1};
            _cursorBuff.Indexbuffer.SetData(selectionIndicies);
            _cursorBuff.IsEnabled = false;

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

            #endregion

            visibleDecksRef.RefModCallback += VisibleDeckChange;
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
            for (int i = 0; i < _deckFloorBoundingboxes[CurDeck.Value].Length; i++){
                if ((ndist = ray.Intersects(_deckFloorBoundingboxes[CurDeck.Value][i])) != null){
                    _cursorActive = true;
                    var rayTermination = ray.Position + ray.Direction*(float) ndist;

                    var distList = new List<float>();

                    for (int point = 0; point < _deckFloorVertexes[CurDeck.Value].Count(); point++){
                        distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[CurDeck.Value][point]));
                    }
                    float f = distList.Min();

                    int ptIdx = distList.IndexOf(f);
                    _cursorPosition = _deckFloorVertexes[CurDeck.Value][ptIdx];
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
                    _cursorBuff.Vertexbuffer.SetData(verts);
                    _cursorBuff.IsEnabled = true;
                    intersectionFound = true;
                    break;
                }
            }
            if (!intersectionFound){
                _cursorBuff.IsEnabled = false;
                _cursorActive = false;
            }

            #endregion

            #region handle mousedown

            if (
                state.LeftButtonState != state.PrevState.LeftButtonState &&
                state.LeftButtonState == ButtonState.Pressed
                && _cursorActive
                ){
                StrokeOrigin = _cursorPosition;
                _isDrawing = true;
                HandleCursorBegin();
            }

            #endregion

            #region handle cursor movement

            if (prevCursorPosition != _cursorPosition && _cursorActive && _isDrawing){
                StrokeEnd = _cursorPosition;
                HandleCursorChange();
            }

            #endregion

            #region handle cursor up

            if (_cursorActive && _isDrawing && state.LeftButtonState == ButtonState.Released){
                _isDrawing = false;
                StrokeOrigin = new Vector3();
                StrokeEnd = new Vector3();
                HandleCursorEnd();
            }

            #endregion
        }

        public void UpdateLogic(double timeDelta){
        }

        public void Enable(){
            _isEnabled = true;
            _cursorBuff.IsEnabled = true;
            _guideGridBuffers[CurDeck.Value].IsEnabled = true;
        }

        public void Disable(){
            foreach (var buffer in _guideGridBuffers){
                buffer.IsEnabled = false;
            }
            _isEnabled = false;
            _cursorBuff.IsEnabled = false;
        }

        #endregion

        void VisibleDeckChange(IntRef caller, int oldVal, int newVal){
            if (_isEnabled){
                foreach (var buffer in _guideGridBuffers){
                    buffer.IsEnabled = false;
                }

                _guideGridBuffers[CurDeck.Value].IsEnabled = true;
            }
        }

        protected abstract void HandleCursorChange();
        protected abstract void HandleCursorEnd();
        protected abstract void HandleCursorBegin();
    }
}

