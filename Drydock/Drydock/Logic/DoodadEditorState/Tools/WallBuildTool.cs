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
        readonly int _numDecks;
        readonly WireframeBuffer _selectionBuff;
        readonly IntRef _visibleDecks;

        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecks){
            _visibleDecks = visibleDecks;
            _numDecks = hullInfo.NumDecks;
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.DeckFloorVertexes;
            _selectionBuff = new WireframeBuffer(2, 2, 1);

            var indicies = new[]{0, 1};
            _selectionBuff.Indexbuffer.SetData(indicies);
            _selectionBuff.IsEnabled = false;
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
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


            int deckIndex = _numDecks - _visibleDecks.Value;
            float? ndist;
            for (int i = 0; i < _deckFloorBoundingboxes[deckIndex].Length; i++){
                if ((ndist = ray.Intersects(_deckFloorBoundingboxes[deckIndex][i])) != null){
                    var rayTermination = ray.Position + ray.Direction*(float) ndist;

                    var distList = new List<float>();

                    for (int point = 0; point < _deckFloorVertexes[deckIndex].Count(); point++){
                        distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[deckIndex][point]));
                    }
                    float f = distList.Min();
                    int ptIdx = distList.IndexOf(f);

                    var verts = new VertexPositionColor[2];
                    verts[0] = new VertexPositionColor(_deckFloorVertexes[deckIndex][ptIdx], Color.White);
                    verts[1] = new VertexPositionColor(
                        new Vector3(
                            _deckFloorVertexes[deckIndex][ptIdx].X,
                            _deckFloorVertexes[deckIndex][ptIdx].Y + 10,
                            _deckFloorVertexes[deckIndex][ptIdx].Z
                            ),
                        Color.White
                        );
                    int h = 5;
                    _selectionBuff.Vertexbuffer.SetData(verts);
                    _selectionBuff.IsEnabled = true;
                }
            }
        }

        public void UpdateLogic(double timeDelta){
        }

        public void Enable(){
        }

        public void Disable(){
            _selectionBuff.IsEnabled = false;
        }

        #endregion
    }
}