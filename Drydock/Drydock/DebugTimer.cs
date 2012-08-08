#region

using System;
using System.Diagnostics;

#endregion

namespace Project_Forge.utilities{
    internal static class DebugTimer{
        private static readonly Stopwatch _timer = new Stopwatch();

        public static void Start(){
            _timer.Start();
        }

        public static void Stop(){
            _timer.Stop();
        }

        public static void Report(string str){
            Console.WriteLine(str + _timer.ElapsedMilliseconds);
            _timer.Reset();
        }
    }
}