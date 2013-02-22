﻿#region

using Drydock.Control;
using Drydock.UI.Widgets;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallMenuTool : IToolbarTool{
        readonly Toolbar _toolbar;

        public WallMenuTool(HullDataManager hullData){
            _toolbar = new Toolbar("Templates/BuildToolbar.json");
            _toolbar.Enabled = false;

            _toolbar.BindButtonToTool(
                0,
                new WallBuildTool(hullData)
                );

            _toolbar.BindButtonToTool(
                1,
                new WallDeleteTool(hullData)
                );
        }

        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
            _toolbar.UpdateInput(ref state);
        }

        public void UpdateLogic(double timeDelta){
            _toolbar.UpdateLogic(timeDelta);
        }

        public bool Enabled{
            get { return _toolbar.Enabled; }
            set { _toolbar.Enabled = value; }
        }

        #endregion
    }
}