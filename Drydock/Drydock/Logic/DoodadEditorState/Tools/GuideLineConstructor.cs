#region

using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal abstract class GuideLineConstructor{
        protected readonly IntRefLambda CurDeck;
        protected readonly WireframeBuffer[] GuideGridBuffers;
        readonly List<BoundingBox>[] _deckFloorBoundingboxes;
        readonly List<Vector3>[] _deckFloorVertexes;
        protected Vector3 CursorPosition;

        protected GuideLineConstructor(HullGeometryInfo hullInfo, IntRef visibleDecksRef){
            CurDeck = new IntRefLambda(visibleDecksRef, input => hullInfo.NumDecks - input);
            GuideGridBuffers = new WireframeBuffer[hullInfo.NumDecks + 1];
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.FloorVertexes;

            for (int i = 0; i < hullInfo.NumDecks + 1; i++){
                #region indicies

                int numBoxes = hullInfo.DeckFloorBoundingBoxes[i].Count();
                GuideGridBuffers[i] = new WireframeBuffer(8*numBoxes, 8*numBoxes, 4*numBoxes);
                var guideDotIndicies = new int[8*numBoxes];
                for (int si = 0; si < 8*numBoxes; si += 1){
                    guideDotIndicies[si] = si;
                }
                GuideGridBuffers[i].Indexbuffer.SetData(guideDotIndicies);

                #endregion

                #region verticies

                var verts = new VertexPositionColor[hullInfo.DeckFloorBoundingBoxes[i].Count()*8];

                int vertIndex = 0;

                foreach (var boundingBox in hullInfo.DeckFloorBoundingBoxes[i]){
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

                GuideGridBuffers[i].IsEnabled = false;
            }
        }

        protected void BaseUpdateInput(ref ControlState state){
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


                float? ndist;
                bool intersectionFound = false;
                for (int i = 0; i < _deckFloorBoundingboxes[CurDeck.Value].Count; i++){
                    if ((ndist = ray.Intersects(_deckFloorBoundingboxes[CurDeck.Value][i])) != null){
                        EnableCursorGhost();
                        var rayTermination = ray.Position + ray.Direction*(float) ndist;

                        var distList = new List<float>();

                        for (int point = 0; point < _deckFloorVertexes[CurDeck.Value].Count(); point++){
                            distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[CurDeck.Value][point]));
                        }
                        float f = distList.Min();

                        int ptIdx = distList.IndexOf(f);

                        if (!IsCursorValid(_deckFloorVertexes[CurDeck.Value][ptIdx], prevCursorPosition, _deckFloorVertexes[CurDeck.Value]))
                            break;

                        CursorPosition = _deckFloorVertexes[CurDeck.Value][ptIdx];
                        if (CursorPosition != prevCursorPosition){
                            UpdateCursorGhost();
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
        }

        protected abstract void EnableCursorGhost();
        protected abstract void DisableCursorGhost();
        protected abstract void UpdateCursorGhost();

        protected abstract bool IsCursorValid(Vector3 newCursorPos, Vector3 prevCursorPosition, List<Vector3> deckFloorVertexes);
    }
}