using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Scripts.DungeonMaster;

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


    }
}
