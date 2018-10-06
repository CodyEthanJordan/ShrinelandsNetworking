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
        }
    }
}
