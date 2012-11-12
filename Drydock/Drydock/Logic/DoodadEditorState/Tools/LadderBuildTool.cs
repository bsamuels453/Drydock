#region

using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Control;
using Drydock.UI.Widgets;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class LadderBuildTool : GuideLineConstructor, IToolbarTool{
        const float _ladderWidthX = 1f;
        const float _ladderWidthZ = 1f;
        readonly float _gridWidth;

        readonly float _deckHeight;
        readonly List<Vector3>[] _floorVertexes;
        readonly int _numDecks;
        List<BoundingBox>[] _boundingBoxes;

        List<GhostedZone> _ghostedZoneData;
        bool _isEnabled;
        readonly GhostedZone _upperTempGhostedZone;
        GhostedZone _lowerTempGhostedZone;

        public LadderBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef)
            : base(hullInfo, visibleDecksRef){

            _deckHeight = hullInfo.DeckHeight;
            _numDecks = hullInfo.NumDecks;
            _gridWidth = hullInfo.WallResolution;

            Debug.Assert(_ladderWidthX % _gridWidth == 0);
            Debug.Assert(_ladderWidthZ % _gridWidth == 0);

            _floorVertexes = hullInfo.FloorVertexes;
            _boundingBoxes = hullInfo.DeckFloorBoundingBoxes;

            _ghostedZoneData = new List<GhostedZone>();

            //these are just the default values for what everything looks like when the editor starts
            _upperTempGhostedZone = new GhostedZone(_boundingBoxes[CurDeck.Value], _floorVertexes[CurDeck.Value]);
            _lowerTempGhostedZone = new GhostedZone(_boundingBoxes[CurDeck.Value+1], _floorVertexes[CurDeck.Value+1]);

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
            QuadIdentifier topQuad, botQuad;
            int topdeck, botdeck;

            GenerateLayerQuads(CursorPosition, out topdeck, out botdeck, out topQuad, out botQuad);

            _upperTempGhostedZone.VertexRef = _floorVertexes[topdeck];
            _lowerTempGhostedZone.VertexRef = _floorVertexes[botdeck];

            _upperTempGhostedZone.BoundingBoxRef = _boundingBoxes[topdeck];
            _lowerTempGhostedZone.BoundingBoxRef = _boundingBoxes[botdeck];

            //convert the quads to bounding boxes
            var topBBox = topQuad.GenerateBoundingBox();
            var botBBox = botQuad.GenerateBoundingBox();
            //min is always cursorpos

            var upperGhostedVertexes = new List<Vector3>();
            var upperGhostedBBoxes = new List<BoundingBox>();

            var lowerGhostedVertexes = new List<Vector3>();
            var lowerGhostedBBoxes = new List<BoundingBox>();

            int ghostVertWidthX = (int) (_ladderWidthX/_gridWidth);
            int ghostVertWidthZ = (int) (_ladderWidthZ/_gridWidth);

            int numGhostedVertexesX = ghostVertWidthX - 1;
            int numGhostedVertexesZ = ghostVertWidthZ - 1;

            for (int x = 1; x < numGhostedVertexesX + 1; x++){
                for (int z = 1; z < numGhostedVertexesZ + 1; z++){
                    upperGhostedVertexes.Add(
                        new Vector3(
                            topBBox.Min.X + x*_gridWidth,
                            topBBox.Min.Y,
                            topBBox.Min.Z + z*_gridWidth
                            )
                        );
                    lowerGhostedVertexes.Add(
                        new Vector3(
                            botBBox.Min.X + x*_gridWidth,
                            botBBox.Min.Y,
                            botBBox.Min.Z + z*_gridWidth
                            )
                        );
                }
            }


            //_upperTempGhostedZone.SetGhostedZone(
            _upperTempGhostedZone.DisableRelevantElements();
            _lowerTempGhostedZone.DisableRelevantElements();

        }

        protected override bool IsCursorValid(Vector3 newCursorPos, Vector3 prevCursorPosition, List<Vector3> deckFloorVertexes, float distToPt){

            int topDeck, bottomDeck;
            QuadIdentifier top, bottom;
            GenerateLayerQuads(newCursorPos, out topDeck, out bottomDeck, out top, out bottom);


            foreach (Vector3 point in top){
                if (!_floorVertexes[topDeck].Contains(point))
                    return false;
            }
            foreach (Vector3 point in bottom){
                if (!_floorVertexes[bottomDeck].Contains(point))
                    return false;
            }

            if (distToPt > _ladderWidthZ)
                return false;

            return true;
        }

        void GenerateLayerQuads(Vector3 cursorPos, out int topDeck, out int bottomDeck, out QuadIdentifier topQuad, out QuadIdentifier botQuad) {
            Vector3 topOffset, botOffset;
            //if curdeck is the bottom deck, change the direction that ladders go in
            if (CurDeck.Value == _numDecks) {
                topDeck = CurDeck.Value - 1;
                bottomDeck = CurDeck.Value;
                topOffset = new Vector3(0, _deckHeight, 0);
                botOffset = new Vector3(0, 0, 0);
            }
            else {
                topDeck = CurDeck.Value;
                bottomDeck = CurDeck.Value + 1;
                topOffset = new Vector3(0, 0, 0);
                botOffset = new Vector3(0, -_deckHeight, 0);
            }

            var quad = new QuadIdentifier(
                cursorPos,
                cursorPos + new Vector3(_ladderWidthZ, 0, 0),
                cursorPos + new Vector3(_ladderWidthZ, 0, _ladderWidthX),
                cursorPos + new Vector3(0, 0, _ladderWidthX)
                );

            topQuad = quad.CloneWithOffset(topOffset);
            botQuad = quad.CloneWithOffset(botOffset);
        }

        void VisibleDeckChange(IntRef caller, int oldVal, int newVal){
            if (_isEnabled){
                foreach (var buffer in GuideGridBuffers){
                    buffer.IsEnabled = false;
                }

                GuideGridBuffers[CurDeck.Value].IsEnabled = true;
            }
        }

        #region Nested type: GhostedZone

        class GhostedZone{

            //when the references are changed, the ghosted zone is disabled
            public List<BoundingBox> BoundingBoxRef {
                set {
                    EnableRelevantElements();
                    _boundingBoxesRef = value;
                }
            }
            public List<Vector3> VertexRef {
                set {
                    EnableRelevantElements();
                    _vertexesRef = value;
                }
            }

            bool _areElementsEnabled;

            List<BoundingBox> _boundingBoxesRef;
            List<Vector3> _vertexesRef;

            List<Vector3> _ghostedVertexes;
            List<BoundingBox> _ghostedBBoxes; 

            //unimpl
            public GhostedZone(List<BoundingBox> boundingBoxes,List<Vector3> floorVertexes){
                _areElementsEnabled = true;

                _boundingBoxesRef = boundingBoxes;
                _vertexesRef = floorVertexes;

                _ghostedBBoxes = null;
                _ghostedVertexes = null;
            }

            public void SetGhostedZone(List<Vector3> vertsToGhost, List<BoundingBox> bBoxesToGhost){
                _ghostedVertexes = vertsToGhost;
                _ghostedBBoxes = bBoxesToGhost;
            }

            public void EnableRelevantElements(){
                if (!_areElementsEnabled && _ghostedVertexes != null && _ghostedBBoxes != null) {
                    //Debug.Assert(_ghostedVertexes != null);
                    //Debug.Assert(_ghostedBBoxes != null);
                    _areElementsEnabled = true;
                    _vertexesRef.AddRange(_ghostedVertexes);
                    _boundingBoxesRef.AddRange(_ghostedBBoxes);
                }
            }

            public void DisableRelevantElements() {
                if (_areElementsEnabled && _ghostedVertexes != null && _ghostedBBoxes != null) {
                    //Debug.Assert(_ghostedVertexes != null);
                    //Debug.Assert(_ghostedBBoxes != null);
                    _areElementsEnabled = false;
                    _vertexesRef.RemoveAll(input => _ghostedVertexes.Contains(input));
                    _boundingBoxesRef.RemoveAll(input => _boundingBoxesRef.Contains(input));
                }
            }
        }

        #endregion
    }
}