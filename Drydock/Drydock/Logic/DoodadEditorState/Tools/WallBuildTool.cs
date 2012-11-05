#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Render;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallBuildTool : WallEditTool{
        readonly ObjectBuffer<WallIdentifier> _tempWallBuffer;
        readonly List<WallIdentifier> _tempWallIdentifiers;


        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer<WallIdentifier>[] wallBuffers, List<WallIdentifier>[] wallIdentifiers) :
            base(hullInfo, visibleDecksRef, wallBuffers, wallIdentifiers){
            _tempWallBuffer = new ObjectBuffer<WallIdentifier>(hullInfo.FloorVertexes[0].Count()*2, 10, 20, 30, "whiteborder"){UpdateBufferManually = true};
            _tempWallIdentifiers = new List<WallIdentifier>();
        }

        protected override void HandleCursorChange(){
            GenerateWallsFromStroke();
        }

        protected override void HandleCursorEnd(){
            WallIdentifiers[CurDeck.Value].AddRange(
                from id in _tempWallIdentifiers
                where !WallIdentifiers[CurDeck.Value].Contains(id)
                select id
                );
            _tempWallIdentifiers.Clear();
            WallBuffers[CurDeck.Value].AbsorbBuffer(_tempWallBuffer);
        }


        protected override void HandleCursorBegin(){
        }

        protected override void OnVisibleDeckChange(){
        }

        void GenerateWallsFromStroke(){
            _tempWallIdentifiers.Clear();
            int strokeW = (int) ((StrokeEnd.Z - StrokeOrigin.Z)/WallResolution);
            int strokeH = (int) ((StrokeEnd.X - StrokeOrigin.X)/WallResolution);

            _tempWallBuffer.ClearObjects();
            int wDir;
            int hDir;
            if (strokeW > 0)
                wDir = 1;
            else
                wDir = -1;
            if (strokeH > 0)
                hDir = 1;
            else
                hDir = -1;

            //generate width walls
            const float wallWidth = 0.1f;
            for (int i = 0; i < Math.Abs(strokeW); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution*i*wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, WallHeight, WallResolution*wDir);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution*wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeW); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeEnd.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution*i*wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, WallHeight, WallResolution*wDir);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution*wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            //generate height walls
            for (int i = 0; i < Math.Abs(strokeH); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution*i*hDir, StrokeOrigin.Y, StrokeOrigin.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution*hDir, WallHeight, wallWidth);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X + WallResolution*hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeH); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution*i*hDir, StrokeOrigin.Y, StrokeEnd.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution*hDir, WallHeight, wallWidth);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X + WallResolution*hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }

            _tempWallBuffer.UpdateBuffers();
        }
    }
}