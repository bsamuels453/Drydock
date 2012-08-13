using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Logic;

namespace Drydock.Utilities {
    /// <summary>
    /// REMEMEBER TO THROW AWAY THIS CLASS'S INSTANCES EVERY TIME THE RELEVANT CURVE CHANGES!
    /// </summary>
    class BezierIntersect {
        CurveController _originController;
        CurveController _destController;

        //7


        public BezierIntersect(CurveController origin, CurveController dest){
            _originController = origin;
            _destController = dest;

        }


    }
}
