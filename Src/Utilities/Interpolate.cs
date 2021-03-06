﻿#region

using System;

#endregion

namespace Drydock.Utilities{
    internal class Interpolate{
        /// <summary>
        ///   the distance between the two points being interpolated. Only matters if value is going to be queried for distances outside the range of 0-1.
        /// </summary>
        float _dist;

        float _value1;
        float _value2;

        #region constructors and setters

        public Interpolate(){
            _value1 = 0;
            _value2 = 0;
            _dist = 1;
        }

        public Interpolate(float value1, float value2, float dist){
            _value1 = value1;
            _value2 = value2;
            _dist = dist;
        }

        public void SetBounds(float value1, float value2){
            _value1 = value1;
            _value2 = value2;
        }

        public void SetDistBetweenBounds(float dist){
            _dist = dist;
        }

        #endregion

        #region normal interpol functions

        /// <summary>
        ///   Linear interpol function. Nothing fancy.
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public float GetLinearValue(float t){
            float d = t/_dist;
            return _value1 + d*(_value2 - _value1);
        }

        /// <summary>
        ///   Interpol function that looks just like cosine but uses a fraction of the processing power(?)
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public float GetSmoothValue(float t){
            float d = t/_dist;
            return _value1 + ((float) Math.Pow(d, 2)*(3 - 2*d))*(_value2 - _value1);
        }

        /// <summary>
        ///   Exponential interpol function, starts slow as t=0 and dt increases as you approach t=_dist
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public float GetAccelerationValue(float t){
            float d = t/_dist;
            return _value1 + ((float) Math.Pow(d, 2))*(_value2 - _value1);
        }

        /// <summary>
        ///   Exponential interpol function, starts fast at t=0 and dt decreases as you approach t=_dist
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public float GetDecelerationValue(float t){
            float d = t/_dist;
            return _value1 + (1f - (float) Math.Pow(1f - d, 2))*(_value2 - _value1);
        }

        #endregion

        #region reverse interpol functions

        public float GetReverseLinearValue(float t){
            t = _dist - t;
            float d = t/_dist;
            return _value1 + d*(_value2 - _value1);
        }

        public float GetReverseSmoothValue(float t){
            t = _dist - t;
            float d = t/_dist;
            return _value1 + ((float) Math.Pow(d, 2)*(3 - 2*d))*(_value2 - _value1);
        }

        public float GetReverseAccelerationValue(float t){
            t = _dist - t;
            float d = t/_dist;
            return _value1 + ((float) Math.Pow(d, 2))*(_value2 - _value1);
        }

        public float GetReverseDecelerationValue(float t){
            t = _dist - t;
            float d = t/_dist;
            return _value1 + (1f - (float) Math.Pow(1f - d, 2))*(_value2 - _value1);
        }

        #endregion

        //public float GetStepValue(float t); //sense of humor?
    }
}