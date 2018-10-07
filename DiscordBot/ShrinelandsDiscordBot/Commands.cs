using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using Assets.Scripts.DungeonMaster;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using Assets.Scripts.Networking;

namespace ShrinelandsDiscordBot
{
    public class Commands
    {
        [Command("view")]
        [Description("Used to get information about specific entity")]
        public async Task View(CommandContext ctx, 
            [Description("type of thing to describe [unit,block]")] string type,
            string name)
        {
            if (type.ToLower() == "unit")
            {
                var unit = Program.battle.units.FirstOrDefault(u => u.Name.ToLower() == name.ToLower());
                if(unit != null)
                {
                    await ctx.RespondAsync(unit.ShowInfo());
                }
                else
                {
                    await ctx.RespondAsync("No unit called " + name);
                }
            }
        }

        [Command("move")]
        [Description("Issue a move order to a unit")]
        public async Task Move(CommandContext ctx,
            [Description("name of unit")] string unitName,
            [Description("direction to move (n,s,e,w)")] string direction)
        {
            var validDirections = new List<string>() { "n", "s", "e", "w" };
            var unit = Program.battle.units.FirstOrDefault(u => u.Name.Equals(unitName, StringComparison.CurrentCultureIgnoreCase));
            if(unit == null)
            {
                await ctx.RespondAsync("No unit named " + unitName);
                return;
            }
            if(!validDirections.Contains(direction.ToLower()))
            {
                await ctx.RespondAsync("Not a valid direction: " + direction);
                return;
            }

            List<Result> results = new List<Result>();
            switch(direction.ToLower())
            {
                case "n":
                    results = Program.battle.MakeMove(unit.ID, Map.Direction.North);
                    break;
                case "s":
                    results = Program.battle.MakeMove(unit.ID, Map.Direction.South);
                    break;
                case "e":
                    results = Program.battle.MakeMove(unit.ID, Map.Direction.East);
                    break;
                case "w":
                    results = Program.battle.MakeMove(unit.ID, Map.Direction.West);
                    break;
            }

            foreach (var result in results)
            {
                await ctx.RespondAsync(result.Description);
            }

        }
    }
}
