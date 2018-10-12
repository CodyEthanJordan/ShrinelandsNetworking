using System;
using System.Linq;
using Assets.Scripts.DungeonMaster;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleTests
{
    [TestClass]
    public class EncounterTests
    {
        [TestMethod]
        public void BasicAttack()
        {
            Battle b = Battle.GetDebugBattle();
            Unit charlie = b.units.First(u => u.Name == "Charlie");
            Unit jj = b.units.First(u => u.Name == "JJ");
            Assert.IsTrue(jj.HP.Current == jj.HP.Max);
            b.MakeMove(charlie.ID, Map.Direction.North);
            b.UseAbility(charlie.Name, "attack", "n");
            Assert.IsTrue(jj.HP.Current < jj.HP.Max);
        }
    }
}
