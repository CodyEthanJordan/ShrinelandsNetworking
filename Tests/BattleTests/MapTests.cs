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

        [TestMethod]
        public void FromStringTest()
        {
            Map m = Map.FromString(DebugData.FunMap);

            Assert.AreEqual("bedrock", m.BlockAt(new Vector3Int(5, 5, 0)).Name);
            Assert.AreEqual("lava", m.BlockAt(new Vector3Int(13, 10, 4)).Name);
        }

        [TestMethod]
        public void TestStringOutput()
        {
            Map m = Map.FromString(DebugData.FunMap);
            string output = m.ShowMap();
            Assert.AreEqual(output[0], '2');
            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"maptest.txt"))
            //{
            //    file.Write(output);
            //}
        }
    }
}
