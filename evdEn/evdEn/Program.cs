using System;

namespace evdEn
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (evdEn game = new evdEn())
            {
                game.args = args;
                game.Run();
            }
        }
    }
#endif
}

