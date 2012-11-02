#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Drydock.Utilities;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallBuildTool : WallEditTool{
        readonly ObjectBuffer _tempWallBuffer;
        readonly List<WallIdentifier> _tempWallIdentifiers;


        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer[] wallBuffers, List<WallIdentifier>[] wallIdentifiers) : 
            base(hullInfo, visibleDecksRef, wallBuffers, wallIdentifiers){

            _tempWallBuffer = new ObjectBuffer(hullInfo.FloorVertexes[0].Count() * 2, 10, 20, 30, "whiteborder") { UpdateBufferManually = true };
            _tempWallIdentifiers = new List<WallIdentifier>();
        }

        protected override void HandleCursorChange(){
            GenerateWallsFromStroke();
        }

        protected override void HandleCursorEnd(){
            WallIdentifiers[CurDeck.Value].AddRange(_tempWallIdentifiers);
            _tempWallIdentifiers.Clear();
            WallBuffers[CurDeck.Value].AbsorbBuffer(_tempWallBuffer);
        }

        protected override void HandleCursorBegin(){}

        void GenerateWallsFromStroke() {
            _tempWallIdentifiers.Clear();
            int strokeW = (int)((StrokeEnd.Z - StrokeOrigin.Z) / WallResolution);
            int strokeH = (int)((StrokeEnd.X - StrokeOrigin.X) / WallResolution);

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
            for (int i = 0; i < Math.Abs(strokeW); i++) {
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution * i * wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, WallHeight, WallResolution * wDir);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution * wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeW); i++) {
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeEnd.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution * i * wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, WallHeight, WallResolution * wDir);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution * wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            //generate height walls
            for (int i = 0; i < Math.Abs(strokeH); i++) {
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution * i * hDir, StrokeOrigin.Y, StrokeOrigin.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution * hDir, WallHeight, wallWidth);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X + WallResolution * hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeH); i++) {
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution * i * hDir, StrokeOrigin.Y, StrokeEnd.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution * hDir, WallHeight, wallWidth);
                var identifier = new WallIdentifier(origin, new Vector3(origin.X + WallResolution * hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }

            _tempWallBuffer.UpdateBuffers();
        }
    }

    #region wallidentifier

    internal struct WallIdentifier{
        public readonly Vector3 EndPoint;
        public readonly Vector3 StartPoint;

        public WallIdentifier(Vector3 startPoint, Vector3 endPoint){
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        #region equality operators

        public static bool operator ==(WallIdentifier wallid1, WallIdentifier wallid2){
            if (wallid1.StartPoint == wallid2.StartPoint && wallid1.EndPoint == wallid2.EndPoint)
                return true;
            if (wallid1.StartPoint == wallid2.EndPoint && wallid2.EndPoint == wallid1.StartPoint)
                return true;
            return false;
        }

        public static bool operator !=(WallIdentifier wallid1, WallIdentifier wallid2){
            if (wallid1.StartPoint == wallid2.StartPoint && wallid1.EndPoint == wallid2.EndPoint)
                return false;
            if (wallid1.StartPoint == wallid2.EndPoint && wallid2.EndPoint == wallid1.StartPoint)
                return false;
            return true;
        }

        public bool Equals(WallIdentifier other){
            if (StartPoint == other.StartPoint && EndPoint == other.EndPoint)
                return true;
            if (StartPoint == other.EndPoint && other.EndPoint == StartPoint)
                return true;
            return false;
        }

        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (WallIdentifier)) return false;
            return Equals((WallIdentifier) obj);
        }

        #endregion

        public override int GetHashCode(){
            unchecked{
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return (StartPoint.GetHashCode()*397) ^ EndPoint.GetHashCode();
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }
    }

    #endregion
}