using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15Puzzle.Models;
using _15Puzzle.ViewModels;

namespace _15PuzzleUnitTests
{
    [TestClass]
    public class MainViewModelTests
    {
        readonly double[][] posesX = {
                new [] { // 0
                                 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 1
                            0.0,      2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 2
                            0.0, 1.0,      3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 3
                            0.0, 1.0, 2.0,      4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 4
                            0.0, 1.0, 2.0, 3.0,      5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] {  // 5
                            0.0, 1.0, 2.0, 3.0, 4.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 6
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                                 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] {  // 7
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0,      2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 8
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0,      3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 9
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0,      4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 10
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0,      5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 11
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 12
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                                 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 13
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0,      2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 14
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0,      3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 15
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0,      4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 16
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0,      5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 17
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 18
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                                 1.0, 2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 19
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0,      2.0, 3.0, 4.0, 5.0,
                        },
                new [] { // 20
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0,      3.0, 4.0, 5.0,
                        },
                new [] { // 21
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0,      4.0, 5.0,
                        },
                new [] { // 22
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0,      5.0,
                        },
                new [] { // 23
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0, 5.0,
                            0.0, 1.0, 2.0, 3.0, 4.0,
                        },
                    };

        private readonly double[][] posesY =
        {
            new[]
            {
                     0.0, 0.0, 0.0, 0.0, 0.0, 
                1.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                2.0, 2.0, 2.0, 2.0, 2.0, 2.0,
                3.0, 3.0, 3.0, 3.0, 3.0, 3.0,
            },
            new[]
            {
                0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                     1.0, 1.0, 1.0, 1.0, 1.0,
                2.0, 2.0, 2.0, 2.0, 2.0, 2.0,
                3.0, 3.0, 3.0, 3.0, 3.0, 3.0,
            },
            new[]
            {
                0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                1.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                     2.0, 2.0, 2.0, 2.0, 2.0,
                3.0, 3.0, 3.0, 3.0, 3.0, 3.0,
            },
            new[]
            {
                0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                1.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                2.0, 2.0, 2.0, 2.0, 2.0, 2.0,
                     3.0, 3.0, 3.0, 3.0, 3.0,
            },
        };

        private _15Puzzle.Models._15Puzzle model;

        [TestMethod]
        public void OnMoveX_notMoved_Right02()
        {
            var tilesCount = 23;
            var dX = 0.2;
            var dY = 0;
            model = new _15Puzzle.Models._15Puzzle();
            var settings = new Settings();
            settings.PuzzleInfos.Add(new PuzzleInfo("dummy", "dummy", 15, false));
            settings.PuzzleInfo = settings.PuzzleInfos[0];
            for (var holeIndex = 0;holeIndex < tilesCount+1; holeIndex++)
                CheckMove(tilesCount, model, holeIndex, settings, dX, dY);
        }

        [TestMethod]
        public void OnMoveX_notMoved_Left02()
        {
            var tilesCount = 23;
            var dX = -0.2;
            var dY = 0;
            model = new _15Puzzle.Models._15Puzzle();
            var settings = new Settings();
            settings.PuzzleInfos.Add(new PuzzleInfo("dummy", "dummy", 15, false));
            settings.PuzzleInfo = settings.PuzzleInfos[0];
            for (var holeIndex = 0; holeIndex < tilesCount + 1; holeIndex++)
                CheckMove(tilesCount, model, holeIndex, settings, dX, dY);
        }

        [TestMethod]
        public void OnMoveX_notMoved_Left12()
        {
            var tilesCount = 23;
            var dX = -1.2;
            var dY = 0;
            model = new _15Puzzle.Models._15Puzzle();
            var settings = new Settings();
            settings.PuzzleInfos.Add(new PuzzleInfo("dummy", "dummy", 15, false));
            settings.PuzzleInfo = settings.PuzzleInfos[0];
            for (var holeIndex = 0; holeIndex < tilesCount + 1; holeIndex++)
                CheckMove(tilesCount, model, holeIndex, settings, dX, dY,0);
        }

        [TestMethod]
        public void OnMoveX_notMoved_Left_8_3()
        {
            var tilesCount = 23;
            var dX = -0.3;
            var dY = 0;
            model = new _15Puzzle.Models._15Puzzle();
            var settings = new Settings();
            settings.PuzzleInfos.Add(new PuzzleInfo("dummy", "dummy", 15, false));
            settings.PuzzleInfo = settings.PuzzleInfos[0];
            for (var holeIndex = 0; holeIndex < tilesCount + 1; holeIndex++)
                CheckMove(tilesCount, model, holeIndex, settings, dX, dY, 3,8);
        }

        private void CheckMove(int tilesCount, _15Puzzle.Models._15Puzzle model, int holeIndex, Settings settings, double dX, int dY,int possibleCount = 1,int count = 1)
        {
            for (var i = 0; i < tilesCount; i++)
            {
                model.Create(tilesCount, holeIndex, true);
                var sut = new MainViewModel(model, settings, false);
                for (var k = 1; k <= count; k++)
                {
                    sut.Tiles[i].OnMove(dX*k, dY*k);
                    for (var j = 0; j < tilesCount; j++)
                    {
                        var dx = 0.0;
                        var dy = 0.0;
                        if ( (dX > 0) && (j >= i) && SameRow(i, holeIndex) && SameRow(j, holeIndex) &&
                            LeftOf(j, holeIndex)) dx = dX * Math.Min(k,possibleCount);
                        if ( (dX < 0) && (j <= i) && SameRow(i, holeIndex) && SameRow(j, holeIndex) &&
                            RightOf(j, holeIndex)) dx = dX * Math.Min(k, possibleCount);
                        Assert.AreEqual(posesX[holeIndex][j] + dx, sut.Tiles[j].X,
                            string.Format("falsches X {0},{1},{2},{3}", holeIndex, i, j,k));
                        Assert.AreEqual(posesY[holeIndex/6][j] + dy, sut.Tiles[j].Y, "falsches Y");
                    }
                }
            }
        }

        private bool LeftOf(int index,int hole)
        {
            var help = index;
            if (index >= hole) help++;
            var helpX = help - ((int) (help/6))*6;
            var holeX = hole - ((int) (hole/6))*6;
            return helpX < holeX;
        }

        private bool RightOf(int index, int hole)
        {
            var help = index;
            if (index >= hole) help++;
            var helpX = help - ((int)(help / 6)) * 6;
            var holeX = hole - ((int)(hole / 6)) * 6;
            return helpX > holeX;
        }

        private bool SameRow(int index, int hole)
        {
            var help = index;
            if (index >= hole) help++;
            return (help / 6) == (hole / 6);
        }
    }
}
