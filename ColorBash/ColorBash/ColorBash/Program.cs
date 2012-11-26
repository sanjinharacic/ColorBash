using System;

namespace ColorBash
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ColorBash game = new ColorBash())
            {
                game.Run();
            }
        }
    }
#endif
}

