using System;
using Assets.Scripts.DungeonMaster;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace BattleTests
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void RookMoves()
        {
            Assert.IsTrue(Map.IsRookMove(new Vector3Int(0, 0, 0), new Vector3Int(0, 10, 0)));
            Assert.IsTrue(Map.IsRookMove(new Vector3Int(2, 2, 2), new Vector3Int(2, -10, 2)));
            Assert.IsFalse(Map.IsRookMove(new Vector3Int(0, 5, -6), new Vector3Int(0, 10, 0)));
        }
    }
}
