using System;
using System.Linq;
using Assets.Scripts.DungeonMaster;
using Assets.Scripts.DungeonMaster.Abilities;
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

        [TestMethod]
        public void LightningBolt()
        {
            Battle b = Battle.GetDebugBattle();
            Unit robby = b.units.Find(u => u.Name == "Robby");
            Unit zach = b.units.Find(u => u.Name == "Zach");
            zach.Abilities.Add(new Lightning());
            zach.Position = new UnityEngine.Vector3Int(3, 3, zach.Position.z);
            robby.Position = new UnityEngine.Vector3Int(3, 5, robby.Position.z);
            var results = b.UseAbility("Zach", "lightning", "Robby", 0);
            Assert.IsTrue(robby.HP.Current < robby.HP.Max);
        }

        [TestMethod]
        public void LightningBoltMiss()
        {
            Battle b = Battle.GetDebugBattle();
            Unit robby = b.units.Find(u => u.Name == "Robby");
            Unit zach = b.units.Find(u => u.Name == "Zach");
            zach.Abilities.Add(new Lightning());
            zach.Position = new UnityEngine.Vector3Int(3, 3, zach.Position.z);
            robby.Position = new UnityEngine.Vector3Int(3, 5, robby.Position.z);
            var results = b.UseAbility("Zach", "lightning", "s", 0);
            Assert.IsTrue(robby.HP.Current == robby.HP.Max);
        }

        [TestMethod]
        public void LightningBoltSlime()
        {
            Battle b = Battle.GetDebugBattle();
            Unit robby = b.units.Find(u => u.Name == "Robby");
            Unit zach = b.units.Find(u => u.Name == "Zach");
            zach.Abilities.Add(new Lightning());
            robby.Abilities.Add(new SlimeSpawn());
            zach.Position = new UnityEngine.Vector3Int(3, 3, zach.Position.z);
            robby.Position = new UnityEngine.Vector3Int(3, 5, robby.Position.z);
            b.UseAbility("Robby", "slimespawn", "none");
            var results = b.UseAbility("Zach", "lightning", "n", 0);
            Assert.IsTrue(robby.HP.Current < robby.HP.Max);
            Assert.IsTrue(b.map.BlockAt(new UnityEngine.Vector3Int(3, 4, robby.Position.z)).Name == "air");
        }
    }
}
