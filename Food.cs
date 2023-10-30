using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snek;

namespace C__Snek
{
    public class Food
    {
        public bool Exists;
        public (int, int) Position;
        public char Character;

        public Food() {
            Exists = true;
            Position = (2, 5);
            Character = 'o';
        }

        public void GetNewFood() {
            while(true) {
                Random r = new();
                (int, int) newPosition = (r.Next(1, Program.game.ScreenDimensions.Item1), r.Next(1, Program.game.ScreenDimensions.Item2));

                if(Program.game.snek.Head.Position == newPosition) continue;
                foreach(BodyPiece _ in Program.game.snek.BodyPieces) if(_.Position == newPosition) continue;

                Position = newPosition;
                break;
            }
        }
        public bool CheckForFood((int, int) position) {
            if(position == (Position.Item1 + 1, Position.Item2 + 3)) return true;
            else return false;
        }

        public void Eat() {
            //Program.game.snek.Head.HeadCharacter = '\u2B26';
            Program.game.snek.CreateBodyPiece(Program.game.snek.BodyPieces[Program.game.snek.BodyPieces.Count - 1].PreviousPosition);
            Program.game.passDownCharacter = 'o';
            Program.game.score += 1;
            if(Program.game.score % 5 == 0) Program.game.snek.Speed += 1;
            GetNewFood();
        }
    }
}