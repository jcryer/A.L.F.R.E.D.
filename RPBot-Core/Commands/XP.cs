using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("xp", CanInvokeWithoutSubcommand = true), Description("Admin command to give XP"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
    class XPClass : BaseCommandModule
    {
        public async Task ExecuteGroupAsync(CommandContext e, [Description("User to change stats of")] DiscordMember user, [Description("How much you wish to change it by")] int xpNum)
        {
            if (xpNum != 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.Xp += xpNum;
                if (userData.UserData.Xp < 0) userData.UserData.Xp = 0;

                RPClass.SaveData(1);
                UserObject.RootObject newUserData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);

                await UpdateStats();
                await UpdatePlayerRanking(e.Guild);
                await XPClass.UpdateGuildRanking(e.Guild);

                await e.RespondAsync("Stat changed.");
            }
        }
        [Command("update"), Description("Updates saved data")]
        public async Task Update(CommandContext e)
        {
            await UpdateStats();
            await UpdatePlayerRanking(e.Guild);
            await XPClass.UpdateGuildRanking(e.Guild);
            await RPBot.UpdateUserList(e.Guild);
            await e.RespondAsync("Done!");
        }

        [Command("bulk"), Description("Staff command to give multiple people XP (Better for bot).")]
        public async Task Bulk(CommandContext e)
        {

            await e.RespondAsync("Change stats by typing `<mention> <xp amount>.\nTo end this process and save, type `stop`.");
            var interactivity = e.Client.GetInteractivity();

            AnotherMessage:

            var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
            if (msg != null)
            {
                if (msg.Message.Content == "stop")
                {
                    await UpdateStats();
                    RPClass.SaveData(1);
                    await UpdatePlayerRanking(e.Guild);

                    await e.RespondAsync("Stats updated.");
                }
                else
                {
                    try
                    {
                        string[] args = msg.Message.Content.Split(" ");
                        DiscordMember member = await e.CommandsNext.ConvertArgument(args[0], e, typeof(DiscordMember)) as DiscordMember;
                        UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == member.Id);
                        userData.UserData.Xp += int.Parse(args[1]);
                        if (userData.UserData.Xp < 0) userData.UserData.Xp = 0;
                        await e.RespondAsync("Stat changed. \nSend another, by typing `<mention> <xp amount>`.\nTo end this process, type `stop`.");
                    }
                    catch
                    {
                        await e.RespondAsync("No user found, or xp was in invalid format.");
                    }
                    goto AnotherMessage;
                }
            }
            else
            {
                await UpdateStats();
                await XPClass.UpdateGuildRanking(e.Guild);
                await UpdatePlayerRanking(e.Guild);

                RPClass.SaveData(1);

                await e.RespondAsync("Stats updated.");
            }
        }

        // Non-Command Methods
        public static async Task UpdateStats()
        {
            var ocs = RPClass.Users.Where(x => x.UserData.Xp > 0);
            if (!ocs.Any()) return;
            DiscordChannel c = RPClass.XPChannel;
            var members = await c.Guild.GetAllMembersAsync();
            int longestName = 1;
            longestName = RPClass.Users.Where(x => x.UserData.Xp > 0).Max(x => members.First(y => y.Id == x.UserData.UserID).DisplayName.Length) + 1;
            int longestXP = 5;
            if (longestName < 5) longestName = 5;

            string Name = "Name".PadRight(longestName) + "| ";
            string XP = "XP ";

            string value = "```" + Name + XP + "\n---------" + new string('-', longestName) + "\n";

            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

            SortedUsers = RPClass.Users.OrderByDescending(x => x.UserData.Xp).ToList();
            try
            {
                List<DiscordMessage> msgs = new List<DiscordMessage>(await c.GetMessagesAsync(100));
                foreach (DiscordMessage msg in msgs)
                {
                    await msg.DeleteAsync();
                    await Task.Delay(500);
                }
            }
            catch { }
            foreach (UserObject.RootObject user in SortedUsers)
            {
                if (user.UserData.Xp > 0)
                {
                    if (value.Length > 1500)
                    {
                        await c.SendMessageAsync(value + "```");
                        value = "```";
                    }

                    value += members.First(y => y.Id == user.UserData.UserID).DisplayName.PadRight(longestName) + "| " + user.UserData.Xp.ToString().PadRight(longestXP) + "\n";
                }
            }
            await c.SendMessageAsync(value + "```");
        }

        public static async Task UpdatePlayerRanking(DiscordGuild e)
        {
            var ocs = RPClass.Users.Where(x => x.UserData.Xp > 0);
            if (!ocs.Any()) return;
            var members = await e.GetAllMembersAsync();

            DiscordChannel RankingChannel = RPClass.StatsChannel;

            int longestName = ocs.Max(x => members.First(y => y.Id == x.UserData.UserID).DisplayName.Length) + 1;
            if (longestName < 5) longestName = 5;
            int longestCount = 4;
            int longestGuild = RPClass.Guilds.Select(x => x.Name.Length).DefaultIfEmpty(7).Max() + 1;
            if (longestGuild < 8) longestGuild = 8;

            string Count = "Pos".PadRight(longestCount) + "| ";
            string Name = "Name".PadRight(longestName) + "| ";
            string Faction = "Faction".PadRight(longestGuild) + "| ";
            string Rank = "Rank";
            string value = "";
            value += $"```{Count}{Name}{Faction}{Rank}\n{new string('-', $"{Count}{Name}{Faction}{Rank}".Length + 4)}\n";
            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

            SortedUsers = ocs.OrderByDescending(x => (x.UserData.Xp)).ToList();
            try
            {
                await RankingChannel.DeleteMessagesAsync(await RankingChannel.GetMessagesAsync(100));
            }
            catch { }
            int countNum = 1;
            foreach (UserObject.RootObject user in SortedUsers)
            {
                string UserRank = user.UserData.GetRank();

                string UserFaction = "";
                if (user.UserData.FactionID == 0) UserFaction += "N/A";
                else UserFaction += RPClass.Guilds.First(x => x.Id == user.UserData.FactionID).Name;

                if (value.Length > 1500)
                {
                    await RankingChannel.SendMessageAsync(value + "```");
                    value = "```";
                }
                value += (countNum.ToString().PadRight(longestCount) + "| " + members.First(y => y.Id == user.UserData.UserID).DisplayName.PadRight(longestName) + "| " + UserFaction.PadRight(longestGuild) + "| " + UserRank + "\n");
                countNum += 1;
            }
            await RankingChannel.SendMessageAsync(value + "```");
        }

        public static async Task UpdateGuildRanking(DiscordGuild e)
        {
            if (!RPClass.Guilds.Any()) return;
            DiscordChannel RankingChannel = RPClass.GuildChannel;
            int longestCount = 5;
            int longestName = 10;
            longestName = RPClass.Guilds.Max(x => x.Name.Length) + 1;

            string Count = "Pos".PadRight(longestCount) + "| ";
            string Name = "Name".PadRight(longestName) + "| ";
            string Rank = "Rank";
            string value = $"```{Count}{Name}{Rank}\n{new string('-', $"{Count}{Name}{Rank}".Length + 4)}\n";

            List<GuildObject.StatSheetObject> GuildsNew = new List<GuildObject.StatSheetObject>();

            foreach (var guild in RPClass.Guilds)
            {
                int xp = 0;
                foreach (var guildMember in RPClass.Users.Where(x => x.UserData.FactionID == guild.Id))
                {
                    if (guildMember != null)
                    {
                        xp += guildMember.UserData.Xp;
                    }
                }
                xp = (xp / RPClass.Users.Where(x => x.UserData.FactionID == guild.Id).Count());
                GuildsNew.Add(new GuildObject.StatSheetObject(guild.Name, xp));
            }

            List<GuildObject.StatSheetObject> SortedGuilds = GuildsNew.OrderByDescending(x => x.Xp).ToList();
            try
            {
                await RankingChannel.DeleteMessagesAsync(await RankingChannel.GetMessagesAsync(100));
            }
            catch { }
            int countNum = 1;
            foreach (GuildObject.StatSheetObject guild in SortedGuilds)
            {
                int rank = guild.Xp;
                string GuildRank = "Tier 10";
                if (rank < 20000) GuildRank = "Tier 9";
                if (rank < 17000) GuildRank = "Tier 8";
                if (rank < 14000) GuildRank = "Tier 7";
                if (rank < 11000) GuildRank = "Tier 6";
                if (rank < 8000) GuildRank = "Tier 5";
                if (rank < 6000) GuildRank = "Tier 4";
                if (rank < 4000) GuildRank = "Tier 3";
                if (rank < 2000) GuildRank = "Tier 2";
                if (rank < 1000) GuildRank = "Tier 1";

                if (value.Length > 1500)
                {
                    await RankingChannel.SendMessageAsync(value + "```");
                    value = "```";
                }

                value += (countNum.ToString().PadRight(longestCount) + "| " + guild.Name.PadRight(longestName) + "| " + GuildRank + "\n");
                countNum += 1;

            }
            await RankingChannel.SendMessageAsync(value + "```");
        }
    }
}