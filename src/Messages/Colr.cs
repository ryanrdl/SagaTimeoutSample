using System;

namespace Messages
{


    public class Colr : IDisposable
    {
        private readonly ConsoleColor _original;

        public Colr(ConsoleColor color)
        {
            _original = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ForegroundColor = _original;
        }

        public static Colr Red()
        {
            return new Colr(ConsoleColor.Red);
        }

        public static Colr Blue()
        {
            return new Colr(ConsoleColor.Blue);
        }

        public static Colr Green()
        {
            return new Colr(ConsoleColor.Green);
        }

        public static Colr Yellow()
        {
            return new Colr(ConsoleColor.Yellow);
        }

        public static Colr White()
        {
            return new Colr(ConsoleColor.White);
        }

        public static Colr Magenta()
        {
            return new Colr(ConsoleColor.Magenta);
        }
    }
}
