using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using Assets.Scripts.DungeonMaster;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;

namespace ShrinelandsDiscordBot
{
    public class Commands
    {
        [Command("view")]
        public async Task View(CommandContext ctx, string type, string name)
        {
            if (type.ToLower() == "unit")
            {
                var unit = Program.battle.units.First(u => u.Name.ToLower() == name.ToLower());
                await ctx.RespondAsync(unit.ShowInfo());
            }
        }
    }
}
