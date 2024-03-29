﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Scripts.DungeonMaster;
using System.Linq;
using UnityEngine;
using Assets.Scripts.DungeonMaster.Buffs;

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
        public void ZachFallsIntoLavaAndDies()
        {
            Battle b = DebugData.GetFunDebugBattle();
            Unit zach = b.units.First(u => u.Name == "Zach");
            b.currentSide = b.sides.First(s => s.ID == zach.SideID);
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

        [TestMethod]
        public void TestStringOutput()
        {
            Battle b = DebugData.GetFunDebugBattle();
            string output = b.ShowBattle();
            Assert.AreEqual(output[0], '2');
            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"battletest.txt"))
            //{
            //    file.Write(output);
            //}
        }

        [TestMethod]
        public void StrikeFromShadowsDissipates()
        {
            Battle b = Battle.GetDebugBattle();
            var robby = b.units.First(u => u.Name == "Robby");
            robby.GainBuff(new StrikeFromShadows());
            Assert.IsTrue(robby.Buffs.Count == 1);
            b.MakeMove(robby.ID, Map.Direction.North);
            Assert.IsTrue(robby.Buffs.Count == 0);
        }



    }
}
