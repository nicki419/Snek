// Made during Krzysztof's Computer Systems Class

using System;
using System.Threading;

namespace Snek {
    public class Program {
        public static Game game = new((32, 16), "light");

        public static void Main() {
            game.Play();
        }
    }
}
