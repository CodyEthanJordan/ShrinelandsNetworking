﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using Assets.Scripts.DungeonMaster;

namespace BattleTests
{
    [TestClass]
    public class ParseTests
    {
        [TestMethod]
        public void ParseTargets()
        {
            Vector3Int dir = (Vector3Int)Ability.ParseTarget(null, null, "n");
            Assert.AreEqual(new Vector3Int(0, 1, 0), dir);

            Vector3Int dir2 = (Vector3Int)Ability.ParseTarget(null, null, "(-1,6,0)");
            Assert.AreEqual(new Vector3Int(-1, 6, 0), dir2);
        }

        [TestMethod]
        public void TestDraws()
        {
            Deck deck = new Deck(5, 5, 0);
            for (int i = 0; i < 100; i++)
            {
                var card = deck.Draw();
                Assert.IsNotNull(card);
            }
        }
    }
}