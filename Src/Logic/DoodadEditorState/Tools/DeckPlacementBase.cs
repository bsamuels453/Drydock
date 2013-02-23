#region

using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal abstract class DeckPlacementBase : IToolbarTool{
        protected readonly WireframeBuffer[] GuideGridBuffers;
        protected readonly HullDataManager HullData;
        protected readonly float GridResolution;

        readonly WireframeBuffer _cursorBuff;
        readonly int _selectionResolution;
        protected Vector3 CursorPosition;

        protected Vector3 StrokeEnd;
        protected Vector3 StrokeOrigin;
        bool _cursorGhostActive;
        bool _enabled;
        bool _isDrawing;

        public bool Enabled{
            get { return _enabled; }
            set{
                _enabled = value;
                _cursorBuff.Enabled = value;

                if (value){
                    OnEnable();
                    GuideGridBuffers[HullData.CurDeck].Enabled = true;
                }
                else{
                    foreach (var buffer in GuideGridBuffers){
                        buffer.Enabled = false;
                    }
                    OnDisable();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hullData"></param>
        /// <param name="gridResolution">not functioning properly</param>
        /// <param name="selectionResolution">how many grid tiles wide the selection marquee is intended to be. Set to -1 for selection type to be set to vertexes, rather than tiles.</param>
        protected DeckPlacementBase(HullDataManager hullData, float gridResolution, int selectionResolution=-1){
            HullData = hullData;
            if(selectionResolution>0)
                _selectionResolution = selectionResolution+1;
            else
                _selectionResolution = selectionResolution;
            
            
            //XXX gridResolution is not being reated properly, beware
            _enabled = false;
            GridResolution = gridResolution;

            _cursorBuff = new WireframeBuffer(2, 2, 1);
            var selectionIndicies = new[]{0, 1};
            _cursorBuff.Indexbuffer.SetData(selectionIndicies);
            _cursorBuff.Enabled = false;

            hullData.OnCurDeckChange += VisibleDeckChange;

            GuideGridBuffers = new WireframeBuffer[hullData.NumDecks];
            GenerateGuideGrid();
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
            #region intersect stuff

            if (state.AllowMouseMovementInterpretation){
                var prevCursorPosition = CursorPosition;

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

                //xx eventually might want to dissect this with comments
                bool intersectionFound = false;
                foreach (BoundingBox t in HullData.CurDeckBoundingBoxes){
                    float? ndist;
                    if ((ndist = ray.Intersects(t)) != null){
                        EnableCursorGhost();
                        _cursorGhostActive = true;
                        var rayTermination = ray.Position + ray.Direction*(float) ndist;

                        var distList = new List<float>();

                        for (int point = 0; point < HullData.CurDeckVertexes.Count(); point++){
                            distList.Add(Vector3.Distance(rayTermination, HullData.CurDeckVertexes[point]));
                        }
                        float f = distList.Min();

                        int ptIdx = distList.IndexOf(f);

                        if (!IsCursorValid(HullData.CurDeckVertexes[ptIdx], prevCursorPosition, HullData.CurDeckVertexes, f)){
                            _cursorGhostActive = false;
                            DisableCursorGhost();
                            break;
                        }

                        CursorPosition = HullData.CurDeckVertexes[ptIdx];
                        if (CursorPosition != prevCursorPosition){
                            UpdateCursorGhost();
                            HandleCursorChange(_isDrawing);
                        }

                        intersectionFound = true;
                        break;
                    }
                }
                if (!intersectionFound){
                    DisableCursorGhost();
                }
            }
            else{
                DisableCursorGhost();
            }

            #endregion

            if (state.AllowLeftButtonInterpretation){
                if (
                    state.LeftButtonState != state.PrevState.LeftButtonState &&
                    state.LeftButtonState == ButtonState.Pressed
                    && _cursorGhostActive
                    ){
                    StrokeOrigin = CursorPosition;
                    _isDrawing = true;
                    HandleCursorDown();
                    _cursorGhostActive = false;
                }
            }

            if (state.AllowLeftButtonInterpretation){
                if (_isDrawing && state.LeftButtonState == ButtonState.Released){
                    _isDrawing = false;
                    StrokeOrigin = new Vector3();
                    StrokeEnd = new Vector3();
                    HandleCursorRelease();
                }
            }
        }

        public void UpdateLogic(double timeDelta){
        }

        #endregion

        bool IsCursorValid(Vector3 newCursorPos, Vector3 prevCursorPosition, List<Vector3> deckFloorVertexes, float distToPt){
            if (_selectionResolution == -1){
                //vertex selection/wall drawing
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

            //object placement
            bool validCursor = true;
            for (int x = 0; x < _selectionResolution; x++){
                for (int z = 0; z < _selectionResolution; z++){
                    var vert = newCursorPos + new Vector3(x*GridResolution, 0, z*GridResolution);
                    if (!deckFloorVertexes.Contains(vert)){
                        validCursor = false;
                        break;
                    }
                }
            }
            return validCursor;
        }

        //todo: refactor gridResolution
        void GenerateGuideGrid(){
            for (int i = 0; i < HullData.NumDecks; i++){
                #region indicies

                int numBoxes = HullData.DeckBoundingBoxes[i].Count();
                GuideGridBuffers[i] = new WireframeBuffer(8*numBoxes, 8*numBoxes, 4*numBoxes);
                var guideDotIndicies = new int[8*numBoxes];
                for (int si = 0; si < 8*numBoxes; si += 1){
                    guideDotIndicies[si] = si;
                }
                GuideGridBuffers[i].Indexbuffer.SetData(guideDotIndicies);

                #endregion

                #region verticies

                var verts = new VertexPositionColor[HullData.DeckBoundingBoxes[i].Count()*8];

                int vertIndex = 0;

                foreach (var boundingBox in HullData.DeckBoundingBoxes[i]){
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
                GuideGridBuffers[i].Vertexbuffer.SetData(verts);

                #endregion

                GuideGridBuffers[i].Enabled = false;
            }
        }

        protected virtual void EnableCursorGhost(){
            _cursorBuff.Enabled = true;
            
        }

        protected virtual void DisableCursorGhost(){
            _cursorBuff.Enabled = false;
            
        }

        protected virtual void UpdateCursorGhost(){
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
            _cursorBuff.Enabled = true;
            if (_isDrawing){
                StrokeEnd = CursorPosition;
            }
        }

        void VisibleDeckChange(int oldVal, int newVal){
            if (_enabled){
                foreach (var buffer in GuideGridBuffers){
                    buffer.Enabled = false;
                }

                GuideGridBuffers[HullData.CurDeck].Enabled = true;
                OnCurDeckChange();
            }
        }

        /// <summary>
        ///   Called when the cursor moves between selection nodes.
        /// </summary>
        protected abstract void HandleCursorChange(bool isDrawing);

        /// <summary>
        ///   Called at the end of the "drawing" period when user releases mouse button.
        /// </summary>
        protected abstract void HandleCursorRelease();

        /// <summary>
        ///   Called when mouse cursor is clicked
        /// </summary>
        protected abstract void HandleCursorDown();

        /// <summary>
        ///   Called when the CurDeck changes.
        /// </summary>
        protected abstract void OnCurDeckChange();

        /// <summary>
        ///   Called when the child needs to be enabled.
        /// </summary>
        protected abstract void OnEnable();

        /// <summary>
        ///   Called when the child needs to be disabled.
        /// </summary>
        protected abstract void OnDisable();
    }
}