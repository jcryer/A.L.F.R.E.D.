using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("money"), IsMuted]
    [Description("All Money Commands")]
    class MoneyClass : BaseCommandModule
    {
        [Command("give"), Description("Command for admins to give out currency to users."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Give(CommandContext e, [Description("Who to award the money to")] DiscordMember user, [Description("Amount of money to award")] int money = -1)
        {
            if (money > 0)
            {
                RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.Money += money;
                UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                await e.RespondAsync("User: " + user.DisplayName + " now has $" + a.UserData.Money);
                RPClass.SaveData(1);
            }
        }

        [Command("take"), Description("Command for admins to take currency from users."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Take(CommandContext e, [Description("Who to take the money from")] DiscordMember user, [Description("Amount of money to take")] int money = -1)
        {
            if (money > 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.Money -= money;
                if (userData.UserData.Money < 0) userData.UserData.Money = 0;
                await e.RespondAsync("User: " + user.DisplayName + " now has $" + userData.UserData.Money);
                RPClass.SaveData(1);
            }
        }
        [Command("transfer"), Description("Command for users to transfer money to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the money to")] DiscordMember user, [Description("Amount of money to award")] int money = -1)
        {

            if (money > 0)
            {
                if (RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.Money >= money)
                {
                    RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.Money += money;
                    RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.Money -= money;
                    UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                    UserObject.RootObject b = RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id);

                    await e.RespondAsync("User: " + user.DisplayName + " now has $" + a.UserData.Money + "\n\n" + "User: " + user.DisplayName + " now has $" + b.UserData.Money);

                    RPClass.SaveData(1);
                }
                else
                {
                    await e.RespondAsync("You don't have enough money to do that.");
                }
            }
        }
        [Command("balance"), Aliases("bal"), Description("Prints the user's current balance.")]
        public async Task Balance(CommandContext e, [Description("Use all keyword to see everyone's balance (Admin only), or @mention someone to view their balance")] string all = "")
        {
            var members = await e.Guild.GetAllMembersAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Prometheus RP");

            if (all == "all" && e.Member.Roles.Any(x => x == RPClass.StaffRole))
            {
                foreach (UserObject.RootObject userData in RPClass.Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(members.First(x => x.Id == userData.UserData.UserID).DisplayName, "Money: $" + userData.UserData.Money);
                    }
                    else
                    {
                        await e.RespondAsync("", embed: embed);
                        await Task.Delay(500);
                        embed.ClearFields();
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(all))
            {
                all = all.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
                if (ulong.TryParse(all, out ulong userNum))
                {
                    embed.AddField(members.First(x => x.Id == userNum).DisplayName, "Money: $" + RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.Money);
                }
                else
                {
                    await e.RespondAsync("Mention a user to select them.");
                }
            }
            else
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == e.Member.Id);

                embed.AddField(members.First(x => x.Id == userData.UserData.UserID).DisplayName, "Money: $" + userData.UserData.Money);
            }
            await e.RespondAsync("", embed: embed);
        }
    }
}
