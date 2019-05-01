using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeperGenerator;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MineSweeperGeneratorUnitTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestFactory()
        {
            var field = MineSweeperFactory.CreateMineField(16, 16, 40);

            Assert.IsNotNull(field);
            Assert.AreEqual(16, field.Width);
            Assert.AreEqual(16, field.Height);
            Assert.AreEqual(40, field.NumberOfBombs);
        }

        [TestMethod]
        public void TestBoundaries()
        {
            try
            {
                MineSweeperFactory.CreateMineField(5, 10, 20);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(TooSmallMineFieldException));
            }

            try
            {
                MineSweeperFactory.CreateMineField(1000, 10, 20);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(TooSmallMineFieldException));
            }

            try
            {
                MineSweeperFactory.CreateMineField(1001, 10, 20);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(TooBigMineFieldException));
            }

            try
            {
                MineSweeperFactory.CreateMineField(40, 40, 800);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(TooManyBombsException));
            }
        }

        [TestMethod]
        public void TestCountBombs()
        {
            var field = MineSweeperFactory.CreateMineField(10, 10 ,10);

            int numberOfBombs = 0;

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    try
                    {
                        var results = field[i, j];
                    }
                    catch (Exception)
                    {
                        numberOfBombs++;
                    }
                }
            }

            Assert.AreEqual(10, numberOfBombs);
        }

        [TestMethod]
        public void TestCountBombsLargeField()
        {
            var field = MineSweeperFactory.CreateMineField(1000, 1000, 1000);

            int numberOfBombs = 0;

            for (int i = 0; i < 1000; ++i)
            {
                for (int j = 0; j < 1000; ++j)
                {
                    try
                    {
                        var n = field[i, j];
                    }
                    catch (Exception)
                    {
                        numberOfBombs++;
                    }
                }
            }

            Assert.AreEqual(1000, numberOfBombs);
        }

        [TestMethod]
        public void TestIdentifyBomb()
        {
            var field = MineSweeperFactory.CreateMineField(40, 40, 20);

            for (int i = 0; i < 40; ++i)
            {
                for (int j = 0; j < 40; ++j)
                {
                    try
                    {
                        var n = field[i, j];

                        // since we don't get an exception no bomb is on this location
                        int bombsThusFar = field.IdentifyBomb(i, j);
                    }
                    catch (BombExplodedException)
                    {
                    }
                    catch (NoBombException ex)
                    {
                        Assert.IsTrue(ex.GetType() == typeof(NoBombException));
                    }
                }
            }
        }

        [TestMethod]
        public void TestValidateAdjacentBombs()
        {
            var field = MineSweeperFactory.CreateMineField(40, 40, 500);
            var random = new Random();

            bool emptyFieldFound = false;

            int numberOfBombs = 0;
            int numberOfBombsToBeFound = 0;
            int c = 15;

            while (!emptyFieldFound)
            {
                try
                {
                    c = random.Next(3, 18);

                    var result = field[c, c];
                    numberOfBombsToBeFound = result[0].NumberOfBombs;
                    emptyFieldFound = true;
                }
                catch (BombExplodedException)
                {
                }
            }

            numberOfBombs += CheckPosition(field, c - 1, c - 1);
            numberOfBombs += CheckPosition(field, c - 1, c);
            numberOfBombs += CheckPosition(field, c - 1, c + 1);
            numberOfBombs += CheckPosition(field, c, c - 1);
            numberOfBombs += CheckPosition(field, c, c + 1);
            numberOfBombs += CheckPosition(field, c + 1, c - 1);
            numberOfBombs += CheckPosition(field, c + 1, c);
            numberOfBombs += CheckPosition(field, c + 1, c + 1);

            Assert.AreEqual(numberOfBombsToBeFound, numberOfBombs);
        }

        [TestMethod]
        public void TestFindEmptyField()
        {
            var minesweeperfields = MineSweeperFactory.CreateMineField(40, 40, 200);

            var random = new Random();

            bool emptyFieldFound = false;

            while (!emptyFieldFound)
            {
                try
                {
                    var fields =
                        minesweeperfields[
                            random.Next(0, minesweeperfields.Width), random.Next(0, minesweeperfields.Height)];

                    if( fields[0].NumberOfBombs == 0)
                    {
                        emptyFieldFound = true;
                    }
                    else
                    {
                        continue;
                    }

                    foreach (var field in fields)
                    {
                        System.Diagnostics.Debug.WriteLine("Field in list <{0},{1},{2}>", field.X, field.Y,
                                                           field.NumberOfBombs);
                    }

                    Assert.IsTrue(true);
                }
                catch (BombExplodedException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }        

        private int CheckPosition(MineSweeperField field, int x, int y)
        {
            try
            {
                var n = field[x, y];
            }
            catch (BombExplodedException)
            {
                return 1;

            }

            return 0;
        }
    }
}
