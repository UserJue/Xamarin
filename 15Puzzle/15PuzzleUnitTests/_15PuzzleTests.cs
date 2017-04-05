using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15PuzzleUnitTests
{
    [TestClass]
    public class _15PuzzleTests
    {
        [TestMethod]
        public void Move()
        {
            var tilesCount = 23;
            var maxX = 5;
            var maxY = 3;
            var canMove = true;
            var sut = new _15Puzzle.Models._15Puzzle();
            for (var i = 0; i < tilesCount; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    sut.Create(tilesCount, i, true);
                    canMove = (j == 0) && (sut.HoleY != 3) ||
                              (j == 1) && (sut.HoleX != 0) ||
                              (j == 2) && (sut.HoleY != 0) ||
                              (j == 3) && (sut.HoleX != 5);
                    Assert.AreEqual(canMove, sut.Move(j),$"HoleX={sut.HoleX},HoleY={sut.HoleY},Pos={i},Dir={j}");
                    for (var k = 0; k < tilesCount; k++)
                    {
                        Assert.IsTrue((sut.Tiles[k].IndexX >= 0) && (sut.Tiles[k].IndexX <= maxX),$"wrong X,Pos={i},Dir={j} X={sut.Tiles[k].IndexX} index={k}");
                        Assert.IsTrue((sut.Tiles[k].IndexY >= 0) && (sut.Tiles[k].IndexY <= maxX), $"wrong Y,Pos={i},Dir={j} Y={sut.Tiles[k].IndexY} index={k}");
                    }
                }
            }
        }
    }
}
