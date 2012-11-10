#region

using System;
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

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal abstract class WallEditTool : GuideLineConstructor, IToolbarTool{
        protected readonly ObjectBuffer<WallIdentifier>[] WallBuffers;
        protected readonly float WallHeight;
        protected readonly List<WallIdentifier>[] WallIdentifiers;
        protected readonly float WallResolution;

        #region fields for maintaining the wall editor environment

        readonly List<BoundingBox>[] _deckFloorBoundingboxes;
        readonly List<Vector3>[] _deckFloorVertexes;

        #endregion

        #region fields for the cursor and temp walls constructed by it

        readonly WireframeBuffer _cursorBuff;

        #endregion

        #region tool state fields

        protected Vector3 StrokeEnd;
        protected Vector3 StrokeOrigin;
        bool _cursorActive;
        Vector3 _cursorPosition;
        bool _isDrawing;
        bool _isEnabled;

        #endregion

        protected WallEditTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer<WallIdentifier>[] wallBuffers, List<WallIdentifier>[] wallIdentifiers) 
            : base(hullInfo, visibleDecksRef){
            #region set fields

            _isEnabled = false;
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.FloorVertexes;
            WallBuffers = wallBuffers;
            WallIdentifiers = wallIdentifiers;
            WallResolution = hullInfo.WallResolution;
            WallHeight = hullInfo.DeckHeight - 0.01f;
            

            #endregion
        
            _cursorBuff = new WireframeBuffer(2, 2, 1);
            var selectionIndicies = new[]{0, 1};
            _cursorBuff.Indexbuffer.SetData(selectionIndicies);
            _cursorBuff.IsEnabled = false;

            visibleDecksRef.RefModCallback += VisibleDeckChange;
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
            var prevCursorPosition = _cursorPosition;

            #region update cursor

            if (state.AllowMouseMovementInterpretation) {
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
                for (int i = 0; i < _deckFloorBoundingboxes[CurDeck.Value].Count; i++) {
                    if ((ndist = ray.Intersects(_deckFloorBoundingboxes[CurDeck.Value][i])) != null) {
                        _cursorActive = true;
                        var rayTermination = ray.Position + ray.Direction * (float)ndist;

                        var distList = new List<float>();

                        for (int point = 0; point < _deckFloorVertexes[CurDeck.Value].Count(); point++) {
                            distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[CurDeck.Value][point]));
                        }
                        float f = distList.Min();

                        int ptIdx = distList.IndexOf(f);

                        if (_deckFloorVertexes[CurDeck.Value].Contains(prevCursorPosition) && _isDrawing) {
                            var v1 = new Vector3(_deckFloorVertexes[CurDeck.Value][ptIdx].X, _deckFloorVertexes[CurDeck.Value][ptIdx].Y, StrokeOrigin.Z);
                            var v2 = new Vector3(StrokeOrigin.X, _deckFloorVertexes[CurDeck.Value][ptIdx].Y, _deckFloorVertexes[CurDeck.Value][ptIdx].Z);

                            if (!_deckFloorVertexes[CurDeck.Value].Contains(v1))
                                break;
                            if (!_deckFloorVertexes[CurDeck.Value].Contains(v2))
                                break;
                        }


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
                if (!intersectionFound) {
                    _cursorBuff.IsEnabled = false;
                    _cursorActive = false;
                }
            }
            else{
                _cursorBuff.IsEnabled = false;
                _cursorActive = false;
            }

            #endregion

            #region handle mousedown
            if (state.AllowLeftButtonInterpretation){
                if (
                    state.LeftButtonState != state.PrevState.LeftButtonState &&
                    state.LeftButtonState == ButtonState.Pressed
                    && _cursorActive
                    ){
                    StrokeOrigin = _cursorPosition;
                    _isDrawing = true;
                    HandleCursorBegin();
                }
            }

            #endregion

            #region handle cursor movement

            if (prevCursorPosition != _cursorPosition && _cursorActive && _isDrawing){
                StrokeEnd = _cursorPosition;
                HandleCursorChange();
            }

            #endregion

            #region handle cursor up

            if (state.AllowLeftButtonInterpretation){
                if (_isDrawing && state.LeftButtonState == ButtonState.Released){
                    _isDrawing = false;
                    StrokeOrigin = new Vector3();
                    StrokeEnd = new Vector3();
                    HandleCursorEnd();
                }
            }

            #endregion
        }

        public void UpdateLogic(double timeDelta){
        }

        public void Enable(){
            _isEnabled = true;
            _cursorBuff.IsEnabled = true;
            GuideGridBuffers[CurDeck.Value].IsEnabled = true;
            OnEnable();
        }

        public void Disable(){
            foreach (var buffer in GuideGridBuffers){
                buffer.IsEnabled = false;
            }
            _isEnabled = false;
            _cursorBuff.IsEnabled = false;
            OnDisable();
        }

        #endregion

        void VisibleDeckChange(IntRef caller, int oldVal, int newVal){
            if (_isEnabled){
                foreach (var buffer in GuideGridBuffers){
                    buffer.IsEnabled = false;
                }

                GuideGridBuffers[CurDeck.Value].IsEnabled = true;
                OnVisibleDeckChange();
            }
        }

        protected abstract void HandleCursorChange();
        protected abstract void HandleCursorEnd();
        protected abstract void HandleCursorBegin();
        protected abstract void OnVisibleDeckChange();
        protected abstract void OnEnable();
        protected abstract void OnDisable();
    }

    #region wallidentifier

    internal struct WallIdentifier : IEquatable<WallIdentifier>{
        public readonly Vector3 EndPoint;
        public readonly Vector3 StartPoint;

        public WallIdentifier(Vector3 startPoint, Vector3 endPoint){
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        #region equality operators

        public bool Equals(WallIdentifier other){
            if (StartPoint == other.StartPoint && EndPoint == other.EndPoint)
                return true;
            if (StartPoint == other.EndPoint && other.StartPoint == EndPoint)
                return true;
            return false;
        }

        public static bool operator ==(WallIdentifier wallid1, WallIdentifier wallid2){
            if (wallid1.StartPoint == wallid2.StartPoint && wallid1.EndPoint == wallid2.EndPoint)
                return true;
            if (wallid1.StartPoint == wallid2.EndPoint && wallid2.EndPoint == wallid1.StartPoint)
                return true;
            return false;
        }

        public static bool operator !=(WallIdentifier wallid1, WallIdentifier wallid2){
            if (wallid1.StartPoint == wallid2.StartPoint && wallid1.EndPoint == wallid2.EndPoint)
                return false;
            if (wallid1.StartPoint == wallid2.EndPoint && wallid2.EndPoint == wallid1.StartPoint)
                return false;
            return true;
        }

        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (WallIdentifier)) return false;
            return Equals((WallIdentifier) obj);
        }

        #endregion

        public override int GetHashCode(){
            unchecked{
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return (StartPoint.GetHashCode()*397) ^ EndPoint.GetHashCode();
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }
    }

    #endregion
}