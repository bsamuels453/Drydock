#region

using System;
using System.Collections.Generic;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal abstract class WallEditTool : GuideLineConstructor, IToolbarTool{
        protected readonly ObjectBuffer<ObjectIdentifier>[] WallBuffers;
        protected readonly float WallHeight;
        protected readonly List<ObjectIdentifier>[] WallIdentifiers;
        protected readonly float WallResolution;

        #region fields for maintaining the wall editor environment

        //readonly List<Vector3>[] _deckFloorVertexes;

        #endregion

        #region fields for the cursor and temp walls constructed by it

        readonly WireframeBuffer _cursorBuff;

        #endregion

        #region tool state fields

        protected Vector3 StrokeEnd;
        protected Vector3 StrokeOrigin;
        bool _cursorGhostActive;
        bool _isDrawing;
        bool _isEnabled;

        #endregion

        protected WallEditTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer<ObjectIdentifier>[] wallBuffers, List<ObjectIdentifier>[] wallIdentifiers)
            : base(hullInfo, visibleDecksRef){
            #region set fields

            _isEnabled = false;
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
            base.BaseUpdateInput(ref state);

            if (state.AllowLeftButtonInterpretation){
                if (
                    state.LeftButtonState != state.PrevState.LeftButtonState &&
                    state.LeftButtonState == ButtonState.Pressed
                    && _cursorGhostActive
                    ){
                    StrokeOrigin = CursorPosition;
                    _isDrawing = true;
                    HandleCursorBegin();
                }
            }

            if (state.AllowLeftButtonInterpretation){
                if (_isDrawing && state.LeftButtonState == ButtonState.Released){
                    _isDrawing = false;
                    StrokeOrigin = new Vector3();
                    StrokeEnd = new Vector3();
                    HandleCursorEnd();
                }
            }
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

        #region GuideLineConstructor overloads

        protected override void EnableCursorGhost(){
            _cursorBuff.IsEnabled = true;
            _cursorGhostActive = true;
        }

        protected override void DisableCursorGhost(){
            _cursorBuff.IsEnabled = false;
            _cursorGhostActive = false;
        }

        protected override void UpdateCursorGhost(){
            var verts = new VertexPositionColor[2];
            verts[0] = new VertexPositionColor(
                new Vector3(
                    CursorPosition.X,
                    CursorPosition.Y + 0.03f,
                    CursorPosition.Z
                    ),
                Color.White
                );
            verts[1] = new VertexPositionColor(
                new Vector3(
                    CursorPosition.X,
                    CursorPosition.Y + 10f,
                    CursorPosition.Z
                    ),
                Color.White
                );
            _cursorBuff.Vertexbuffer.SetData(verts);
            _cursorBuff.IsEnabled = true;
            if (_isDrawing){
                StrokeEnd = CursorPosition;
                HandleCursorChange();
            }
        }

        protected override bool IsCursorValid(Vector3 newCursorPos, Vector3 prevCursorPosition, List<Vector3> deckFloorVertexes, float distToPt){
            if (deckFloorVertexes.Contains(prevCursorPosition) && _isDrawing){
                var v1 = new Vector3(newCursorPos.X, newCursorPos.Y, StrokeOrigin.Z);
                var v2 = new Vector3(StrokeOrigin.X, newCursorPos.Y, newCursorPos.Z);

                if (!deckFloorVertexes.Contains(v1))
                    return false;
                if (!deckFloorVertexes.Contains(v2))
                    return false;
            }
            return true;
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

    internal struct ObjectIdentifier : IEquatable<ObjectIdentifier>{
        public readonly Vector3 RefPoint1;
        public readonly Vector3 RefPoint2;

        public ObjectIdentifier(Vector3 refPoint2, Vector3 refPoint1){
            RefPoint2 = refPoint2;
            RefPoint1 = refPoint1;
        }

        #region equality operators

        public bool Equals(ObjectIdentifier other){
            if (RefPoint2 == other.RefPoint2 && RefPoint1 == other.RefPoint1)
                return true;
            if (RefPoint2 == other.RefPoint1 && other.RefPoint2 == RefPoint1)
                return true;
            return false;
        }

        public static bool operator ==(ObjectIdentifier wallid1, ObjectIdentifier wallid2){
            if (wallid1.RefPoint2 == wallid2.RefPoint2 && wallid1.RefPoint1 == wallid2.RefPoint1)
                return true;
            if (wallid1.RefPoint2 == wallid2.RefPoint1 && wallid2.RefPoint1 == wallid1.RefPoint2)
                return true;
            return false;
        }

        public static bool operator !=(ObjectIdentifier wallid1, ObjectIdentifier wallid2){
            if (wallid1.RefPoint2 == wallid2.RefPoint2 && wallid1.RefPoint1 == wallid2.RefPoint1)
                return false;
            if (wallid1.RefPoint2 == wallid2.RefPoint1 && wallid2.RefPoint1 == wallid1.RefPoint2)
                return false;
            return true;
        }

        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (ObjectIdentifier)) return false;
            return Equals((ObjectIdentifier) obj);
        }

        #endregion

        public override int GetHashCode(){
            unchecked{
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return (RefPoint2.GetHashCode()*397) ^ RefPoint1.GetHashCode();
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }
    }

    #endregion
}