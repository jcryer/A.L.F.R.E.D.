using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("faction"), Description("Faction commands"), IsMuted]
    class GuildClass : BaseCommandModule
    {

        [Command("create"), Description("Command for admins to create a faction."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Create(CommandContext e, [RemainingText, Description("Name of new faction")] string guildName)
        {
            RPClass.Guilds.Add(new GuildObject.RootObject(1 + RPClass.Guilds.Count, guildName));
            await XPClass.UpdateGuildRanking(e.Guild);

            RPClass.SaveData(3);
            await e.RespondAsync("Guild created.");
        }

        [Command("destroy"), Description("Command for admins to destroy a faction."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Destroy(CommandContext e, [RemainingText, Description("Name of faction to be destroyed.")] string guildName)
        {
            try
            {
                foreach (var user in RPClass.Users.Where(x => x.UserData.FactionID == RPClass.Guilds.First(y => y.Name == guildName).Id))
                {
                    user.UserData.FactionID = 0;
                }
                RPClass.Guilds.Remove(RPClass.Guilds.First(x => x.Name == guildName));
                await XPClass.UpdateGuildRanking(e.Guild);
                await XPClass.UpdatePlayerRanking(e.Guild);

                RPClass.SaveData(3);
                RPClass.SaveData(1);

                await e.RespondAsync("Faction deleted.");
            }
            catch
            {
                await e.RespondAsync("No faction found with that name. Are you sure you typed it in correctly?");
            }
        }

        [Command("addmember"), Description("Command for admins to add a member to a faction."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task AddMember(CommandContext e, [Description("Member to be added")] DiscordMember user, [RemainingText, Description("Name of faction which the user will be added to.")] string guildName)
        {
            try
            {
                var guild = RPClass.Guilds.First(x => x.Name == guildName);
                var rpUser = RPClass.Users.First(x => x.UserData.UserID == user.Id);
                if (rpUser.UserData.FactionID == guild.Id) {
                    await e.RespondAsync("User already in faction.");
                    return;
                }

                rpUser.UserData.FactionID = guild.Id;
                await XPClass.UpdateGuildRanking(e.Guild);
                await XPClass.UpdatePlayerRanking(e.Guild);

                RPClass.SaveData(3);
                RPClass.SaveData(1);

                await e.RespondAsync("User added to faction.");
            }
            catch
            {
                await e.RespondAsync("No faction found with that name. Are you sure you typed it in correctly?");
            }
        }

        [Command("removemember"), Description("Command for admins to remove a member from a faction."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task RemoveMember(CommandContext e, [Description("Member to be removed ")] DiscordMember user, [RemainingText, Description("Name of faction which the user will be removed from.")] string guildName)
        {
            try
            {

                var guild = RPClass.Guilds.First(x => x.Name == guildName);
                var rpUser = RPClass.Users.First(x => x.UserData.UserID == user.Id);
                if (rpUser.UserData.FactionID != guild.Id)
                {
                    await e.RespondAsync("User not in faction.");
                    return;
                }

                rpUser.UserData.FactionID = guild.Id;
                await XPClass.UpdatePlayerRanking(e.Guild);
                await XPClass.UpdateGuildRanking(e.Guild);

                RPClass.SaveData(3);
                RPClass.SaveData(1);
                await e.RespondAsync("User removed from faction.");
            }
            catch
            {
                await e.RespondAsync("No faction found with that name. Are you sure you typed it in correctly?");
            }
        }
    }
}
