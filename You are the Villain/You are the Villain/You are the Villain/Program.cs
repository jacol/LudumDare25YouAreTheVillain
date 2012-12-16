using System;

namespace You_are_the_Villain
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Walkers game = new Walkers())
            {
                game.Run();
            }
        }
    }
#endif
}

