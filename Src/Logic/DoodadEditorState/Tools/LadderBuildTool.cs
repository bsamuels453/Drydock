using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class LadderBuildTool : DeckPlacementBase{
        const float _ladderWidthX = 1f;
        const float _ladderWidthZ = 1f;
        const float _gridWidth=0.5f;

        readonly HullDataManager _hullData;
        readonly ObjectModelBuffer<int> _ghostedLadderModel;  

        public LadderBuildTool(HullDataManager hullData)
            : base(hullData, hullData.WallResolution, 2) {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            Debug.Assert(_ladderWidthX % _gridWidth == 0);
            Debug.Assert(_ladderWidthZ % _gridWidth == 0);
            // ReSharper restore CompareOfFloatsByEqualityOperator
            _hullData = hullData;

            _ghostedLadderModel = new ObjectModelBuffer<int>(1);
            Matrix trans = Matrix.CreateRotationX((float)-Math.PI / 2) * Matrix.CreateRotationY((float)-Math.PI / 2);
            _ghostedLadderModel.AddObject(0, Singleton.ContentManager.Load<Model>("models/ladder"), trans);
            _ghostedLadderModel.DisableObject(0);
        }

        protected override void EnableCursorGhost() {
            _ghostedLadderModel.EnableObject(0);
        }

        protected override void DisableCursorGhost() {
            _ghostedLadderModel.DisableObject(0);
        }

        protected override void UpdateCursorGhost() {
            _ghostedLadderModel.TransformAll(base.CursorPosition);
        }

        protected override void HandleCursorChange(bool isDrawing){   
        }

        protected override void HandleCursorEnd(){
        }

        protected override void HandleCursorBegin(){
        }

        protected override void OnCurDeckChange(){
        }

        protected override void OnEnable(){
        }

        protected override void OnDisable(){
            _ghostedLadderModel.DisableObject(0);
        }
    }
}
