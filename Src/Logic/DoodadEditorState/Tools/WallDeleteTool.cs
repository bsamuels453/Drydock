#region

using System;
using System.Collections.Generic;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallDeleteTool : WallEditTool{
        readonly ObjectBuffer<ObjectIdentifier> _tempWallBuffer;
        List<ObjectIdentifier> _prevIdentifiers;

        public WallDeleteTool(HullDataManager hullData) :
            base(hullData){
            _tempWallBuffer = new ObjectBuffer<ObjectIdentifier>(5000, 10, 20, 30, "WallDeleteMarqueeTex"){UpdateBufferManually = true};
            _prevIdentifiers = new List<ObjectIdentifier>();
        }

        protected override void HandleCursorChange(){
            _tempWallBuffer.ClearObjects();
            int strokeW = (int) ((StrokeEnd.Z - StrokeOrigin.Z)/WallResolution);
            int strokeH = (int) ((StrokeEnd.X - StrokeOrigin.X)/WallResolution);

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

            var identifiers = new List<ObjectIdentifier>();
            //generate width walls
            const float wallWidth = 0.1f;
            const float height = 0.01f;
            for (int i = 0; i < Math.Abs(strokeW); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution*i*wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, height, WallResolution*wDir);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution*wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                identifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeW); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeEnd.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution*i*wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, height, WallResolution*wDir);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution*wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                identifiers.Add(identifier);
            }
            //generate height walls
            for (int i = 0; i < Math.Abs(strokeH); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution*i*hDir, StrokeOrigin.Y, StrokeOrigin.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution*hDir, height, wallWidth);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X + WallResolution*hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                identifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeH); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution*i*hDir, StrokeOrigin.Y, StrokeEnd.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution*hDir, height, wallWidth);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X + WallResolution*hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                identifiers.Add(identifier);
            }

            _tempWallBuffer.UpdateBuffers();

            HullData.CurWallBuffer.UpdateBufferManually = true;

            foreach (var identifier in _prevIdentifiers){
                HullData.CurWallBuffer.EnableObject(identifier);
            }
            foreach (var identifier in identifiers){
                HullData.CurWallBuffer.DisableObject(identifier);
            }

            HullData.CurWallBuffer.UpdateBuffers();
            HullData.CurWallBuffer.UpdateBufferManually = false;

            _prevIdentifiers = identifiers;
        }

        protected override void HandleCursorEnd(){
            HullData.CurWallBuffer.UpdateBufferManually = true;
            foreach (var identifier in _prevIdentifiers){
                HullData.CurWallBuffer.RemoveObject(identifier);
            }
            HullData.CurWallBuffer.UpdateBuffers();
            HullData.CurWallBuffer.UpdateBufferManually = false;

            foreach (var identifier in _prevIdentifiers){
                HullData.CurWallIdentifiers.Remove(identifier);
            }

            _tempWallBuffer.ClearObjects();
            _prevIdentifiers.Clear();
        }

        protected override void HandleCursorBegin(){
            //throw new NotImplementedException();
        }

        protected override void OnVisibleDeckChange(){
            _prevIdentifiers.Clear();
        }

        protected override void OnEnable(){
            _tempWallBuffer.Enabled = true;
        }

        protected override void OnDisable(){
            _tempWallBuffer.Enabled = false;
            _prevIdentifiers.Clear();
        }
    }
}