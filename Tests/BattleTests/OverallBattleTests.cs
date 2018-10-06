using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Scripts.DungeonMaster;
using System.Linq;

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
            b.MakeMove(charlie.ID, Map.Direction.North);
            b.BasicAttack(charlie.ID, Map.Direction.North, 0); //guaranteed hit
            Assert.IsTrue(jj.HP.Current < jj.HP.Max);
        }





    }
}
