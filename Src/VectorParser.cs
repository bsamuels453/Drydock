﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Drydock {
    /// <summary>
    /// this helper class serves as a base to parse values that json is known to fuck up deserializing
    /// </summary>
    static class VectorParser {
        static public T Parse<T>(string s) {
            Type t = typeof(T);
            if (t == typeof(Vector4)) {
                return ParseVec4<T>(s);
            }
            if (t == typeof(Vector3)) {
                return ParseVec3<T>(s);
            }
            if (t == typeof(Vector2)){
                return ParseVec2<T>(s);
            }
            throw new Exception("JsonFallbackParser recieved a type which it cannot deserialize");
        }

        static T ParseVec4<T>(string s) {
            var split = s.Split(',');
            Debug.Assert(split.Count() == 4);

            Vector4 v = new Vector4(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2]),
                float.Parse(split[3])
                );
            return (T)Convert.ChangeType(v, typeof(T));
        }

        static T ParseVec3<T>(string s) {
            var split = s.Split(',');
            Debug.Assert(split.Count() == 3);

            Vector3 v = new Vector3(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2])
                );
            return (T)Convert.ChangeType(v, typeof(T));
        }
        static T ParseVec2<T>(string s) {
            var split = s.Split(',');
            Debug.Assert(split.Count() == 3);

            Vector2 v = new Vector2(
                float.Parse(split[0]),
                float.Parse(split[1])
                );
            return (T)Convert.ChangeType(v, typeof(T));
        }
    }
}
