#region

using System.Collections.Generic;
using Drydock.Control;
using Drydock.UI.Widgets;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class LadderBuildTool : GuideLineConstructor, IToolbarTool{
        const float _ladderWidth = 1f;
        const float _ladderLength = 1f;

        readonly float _deckHeight;
        readonly List<Vector3>[] _floorVertexes;
        readonly int _numDecks;
        List<BoundingBox>[] _boundingBoxes;

        List<GhostedZoneData> _ghostedZoneData;
        bool _isEnabled;
        List<GhostedZoneData> _tempGhostedZoneData;

        public LadderBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef)
            : base(hullInfo, visibleDecksRef){
            _deckHeight = hullInfo.DeckHeight;
            _numDecks = hullInfo.NumDecks;

            _floorVertexes = hullInfo.FloorVertexes;
            _boundingBoxes = hullInfo.DeckFloorBoundingBoxes;

            _ghostedZoneData = new List<GhostedZoneData>();
            _tempGhostedZoneData = new List<GhostedZoneData>();
            visibleDecksRef.RefModCallback += VisibleDeckChange;
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
            base.BaseUpdateInput(ref state);
        }

        public void UpdateLogic(double timeDelta){
        }

        public void Enable(){
            _isEnabled = true;
            GuideGridBuffers[CurDeck.Value].IsEnabled = true;
        }

        public void Disable(){
            _isEnabled = false;
            foreach (var buffer in GuideGridBuffers){
                buffer.IsEnabled = false;
            }
        }

        #endregion

        protected override void EnableCursorGhost(){
            //throw new NotImplementedException();
        }

        protected override void DisableCursorGhost(){
            //throw new NotImplementedException();
        }

        protected override void UpdateCursorGhost(){
            //throw new NotImplementedException();
        }

        protected override bool IsCursorValid(Vector3 newCursorPos, Vector3 prevCursorPosition, List<Vector3> deckFloorVertexes, float distToPt){
            Vector3 topOffset, botOffset;
            int topDeck, bottomDeck;

            //if curdeck is the bottom deck, change the direction that ladders go in
            if (CurDeck.Value == _numDecks){
                topDeck = CurDeck.Value - 1;
                bottomDeck = CurDeck.Value;
                topOffset = new Vector3(0, _deckHeight, 0);
                botOffset = new Vector3(0, 0, 0);
            }
            else{
                topDeck = CurDeck.Value;
                bottomDeck = CurDeck.Value + 1;
                topOffset = new Vector3(0, 0, 0);
                botOffset = new Vector3(0, -_deckHeight, 0);
            }

            var quad = new QuadIdentifier(
                newCursorPos,
                newCursorPos + new Vector3(_ladderLength, 0, 0),
                newCursorPos + new Vector3(_ladderLength, _ladderWidth, 0),
                newCursorPos + new Vector3(0, _ladderWidth, 0
                                   ));

            var top = quad.CloneWithOffset(topOffset);
            var bottom = quad.CloneWithOffset(botOffset);

            foreach (Vector3 point in top){
                if (!_floorVertexes[topDeck].Contains(point))
                    return false;
            }
            foreach (Vector3 point in bottom){
                if (!_floorVertexes[bottomDeck].Contains(point))
                    return false;
            }

            if (distToPt > _ladderLength)
                return false;

            return true;
        }

        void VisibleDeckChange(IntRef caller, int oldVal, int newVal){
            if (_isEnabled){
                foreach (var buffer in GuideGridBuffers){
                    buffer.IsEnabled = false;
                }

                GuideGridBuffers[CurDeck.Value].IsEnabled = true;
            }
        }

        #region Nested type: GhostedZoneData

        struct GhostedZoneData{
            public readonly List<BoundingBox>[] DisabledBoundingBoxes;
            public readonly List<Vector3>[] DisabledVertexes;

            public readonly BoundingBox LowerBounding;
            public readonly BoundingBox UpperBounding;

            public GhostedZoneData(BoundingBox upperBounding, BoundingBox lowerBounding, List<Vector3>[] disabledVerts, List<BoundingBox>[] disabledBBoxes){
                UpperBounding = upperBounding;
                LowerBounding = lowerBounding;
                DisabledVertexes = disabledVerts;
                DisabledBoundingBoxes = disabledBBoxes;
            }
        }

        #endregion
    }
}