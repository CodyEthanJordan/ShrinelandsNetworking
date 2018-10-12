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
        public async Task ShowResults(CommandContext ctx, List<Result> results)
        {
            foreach (var result in results)
            {
                if (result.Type == Result.ResultType.Deck)
                {
                    await ctx.RespondAsync(result.OutcomeDeck.ToString());
                    await ctx.RespondAsync(result.OutcomeDeck.DrawnCard.ToString());
                }
                await ctx.RespondAsync(result.Description);
            }
        }

        public async Task<bool> ValidPlayer(CommandContext ctx, string name)
        {
            if (!Program.PlayingAs.Values.Contains(name))
            {
                await ctx.RespondAsync(name + " is not currently playing, use `sides` to see options, and `playas` to choose");
                return false;
            }
            else if (Program.PlayingAs[Program.battle.currentSide.Name] != name)
            {
                string currenSide = Program.battle.currentSide.Name;
                string player = Program.PlayingAs[currenSide];
                if (player == null)
                {
                    await ctx.RespondAsync("Not your turn, current side: " + currenSide + ", played by no one, use `playas`");
                    return false;
                }
                else
                {
                    await ctx.RespondAsync("Not your turn, current side: " + currenSide + "played by: " + player);
                    return false;
                }
            }

            return true;
        }

        [Command("map")]
        public async Task ShowMap(CommandContext ctx, string level)
        {
            int levelIndex;
            if (level == "all")
            {
                int i = 0;
                foreach (var levelString in Program.battle.ShowMapByLevel())
                {
                    await ctx.RespondAsync("```z=" + i.ToString() + "\n" + levelString + "```");
                    i++;
                }
            }
            else if (int.TryParse(level, out levelIndex))
            {
                string output = Program.battle.ShowMapByLevel()[levelIndex];
                await ctx.RespondAsync("```" + output + "```");
            }
            else
            {
                await ctx.RespondAsync("Either 'map all' or 'map n' for some number n");
            }
        }

        [Command("use")]
        public async Task UseAbility(CommandContext ctx, string unitName, string abilityName, string target)
        {
            if (!await ValidPlayer(ctx, ctx.User.Username))
            {
                return;
            }

            List<Result> results = Program.battle.UseAbility(unitName, abilityName, target);

            await ShowResults(ctx, results);
        }

        [Command("view")]
        [Description("Used to get information about specific entity")]
        public async Task View(CommandContext ctx,
            [Description("type of thing to describe [unit,area]")] string type,
            string name)
        {
            if (type.ToLower() == "unit")
            {
                var unit = Program.battle.units.FirstOrDefault(u => u.Name.ToLower() == name.ToLower());
                if (unit != null)
                {
                    await ctx.RespondAsync(unit.ShowInfo(Program.battle));
                }
                else
                {
                    await ctx.RespondAsync("No unit called " + name);
                }
            }
            else if (type.ToLower() == "area")
            {
                var unit = Program.battle.units.FirstOrDefault(u => u.Name.ToLower() == name.ToLower());
                var areaAround = Program.battle.GetAreaNear(unit.ID);
            }
        }

        [Command("sides")]
        public async Task Sides(CommandContext ctx)
        {
            string message =
                Program.PlayingAs.Aggregate(
                    new StringBuilder(), (sb, x)
                    => sb.Append(x.Key + " : " + x.Value + "\n"),
                    sb => sb.ToString(0, sb.Length - 1));
            await ctx.RespondAsync(message);
        }

        [Command("playas")]
        public async Task PlayAs(CommandContext ctx, string sideName)
        {
            if (!Program.PlayingAs.Keys.Contains(sideName))
            {
                await ctx.RespondAsync("Not a valid side");
                return;
            }

            if (Program.PlayingAs[sideName] != null)
            {
                await ctx.RespondAsync("Already played by " + Program.PlayingAs[sideName] + " use force to override");
                return;
            }

            Program.PlayingAs[sideName] = ctx.User.Username;
            await ctx.RespondAsync("Now you are playing as " + sideName);
        }

        [Command("passturn")]
        public async Task PassTurn(CommandContext ctx)
        {
            if(!await ValidPlayer(ctx, ctx.User.Username))
            {
                return;
            }

            var results = Program.battle.EndTurn();
            await ShowResults(ctx, results);
        }

        [Command("move")]
        [Description("Issue a move order to a unit")]
        public async Task Move(CommandContext ctx,
            [Description("name of unit")] string unitName,
            [Description("direction to move (n,s,e,w)")] string direction)
        {
            bool valid = await ValidPlayer(ctx, ctx.User.Username);
            if (!valid)
            {
                return;
            }
            var validDirections = new List<string>() { "n", "s", "e", "w", "u", "d"};
            var unit = Program.battle.units.FirstOrDefault(u => u.Name.Equals(unitName, StringComparison.CurrentCultureIgnoreCase));
            if (unit == null)
            {
                await ctx.RespondAsync("No unit named " + unitName);
                return;
            }
            if (!validDirections.Contains(direction.ToLower()))
            {
                await ctx.RespondAsync("Not a valid direction: " + direction);
                return;
            }

            List<Result> results = new List<Result>();
            switch (direction.ToLower())
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
                case "u":
                    results = Program.battle.MakeMove(unit.ID, Map.Direction.Up);
                    break;
                case "d":
                    results = Program.battle.MakeMove(unit.ID, Map.Direction.Down);
                    break;
            }

            foreach (var result in results)
            {
                await ctx.RespondAsync(result.Description);
            }

        }
    }
}
