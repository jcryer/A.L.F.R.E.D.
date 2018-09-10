using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("status"), Aliases("st"), Description("Status commands"), RequireRoles(RoleCheckMode.Any, "Administrator", "Bot Mod"), IsMuted]
    class Status : BaseCommandModule
    {
        [Command("compare"), Description("Command for admins to compare status histories.")]
        public async Task Create(CommandContext e, DiscordMember person1, DiscordMember person2)
        {
            var statuses = RPClass.StatusList;

            if (!statuses.Any(x => x.UserID == person1.Id) && !statuses.Any(x => x.UserID == person2.Id))
            {
                await e.RespondAsync("No history for at least one of the specified people.");
                return;
            }

            var person_one = statuses.First(x => x.UserID == person1.Id);
            var person_two = statuses.First(x => x.UserID == person2.Id);

            var statuses_one_old = person_one.StatusData;
            var statuses_two_old = person_two.StatusData;

            if (statuses_one_old.First().Datetime < statuses_two_old.First().Datetime)
            {
                statuses_one_old = statuses_one_old.Where(x => x.Datetime >= statuses_two_old.First().Datetime).ToList();
            }
            else
            {
                statuses_two_old = statuses_two_old.Where(x => x.Datetime >= statuses_one_old.First().Datetime).ToList();
            }

            List<StatusObject.StatusData> statuses_one;
            List<StatusObject.StatusData> statuses_two;

            if (statuses_one_old.Count > statuses_two_old.Count)
            {
                statuses_one = statuses_one_old;
                statuses_two = statuses_two_old;
            }
            else
            {
                var person1_new = person1;
                person1 = person2;
                person2 = person1_new;
                statuses_two = statuses_one_old;
                statuses_one = statuses_two_old;
            }

            float hits = 0;
            float hits_same = 0;
            float hits_different = 0;

            foreach (var status_one in statuses_one)
            {
                var status_two = statuses_two.OrderBy(x => Math.Abs((x.Datetime - status_one.Datetime).Ticks)).FirstOrDefault();
                if (status_two == null) continue;
                if (Math.Abs((status_two.Datetime - status_one.Datetime).Minutes) < 10)
                {
                    hits += 1;
                    if (status_one.Status == UserStatus.Online || status_one.Status == UserStatus.DoNotDisturb)
                    {
                        if (status_two.Status == UserStatus.Online || status_two.Status == UserStatus.DoNotDisturb)
                        {
                            hits_same += 1;
                        }
                        else if (status_two.Status == UserStatus.Offline || status_two.Status == UserStatus.Invisible)
                        {
                            hits_different += 1;
                        }
                    }
                    else if (status_one.Status == UserStatus.Idle)
                    {
                        if (status_two.Status == UserStatus.Idle)
                        {
                            hits_same += 1;
                        }
                        else if (status_two.Status == UserStatus.Offline || status_two.Status == UserStatus.Invisible)
                        {
                            hits_different += 1;
                        }
                    }
                    else if (status_one.Status == UserStatus.Offline || status_one.Status == UserStatus.Invisible)
                    {
                        if (status_two.Status == UserStatus.Offline || status_two.Status == UserStatus.Invisible)
                        {
                            hits_same += 1;
                        }
                        else if (status_two.Status == UserStatus.Online || status_two.Status == UserStatus.DoNotDisturb)
                        {
                            hits_different += 1;
                        }
                    }
                }
            }

            await e.RespondAsync($"Status values saved for {person1.DisplayName}: {statuses_one.Count}\n" +
                $"Status values saved for {person2.DisplayName}: {statuses_two.Count}\n" +
                $"Time-Based Hits: {hits} - {(int)((hits / statuses_one.Count) * 100)}% of recorded values\n" +
                $"Time-Based Hits with similar statuses: {hits_same} - {Math.Round(hits_same / statuses_one.Count, 2) * 100}% of recorded values\n" +
                $"Time-Based Hits with opposite statuses: {hits_different} - {Math.Round(hits_different / statuses_one.Count, 2) * 100}% of recorded values\n");

        }

        [Command("scan"), Description("Command for admins to compare status histories.")]
        public async Task Scan(CommandContext e)
        {
            var statuses = RPClass.StatusList;
            var members = await e.Guild.GetAllMembersAsync();
            string response = "Scan:\n\n";
            foreach (var person_one in statuses.Where(x => x.StatusData.Any()))
            {
                foreach (var person_two in statuses.Where(x => x.StatusData.Any()))
                {
                    if (person_one != person_two)
                    {
                        var statuses_one_old = person_one.StatusData;
                        var statuses_two_old = person_two.StatusData;

                        if (statuses_one_old.First().Datetime < statuses_two_old.First().Datetime)
                        {
                            statuses_one_old = statuses_one_old.Where(x => x.Datetime >= statuses_two_old.First().Datetime).ToList();
                        }
                        else
                        {
                            statuses_two_old = statuses_two_old.Where(x => x.Datetime >= statuses_one_old.First().Datetime).ToList();
                        }

                        List<StatusObject.StatusData> statuses_one;
                        List<StatusObject.StatusData> statuses_two;

                        if (statuses_one_old.Count > statuses_two_old.Count)
                        {
                            statuses_one = statuses_one_old;
                            statuses_two = statuses_two_old;
                        }
                        else
                        {
                            statuses_two = statuses_one_old;
                            statuses_one = statuses_two_old;
                        }

                        float hits = 0;
                        float hits_same = 0;
                        float hits_different = 0;

                        foreach (var status_one in statuses_one)
                        {
                            var status_two = statuses_two.OrderBy(x => Math.Abs((x.Datetime - status_one.Datetime).Ticks)).FirstOrDefault();
                            if (status_two == null) continue;
                            if (Math.Abs((status_two.Datetime - status_one.Datetime).Minutes) < 10)
                            {
                                hits += 1;
                                if (status_one.Status == UserStatus.Online || status_one.Status == UserStatus.DoNotDisturb)
                                {
                                    if (status_two.Status == UserStatus.Online || status_two.Status == UserStatus.DoNotDisturb)
                                    {
                                        hits_same += 1;
                                    }
                                    else if (status_two.Status == UserStatus.Offline || status_two.Status == UserStatus.Invisible)
                                    {
                                        hits_different += 1;
                                    }
                                }
                                else if (status_one.Status == UserStatus.Idle)
                                {
                                    if (status_two.Status == UserStatus.Idle)
                                    {
                                        hits_same += 1;
                                    }
                                    else if (status_two.Status == UserStatus.Offline || status_two.Status == UserStatus.Invisible)
                                    {
                                        hits_different += 1;
                                    }
                                }
                                else if (status_one.Status == UserStatus.Offline || status_one.Status == UserStatus.Invisible)
                                {
                                    if (status_two.Status == UserStatus.Offline || status_two.Status == UserStatus.Invisible)
                                    {
                                        hits_same += 1;
                                    }
                                    else if (status_two.Status == UserStatus.Online || status_two.Status == UserStatus.DoNotDisturb)
                                    {
                                        hits_different += 1;
                                    }
                                }
                            }
                        }
                        if (hits/statuses_one.Count > 0.5 && hits > 20)
                        {
                            try
                            {
                                float endHits = 0;
                                if (hits_same > hits_different) endHits = hits_same;
                                else endHits = hits_different;
                                response += $"{members.First(x => x.Id == person_one.UserID).DisplayName} & {members.First(x => x.Id == person_two.UserID).DisplayName} - {endHits} hits - {(int)((endHits / statuses_one.Count) * 100)}%\n";
                            }
                            catch
                            {

                            }
                        }

                    }

                }
            }
            await e.RespondAsync(response);
        }
    }
}
