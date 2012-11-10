using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Render;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic.DoodadEditorState.Tools {
    abstract class GuideLineConstructor {
        protected readonly WireframeBuffer[] GuideGridBuffers;
        protected readonly IntRefLambda CurDeck;

        protected GuideLineConstructor(HullGeometryInfo hullInfo, IntRef visibleDecksRef) {
            CurDeck = new IntRefLambda(visibleDecksRef, input => hullInfo.NumDecks - input);
            GuideGridBuffers = new WireframeBuffer[hullInfo.NumDecks + 1];

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
    }
}
