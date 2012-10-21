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
        readonly WireframeBuffer _selectionBuff;
        readonly IntRefLambda _curDeck;
        readonly ShipGeometryBuffer _guideDotBuffer;

        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecks){
            _deckFloorBoundingboxes = hullInfo.DeckFloorBoundingBoxes;
            _deckFloorVertexes = hullInfo.DeckFloorVertexes;
            _curDeck = new IntRefLambda(visibleDecks, input => hullInfo.NumDecks - input);

            _selectionBuff = new WireframeBuffer(2, 2, 1);
            var selectionIndicies = new[]{0, 1};
            _selectionBuff.Indexbuffer.SetData(selectionIndicies);
            _selectionBuff.IsEnabled = false;

            int numDots = _deckFloorVertexes[_curDeck.Value].Count();
            _guideDotBuffer = new ShipGeometryBuffer(4 * numDots, 4 * numDots, numDots, "blackdot");
            var guideDotIndicies = new int[4 * numDots];
            for (int i = 0; i < 4 * numDots; i++)
                guideDotIndicies[i] = i;
            _guideDotBuffer.Indexbuffer.SetData(guideDotIndicies);
            _guideDotBuffer.IsEnabled = true; 
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state) {
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
            for (int i = 0; i < _deckFloorBoundingboxes[_curDeck.Value].Length; i++) {
                if ((ndist = ray.Intersects(_deckFloorBoundingboxes[_curDeck.Value][i])) != null) {
                    var rayTermination = ray.Position + ray.Direction*(float) ndist;

                    var distList = new List<float>();

                    for (int point = 0; point < _deckFloorVertexes[_curDeck.Value].Count(); point++) {
                        distList.Add(Vector3.Distance(rayTermination, _deckFloorVertexes[_curDeck.Value][point]));
                    }
                    float f = distList.Min();
                    int ptIdx = distList.IndexOf(f);

                    var verts = new VertexPositionColor[2];
                    verts[0] = new VertexPositionColor(_deckFloorVertexes[_curDeck.Value][ptIdx], Color.White);
                    verts[1] = new VertexPositionColor(
                        new Vector3(
                            _deckFloorVertexes[_curDeck.Value][ptIdx].X,
                            _deckFloorVertexes[_curDeck.Value][ptIdx].Y + 10,
                            _deckFloorVertexes[_curDeck.Value][ptIdx].Z
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