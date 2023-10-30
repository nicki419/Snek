using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using C__Snek;

namespace Snek {
    public class Game {
        public Snek snek = new();
        public (int, int) ScreenDimensions;
        public int score;
        public string BoxStyle;
        public int HighScore;
        public Food food = new();
        public char passDownCharacter;

        public struct BoxStyles {
            /// <summary> Construction:
            /// https://en.wikipedia.org/wiki/Box-drawing_character
            /// 
            /// 0 - Horizontal
            /// 1 - Vertical
            /// 2 - Top Left Corner
            /// 3 - Top Right Corner
            /// 4 - Bottom Left Corner
            /// 5 - Bottom Right Corner
            ///  
            /// </summary>
            public List<char> Light = new() {
                /* ─ */ '\u2500', 
                /* │ */ '\u2502', 
                /* ┌ */ '\u250c', 
                /* ┐ */ '\u2510',
                /* └ */ '\u2514', 
                /* ┘ */ '\u2518',
            };
            public BoxStyles() {

            }
        }
        
        public Game((int, int) screenDimensions, string boxStyle) {
            ScreenDimensions = screenDimensions;
            BoxStyle = boxStyle;
            HighScore = 0;
        }



        public void Play() {
            Console.CursorVisible = false;
            (int, int) lastPosition = (1, 3);
            (int, int) passDownPosition;
            (int, int) positionPlaceholder;
            char characterPlaceHolder;
            bool moveOverride = false;

            char direction = 'r';
            char previousDirection = 'r';
            score = 0;

            snek.Head.HeadCharacter = '<';
            foreach(BodyPiece piece in snek.BodyPieces) piece.BodyCharacter = '\u2500';

            // Task for detecting keystrokes 
            Task.Factory.StartNew(() => {
                while(true) {
                    switch(Console.ReadKey().Key) {
                        case ConsoleKey.UpArrow:
                            if(previousDirection != 'd') direction = 'u';
                            if(previousDirection == 'l' || previousDirection == 'r') {
                                if(previousDirection == 'l') passDownCharacter = '\u2570';
                                else if(previousDirection == 'r') passDownCharacter = '\u256f';
                                else passDownCharacter = '\u2502';

                                if(snek.Head.Position.Item2 == 3) snek.Head.Position = (snek.Head.Position.Item1, 3);
                                else snek.Head.Position = (snek.Head.Position.Item1, snek.Head.Position.Item2 - 1);
                                moveOverride = true;
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                                Thread.Sleep(1000/(snek.Speed + 2));
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if(previousDirection != 'r') direction = 'l';
                            if(previousDirection == 'd' || previousDirection == 'u') {
                                if(previousDirection == 'u') passDownCharacter = '\u256e';
                                else if(previousDirection == 'd') passDownCharacter = '\u256f';
                                else passDownCharacter = '\u2502';

                                if(snek.Head.Position.Item1 == 1) snek.Head.Position = (ScreenDimensions.Item1, snek.Head.Position.Item2);
                                else snek.Head.Position = (snek.Head.Position.Item1 - 1, snek.Head.Position.Item2);
                                moveOverride = true;
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                                Thread.Sleep(1000/(snek.Speed + 2));
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if(previousDirection != 'u') direction = 'd';
                            if(previousDirection == 'l' || previousDirection == 'r') {
                                if(previousDirection == 'l') passDownCharacter = '\u256d';
                                else if(previousDirection == 'r') passDownCharacter = '\u256e';
                                else passDownCharacter = '\u2502';
                                
                                if(snek.Head.Position.Item2 == ScreenDimensions.Item2 + 3) snek.Head.Position = (snek.Head.Position.Item1, 3);
                                else snek.Head.Position = (snek.Head.Position.Item1, snek.Head.Position.Item2  + 1);
                                moveOverride = true;
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                                Thread.Sleep(1000/(snek.Speed + 2));
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if(previousDirection != 'l') direction = 'r';
                            if(previousDirection == 'd' || previousDirection == 'u') {
                                if(previousDirection == 'd') passDownCharacter = '\u2570';
                                else if(previousDirection == 'u') passDownCharacter = '\u256d';
                                else passDownCharacter = '\u2500';

                                if(snek.Head.Position.Item1 == ScreenDimensions.Item1) snek.Head.Position = (1, snek.Head.Position.Item2);
                                else snek.Head.Position = (snek.Head.Position.Item1 + 1, snek.Head.Position.Item2);
                                moveOverride = true;
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                                Thread.Sleep(1000/(snek.Speed + 2));
                            }
                            break;
                    }
                }
            });

            //Console.Clear();
            DrawGame();

            // offset the position of the entire snake by 3 lines down to draw some lines on top
            snek.Head.Position = (snek.Head.Position.Item1 + 1, snek.Head.Position.Item2 + 3);
            foreach(BodyPiece piece in snek.BodyPieces) piece.Position = (piece.Position.Item1 + 1, piece.Position.Item2 + 3);

            passDownCharacter = snek.BodyPieces[0].BodyCharacter;
            passDownPosition = snek.Head.Position;

            // Main Game Loop
            while(true) {

                // Determining the next move based on the direction variable
                switch(direction) {
                    case 'r':
                        snek.Head.HeadCharacter = '<';

                        if(!moveOverride) {
                            if(previousDirection == 'd') passDownCharacter = '\u2570';
                            else if(previousDirection == 'u') passDownCharacter = '\u256d';
                            else {
                                passDownCharacter = '\u2500';
                                if(snek.Head.Position.Item1 != ScreenDimensions.Item1) snek.Head.Position = (snek.Head.Position.Item1 + 1, snek.Head.Position.Item2);
                                else snek.Head.Position = (1, snek.Head.Position.Item2);
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                            }
                        }

                        previousDirection = 'r';
                        break;
                    
                    case 'l':
                        snek.Head.HeadCharacter = '>';

                        if(!moveOverride) {
                            if(previousDirection == 'd') passDownCharacter = '\u256f';
                            else if(previousDirection == 'u') passDownCharacter = '\u256e';
                            else {
                                passDownCharacter = '\u2500';
                                if(snek.Head.Position.Item1 != 1) snek.Head.Position = (snek.Head.Position.Item1 - 1, snek.Head.Position.Item2);
                                else snek.Head.Position = (ScreenDimensions.Item1, snek.Head.Position.Item2);
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                            }
                        }

                        previousDirection = 'l';
                        break;
                    
                    case 'u':
                        snek.Head.HeadCharacter = '\u2304';

                        if(!moveOverride) {
                            if(previousDirection == 'l') passDownCharacter = '\u2570';
                            else if(previousDirection == 'r') passDownCharacter = '\u256f';
                            else {
                                passDownCharacter = '\u2502';
                                if(snek.Head.Position.Item2 != 3) snek.Head.Position = (snek.Head.Position.Item1, snek.Head.Position.Item2 - 1);
                                else snek.Head.Position = (snek.Head.Position.Item1, ScreenDimensions.Item2 + 3);
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                            }
                        }

                        previousDirection = 'u';
                        break;

                    case 'd':
                        snek.Head.HeadCharacter = '\u2303';

                        if(!moveOverride) {
                            if(previousDirection == 'l') passDownCharacter = '\u256d';
                            else if(previousDirection == 'r') passDownCharacter = '\u256e';
                            else {
                                passDownCharacter = '\u2502';
                                if(snek.Head.Position.Item2 != ScreenDimensions.Item2 + 3) snek.Head.Position = (snek.Head.Position.Item1, snek.Head.Position.Item2  + 1);
                                else snek.Head.Position = (snek.Head.Position.Item1, 3);
                                if(food.CheckForFood(snek.Head.Position)) food.Eat();
                            }
                        }

                        previousDirection = 'd';
                        break;
                }

                foreach(BodyPiece piece in snek.BodyPieces) {
                    positionPlaceholder = piece.Position;
                    piece.Position = passDownPosition;
                    passDownPosition = positionPlaceholder;

                    characterPlaceHolder = piece.BodyCharacter;
                    piece.BodyCharacter = passDownCharacter;
                    passDownCharacter = characterPlaceHolder;

                }
                
                moveOverride = false;

                Console.SetCursorPosition(7, 0);
                Console.Write(score);

                // Place Food
                Console.SetCursorPosition(food.Position.Item1 + 1, food.Position.Item2 + 3);
                Console.Write(food.Character);

                // Erase last snek body piece
                Console.SetCursorPosition(lastPosition.Item1, lastPosition.Item2);
                Console.Write(" ");

                Console.SetCursorPosition(snek.Head.Position.Item1, snek.Head.Position.Item2);
                Console.Write(snek.Head.HeadCharacter);

                foreach(BodyPiece piece in snek.BodyPieces) {
                    Console.SetCursorPosition(piece.Position.Item1, piece.Position.Item2);
                    Console.Write(piece.BodyCharacter);
                    lastPosition = piece.Position;
                }

                passDownPosition = snek.Head.Position;
                Thread.Sleep(1000/(snek.Speed + 2));
            }
        }

        public void DrawGame() {
            BoxStyles boxStyles = new();
            List<char> boxCharacters = new();

            switch(BoxStyle) {
                case "light":
                    boxCharacters = boxStyles.Light;
                    break;
            }

            Console.Clear();
            Console.WriteLine($"Score: \t\tHigh Score: {HighScore}\n");

            Console.Write(boxCharacters[2]);
            for(int i = 0; i <= ScreenDimensions.Item1; ++i) Console.Write(boxCharacters[0]);
            Console.Write(boxCharacters[3]);

            for(int i = 0; i <= ScreenDimensions.Item2; ++i){
                Console.Write('\n');
                Console.Write(boxCharacters[1]);
                Console.Write(new string(' ', ScreenDimensions.Item1 + 1));
                Console.Write(boxCharacters[1]);
            }
            Console.Write('\n');

            Console.Write(boxCharacters[4]);
            for(int i = 0; i <= ScreenDimensions.Item1; ++i) Console.Write(boxCharacters[0]);
            Console.Write(boxCharacters[5]);
        }

        public Game() {
            BoxStyle = "light";
        }
    }
}