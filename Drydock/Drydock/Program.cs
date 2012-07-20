using System;

namespace Drydock {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Drydock game = new Drydock())
            {
                game.Run();
            }
        }
    }
#endif
}

