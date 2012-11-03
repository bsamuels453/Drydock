using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Drydock.Utilities.ReferenceTypes;

namespace Drydock.Logic.DoodadEditorState.Tools {
    class WallMenuTool :IToolbarTool{
        readonly Toolbar _toolbar;

        public WallMenuTool(HullGeometryInfo hullInfo, IntRef visibleDecksRef, ObjectBuffer<WallIdentifier>[] wallBuffers, List<WallIdentifier>[] wallIdentifiers){

            _toolbar = new Toolbar("Templates/BuildToolbar.json");
            _toolbar.IsEnabled = false;

            _toolbar.BindButtonToTool(
                0,
                new WallBuildTool(
                    hullInfo,
                    visibleDecksRef,
                    wallBuffers,
                    wallIdentifiers
                    )
                );

            _toolbar.BindButtonToTool(
                1,
                new WallDeleteTool(
                    hullInfo,
                    visibleDecksRef,
                    wallBuffers,
                    wallIdentifiers
                    )
                );
        }

        public void UpdateInput(ref ControlState state){
            _toolbar.UpdateInput(ref state);
        }

        public void UpdateLogic(double timeDelta){
            _toolbar.UpdateLogic(timeDelta);
        }

        public void Enable(){
            _toolbar.IsEnabled = true;
        }

        public void Disable(){
            _toolbar.IsEnabled = false;
        }
    }
}
