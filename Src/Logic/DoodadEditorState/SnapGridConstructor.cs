﻿#region

using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal abstract class SnapGridConstructor{
        protected readonly WireframeBuffer[] GuideGridBuffers;
        protected readonly HullDataManager HullData;
        protected Vector3 CursorPosition;

        protected SnapGridConstructor(HullDataManager hullData){
            HullData = hullData;
            GuideGridBuffers = new WireframeBuffer[HullData.NumDecks];
            GenerateGuideGrid();
        }

        protected void GenerateGuideGrid(){
            for (int i = 0; i < HullData.NumDecks; i++){
                #region indicies

                int numBoxes = HullData.DeckFloorBoundingBoxes[i].Count();
                GuideGridBuffers[i] = new WireframeBuffer(8*numBoxes, 8*numBoxes, 4*numBoxes);
                var guideDotIndicies = new int[8*numBoxes];
                for (int si = 0; si < 8*numBoxes; si += 1){
                    guideDotIndicies[si] = si;
                }
                GuideGridBuffers[i].Indexbuffer.SetData(guideDotIndicies);

                #endregion

                #region verticies

                var verts = new VertexPositionColor[HullData.DeckFloorBoundingBoxes[i].Count()*8];

                int vertIndex = 0;

                foreach (var boundingBox in HullData.DeckFloorBoundingBoxes[i]){
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
                for (int i = 0; i < HullData.DeckFloorBoundingBoxes[HullData.CurDeck].Count; i++){
                    if ((ndist = ray.Intersects(HullData.DeckFloorBoundingBoxes[HullData.CurDeck][i])) != null){
                        EnableCursorGhost();
                        var rayTermination = ray.Position + ray.Direction*(float) ndist;

                        var distList = new List<float>();

                        for (int point = 0; point < HullData.FloorVertexes[HullData.CurDeck].Count(); point++){
                            distList.Add(Vector3.Distance(rayTermination, HullData.FloorVertexes[HullData.CurDeck][point]));
                        }
                        float f = distList.Min();

                        int ptIdx = distList.IndexOf(f);

                        if (!IsCursorValid(HullData.FloorVertexes[HullData.CurDeck][ptIdx], prevCursorPosition, HullData.FloorVertexes[HullData.CurDeck], f)){
                            DisableCursorGhost();
                            break;
                        }

                        CursorPosition = HullData.FloorVertexes[HullData.CurDeck][ptIdx];
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

        protected abstract bool IsCursorValid(Vector3 newCursorPos, Vector3 prevCursorPosition, List<Vector3> deckFloorVertexes, float distToPt);
    }
}