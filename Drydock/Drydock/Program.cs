namespace Drydock{
#if WINDOWS || XBOX
    internal static class Program{
        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        static void Main(string[] args){
            using (var game = new Drydock()){
                game.Run();
            }
        }
    }
#endif
}