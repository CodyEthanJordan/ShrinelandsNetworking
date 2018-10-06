using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Scripts.DungeonMaster;
using System.Linq;
using UnityEngine;

namespace BattleTests
{
    [TestClass]
    public class OverallBattleTests
    {
        [TestMethod]
        public void BattleExists()
        {
            Battle b = Battle.GetDebugBattle();
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void BasicMove()
        {
            Battle b = Battle.GetDebugBattle();
            Unit charlie = b.units.First(u => u.Name == "Charlie");
            Unit jj = b.units.First(u => u.Name == "JJ");
            b.MakeMove(charlie.ID, Map.Direction.North);
            Assert.IsTrue(Battle.IsAdjacent(charlie, jj));
        }

        [TestMethod]
        public void BasicAttack()
        {
            Battle b = Battle.GetDebugBattle();
            Unit charlie = b.units.First(u => u.Name == "Charlie");
            Unit jj = b.units.First(u => u.Name == "JJ");
            Assert.IsTrue(jj.HP.Current == jj.HP.Max);
            b.MakeMove(charlie.ID, Map.Direction.North);
            b.BasicAttack(charlie.ID, Map.Direction.North, 0); //guaranteed hit
            Assert.IsTrue(jj.HP.Current < jj.HP.Max);
        }

        [TestMethod]
        public void ZachFallsIntoLavaAndDies()
        {
            Battle b = DebugData.GetFunDebugBattle();
            Unit zach = b.units.First(u => u.Name == "Zach");
            Assert.AreEqual("stone", b.map.StandingOn(zach).Name);
            Assert.IsTrue(zach.HP.Current == zach.HP.Max);
            b.MakeMove(zach.ID, Map.Direction.South);
            Assert.AreEqual("stone", b.map.StandingOn(zach).Name);
            Assert.IsTrue(zach.HP.Current == zach.HP.Max);
            b.MakeMove(zach.ID, Map.Direction.West);
            Assert.AreEqual("lava", b.map.BlockAt(zach.Position).Name);
            Assert.IsTrue(zach.HP.Current < zach.HP.Max);
            Assert.AreEqual(4, zach.Position.z);
        }





    }
}
