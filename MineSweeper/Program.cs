using System;
using System.Collections.Generic;
using System.Linq;
using MineSweeperGenerator;

namespace MineSweeper
{
    class Program
    {
        private static readonly MineSweeperField _field = MineSweeperFactory.CreateMineField(30, 30, 100); // width, height, nr of bombs

        static void Main(string[] args)
        {
            /*
             * You can play the game here: https://geekprank.com/minesweeper/
             * */
            
            try
            {
                var freefields = _field[0, 0];
                int rows = _field.Width;
                int columns = _field.Height;
                int noOfBombs = _field.NumberOfBombs;
                int noOfUnopenedCells = _field.Width * _field.Height - freefields.Count();

                //-1 denotes bomb cells
                checkpoint: int max = freefields.Where(x => x.NumberOfBombs != -1).Max(x => x.NumberOfBombs);

                for (int min = 1; min <= max; min++)
                {
                    for (int rowno = 0; rowno < rows; rowno++)
                    {
                        for (int colno = 0; colno < columns; colno++)
                        {
                            int unopenedCells = 0;
                            var currentCell = freefields.FirstOrDefault(x => x.X == rowno && x.Y == colno);
                            if (currentCell != null && currentCell.NumberOfBombs == min)
                            {
                                for (int i = rowno - 1; i <= rowno + 1; i++)
                                {
                                    for (int j = colno - 1; j <= colno + 1; j++)
                                    {
                                        if (i >= 0 && j >= 0)
                                        {
                                            if (freefields.FirstOrDefault(x => x.X == i && x.Y == j) == null)
                                            {
                                                unopenedCells++;
                                            }
                                        }
                                    }
                                }

                                if (unopenedCells > 0 && unopenedCells <= min)
                                {
                                    int totalSurroundingBombs = 0;
                                    for (int i = rowno - 1; i <= rowno + 1; i++)
                                    {
                                        for (int j = colno - 1; j <= colno + 1; j++)
                                        {
                                            if (freefields.FirstOrDefault(x => x.X == i && x.Y == j)?.NumberOfBombs == -1)
                                            {
                                                totalSurroundingBombs++;
                                            }
                                        }
                                    }

                                    for (int i = rowno - 1; i <= rowno + 1; i++)
                                    {
                                        for (int j = colno - 1; j <= colno + 1; j++)
                                        {
                                            if (i >= 0 && j >= 0 && i <= rows && j <= columns && freefields.FirstOrDefault(x => x.X == i && x.Y == j) == null
                                                && min == (totalSurroundingBombs + unopenedCells))
                                            {
                                                _field.IdentifyBomb(i, j);
                                                freefields.Add(new FieldInfo
                                                {
                                                    X = i,
                                                    Y = j,
                                                    NumberOfBombs = -1
                                                });
                                            }
                                            else if (i >= 0 && j >= 0 && i < rows && j < columns &&  min == (totalSurroundingBombs + unopenedCells)
                                                && freefields.FirstOrDefault(x => x.X == i && x.Y == j) == null)
                                            {
                                                freefields.AddRange(_field[i, j]);
                                                max = freefields.Where(x => x.NumberOfBombs != -1).Max(x => x.NumberOfBombs);
                                                goto checkpoint;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (freefields.FirstOrDefault(x => x.X == i && x.Y == j) == null)
                        {
                            if (freefields.Count() + 1 == rows * columns)
                            {
                                _field.IdentifyBomb(i, j);
                            }
                            freefields.AddRange(_field[i, j]);
                            goto checkpoint;
                        }
                    }
                }
            }
            catch (BombExplodedException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NoBombException e)
            {                
                Console.WriteLine(e.Message);
            }
            catch (MineFieldClearedException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}