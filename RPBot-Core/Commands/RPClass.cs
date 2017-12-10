﻿using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using System;
using System.Threading;
using Newtonsoft.Json;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace RPBot
{
    class RPClass : Program
    {
        public static List<UserObject.RootObject> Users = new List<UserObject.RootObject>();
        public static List<ShopObject.RootObject> ItemsList = new List<ShopObject.RootObject>();
        public static List<GuildObject.RootObject> Guilds = new List<GuildObject.RootObject>();
        public static List<SpeechObject.RootObject> SpeechList = new List<SpeechObject.RootObject>();
        public static List<InstanceObject.RootObject> InstanceList = new List<InstanceObject.RootObject>();
        public static List<InstanceObject.ChannelTemplate> ChannelTemplates = new List<InstanceObject.ChannelTemplate>();
        public static Dictionary<ulong, ulong> approvalsList = new Dictionary<ulong, ulong>(); // Channel ID : User ID
        public static TOSObject.RootObject TOSData = new TOSObject.RootObject();
        public static DiscordChannel GuildRankingChannel;
        public static DiscordChannel PlayerRankingChannel;
        public static DiscordChannel VillainRankingChannel;
        public static DiscordChannel RogueRankingChannel;
        public static DiscordChannel StatsChannel;
        public static DiscordChannel ApprovalsCategory;
        public static DiscordRole StaffRole;
        public static DiscordRole HelpfulRole;
        public static DiscordRole PunishedRole;

        public static bool Restarted = false;

        public static void SaveData(int saveType)
        {
            if (saveType == -1)
            {
                SaveData(1);
                SaveData(2);
                SaveData(3);
                SaveData(4);
                SaveData(6);
                SaveData(7);
                SaveData(8);
            }
            if (saveType == 1)
            {
                string output = JsonConvert.SerializeObject(Users, Formatting.Indented);
              
                File.WriteAllText("Data/UserData.txt", output);
            }
			else if (saveType == 2)
            {
                string output = JsonConvert.SerializeObject(ItemsList, Formatting.Indented);

                File.WriteAllText("Data/ShopData.txt", output);
            }
            else if (saveType == 3)
            {
                string output = JsonConvert.SerializeObject(Guilds, Formatting.Indented);

                File.WriteAllText("Data/GuildData.txt", output);
            }
            else if (saveType == 4)
            {
                string output = JsonConvert.SerializeObject(SpeechList, Formatting.Indented);
                File.WriteAllText("Data/SpeechData.txt", output);
            }
            else if (saveType == 5)
            {
                string output = ".";
                File.WriteAllText("Restart.txt", output);
            }
            else if (saveType == 6)
            {
                string output = JsonConvert.SerializeObject(ChannelTemplates, Formatting.Indented);
                File.WriteAllText("Data/ChannelTemplates.txt", output);
            }
            else if (saveType == 7)
            {
                string output = JsonConvert.SerializeObject(InstanceList, Formatting.Indented);
                File.WriteAllText("Data/InstanceData.txt", output);
            }
            else if (saveType == 8)
            {
                string output = JsonConvert.SerializeObject(approvalsList, Formatting.Indented);
                File.WriteAllText("Data/ApprovalsList.txt", output);
            }
        }

        public static void LoadData()
        {
            if (File.ReadAllLines("Data/UserData.txt").Any())
            {
                List<UserObject.RootObject> input = JsonConvert.DeserializeObject<List<UserObject.RootObject>>(File.ReadAllText("Data/UserData.txt"));
                Users = input;

            }
            /*
            if (File.ReadAllLines("a.txt").Any())
            {
                string[] a = File.ReadAllLines("a.txt");
                foreach (string b in a)
                {
                    string[] c = b.Split(';');
                    Users.First(x => x.UserData.username == c[0]).xp = int.Parse(c[1]);
                    Console.WriteLine(c[0]);
                }

            }*/
            TriviaClass.TriviaList = Directory.GetFiles("trivia/").ToList();
            for (int i = 0; i < TriviaClass.TriviaList.Count; i++)
            {
                TriviaClass.TriviaList[i] = TriviaClass.TriviaList[i].Replace(".txt", "").Replace("trivia/", "");
            }

            if (File.ReadAllLines("Data/ShopData.txt").Any())
            {
                List<ShopObject.RootObject> input = JsonConvert.DeserializeObject<List<ShopObject.RootObject>>(File.ReadAllText("Data/ShopData.txt"));
                ItemsList = input;
            }
            if (File.ReadAllLines("Data/GuildData.txt").Any())
            {
                List<GuildObject.RootObject> input = JsonConvert.DeserializeObject<List<GuildObject.RootObject>>(File.ReadAllText("Data/GuildData.txt"));
                Guilds = input;
            }
            if (File.ReadAllLines("Data/SpeechData.txt").Any())
            {
                List<SpeechObject.RootObject> input = JsonConvert.DeserializeObject<List<SpeechObject.RootObject>>(File.ReadAllText("Data/SpeechData.txt"));
                SpeechList = input;
            }
            if (File.ReadAllLines("Data/ChannelTemplates.txt").Any())
            {
                List<InstanceObject.ChannelTemplate> input = JsonConvert.DeserializeObject<List<InstanceObject.ChannelTemplate>>(File.ReadAllText("Data/ChannelTemplates.txt"));
                ChannelTemplates = input;
            }
            if (File.ReadAllLines("Data/InstanceData.txt").Any())
            {
                List<InstanceObject.RootObject> input = JsonConvert.DeserializeObject<List<InstanceObject.RootObject>>(File.ReadAllText("Data/InstanceData.txt"));
                InstanceList = input;
            }
            if (File.ReadAllLines("Data/ApprovalsList.txt").Any())
            {
                Dictionary<ulong, ulong> input = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText("Data/ApprovalsList.txt"));
                approvalsList = input;
            }
        }

        public static async Task AddOrUpdateUsers(DiscordGuild guild, bool update)
        {
            List<DiscordMember> AllUsers = new List<DiscordMember>(await guild.GetAllMembersAsync());
            AllUsers.RemoveAll(x => x.IsBot);
            List<DiscordChannel> Channels = new List<DiscordChannel>(await guild.GetChannelsAsync());

            foreach (DiscordMember user in AllUsers)
            {
                int role = 0;
                if (user.Roles.Any(x => x.Id == 312980663638818817 || x.Id == 312981984790052866 || x.Id == 312980292086530048))
                {
                    role = 1;
                }
                else if (user.Roles.Any(x => x.Id == 312982325908471808 || x.Id == 312982262310502401))
                {
                    role = 2;
                }
                else if (user.Roles.Any(x => x.Id == 317915877775245312))
                {
                    role = 3;
                }
                if (!Users.Any(x => x.UserData.userID == user.Id))
                {
                    Users.Add(new UserObject.RootObject(new UserObject.UserData(user.Id, user.DisplayName, role, 1, 0, 0, 0, 0), 0, new UserObject.InvData(new List<int>())));
                }
                if (Users.Find(x => x.UserData.userID == user.Id).UserData.username != user.DisplayName)
                {
                    Users.Find(x => x.UserData.userID == user.Id).UserData.username = user.DisplayName;
                }
                if (Users.Find(x => x.UserData.userID == user.Id).UserData.role != role)
                {
                    Users.Find(x => x.UserData.userID == user.Id).UserData.role = role;
                }
            }
            if (update)
            {
                await XPClass.UpdateStats(StatsChannel);
                await XPClass.UpdateGuildRanking(guild);
                await XPClass.UpdatePlayerRanking(guild, 1);
                await XPClass.UpdatePlayerRanking(guild, 2);
                await XPClass.UpdatePlayerRanking(guild, 3);

            }
            SaveData(-1);
        }

		public static async Task UpdateClock(MessageCreateEventArgs e, DiscordClient d)
        {
            DiscordGuild RPGuild = e.Guild;
            List<DiscordChannel> RPChannels = new List<DiscordChannel>(await e.Guild.GetChannelsAsync());
            DiscordChannel AnnouncementChannel = RPChannels.First(x => x.Id == 312918289988976653);
            DateTime y = DateTime.Now.AddHours(-4);
            while (true)
            {
				await d.UpdateStatusAsync(new DiscordGame(DateTime.Now.DayOfWeek + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute.ToString("00")));

                string TimePhase = "";
                if (DateTime.Now.Minute == 0 && DateTime.Now.Hour == 6 && y.AddHours(2) < DateTime.Now)
                {
                    TimePhase = "It is now dawn, on " + DateTime.Now.DayOfWeek;
                    y = DateTime.Now;
					await AnnouncementChannel.SendMessageAsync(TimePhase);
                }
                else if (DateTime.Now.Minute == 0 && DateTime.Now.Hour == 12 && y.AddHours(2) < DateTime.Now)
                {
                    TimePhase = "It is now midday, on " + DateTime.Now.DayOfWeek;
                    y = DateTime.Now;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);

                }
                else if (DateTime.Now.Minute == 0 && DateTime.Now.Hour == 18 && y.AddHours(2) < DateTime.Now)
                {
                    
                    TimePhase = "It is now dusk, on " + DateTime.Now.DayOfWeek;
                    y = DateTime.Now;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);
                    
                }

                await Task.Delay(45000);
            }
        }
    }
}