using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snek {

    public class Head {
        public (int, int) Position;
        public (int, int) PreviousPosition;
        public char HeadCharacter;
        public Head() {
            Position = (6, 3);
        }
    }

    public class BodyPiece {
        public (int, int) Position;
        public (int, int) PreviousPosition;
        public char BodyCharacter;

        public BodyPiece((int, int) position) {
            Position = position;
        }
    }
    public class Snek {
        public List<BodyPiece> BodyPieces;
        public Head Head;
        public int Speed;

        public Snek() {
            Head = new();
            BodyPieces = new() {new((Head.Position.Item1-1, Head.Position.Item2)), new((Head.Position.Item1-2, Head.Position.Item2)), new((Head.Position.Item1-3, Head.Position.Item2)) };
            Speed = 2;

        }
    }
}