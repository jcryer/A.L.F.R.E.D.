﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPBot
{
    class ModClass : BaseCommandModule
    {
        [Command("mute"), Aliases("ultmute", "ultimatepunish", "upunish", "umute", "um", "up", "ultimute", "begone", "punish"), Description("Command for admins to temporarily strip away a user's ranks when muted."), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
        public async Task UltimateMute(CommandContext e, [Description("Member to be muted")] DiscordMember user, bool silent = false)
        {
            try
            {
                UserObject.RootObject userObject = RPClass.Users.First(x => x.UserData.UserID == user.Id);
                
                if (userObject.ModData.IsMuted == 1)
                {
                    userObject.ModData.IsMuted = 0;
                    await user.ReplaceRolesAsync(userObject.ModData.Roles);
                    if (!silent)
                        await e.RespondAsync("User unmuted.");

                }
                else
                {
                    if (user.Roles.Any(x => x == RPClass.AdminRole) && !e.Member.Roles.Any(x => x == RPClass.AdminRole))
                    {
                        if (!silent)
                            await e.RespondAsync("Admins are the master race, leave us alone.");
                        return;
                    }
                    userObject.ModData.IsMuted = 1;
                    userObject.ModData.Roles = user.Roles.ToList();
                    await user.ReplaceRolesAsync(new List<DiscordRole>() { RPClass.PunishedRole });
                    if (!silent)
                        await e.RespondAsync("User muted.");
                }
                RPClass.SaveData(1);

            }
            catch
            {
                if (!silent)
                    await e.RespondAsync("NO");
            }
        }


        [Command("ultibulk"), Aliases ("allmute", "am", "ub", "ultib", "allm"), Description("Staff command to mute multiple people."), RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task Bulk(CommandContext e)
        {

            await e.RespondAsync("Mention a user to ultimute/un ultimute them. To end this process, type `stop`.");
            var interactivity = e.Client.GetInteractivity();

            AnotherMessage:

            var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
            if (msg != null)
            {
                if (msg.Message.Content == "stop")
                {
                    await e.RespondAsync("Ultibulk complete.");
                }
                else
                {
                    try
                    {
                        DiscordMember member = await e.CommandsNext.ConvertArgument(msg.Message.Content, e, typeof(DiscordMember)) as DiscordMember;

                        await UltimateMute(e, member, true);
                    }
                    catch
                    {
                        await e.RespondAsync("Error.");
                    }
                    goto AnotherMessage;

                }
            }
            else
            {
                await e.RespondAsync("Ultibulk complete.");
            }
        }
    }
}
