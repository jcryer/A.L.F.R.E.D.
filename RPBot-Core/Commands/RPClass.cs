using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using System;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using PasteSharp;

namespace RPBot
{
    public static class RPClass
    {
        public static List<UserObject.RootObject> Users = new List<UserObject.RootObject>();
        public static List<GuildObject.RootObject> Guilds = new List<GuildObject.RootObject>();
        public static List<InstanceObject.ChannelTemplate> ChannelTemplates = new List<InstanceObject.ChannelTemplate>();
        public static List<TagObject.RootObject> TagsList = new List<TagObject.RootObject>();
        public static List<SignupObject.RootObject> SignupList = new List<SignupObject.RootObject>();
        public static List<StatusObject.RootObject> StatusList = new List<StatusObject.RootObject>();
        public static Dictionary<ulong, ulong> ApprovalsList = new Dictionary<ulong, ulong>(); // Channel ID : User ID
        public static Dictionary<DiscordMember, DateTime> slowModeList = new Dictionary<DiscordMember, DateTime>(); //  Member : Timeout
        public static int slowModeTime = -1;
       
        public static DiscordChannel ApprovalsCategory;
        public static DiscordChannel InstanceCategory;
        public static DiscordChannel LogChannel;
        public static DiscordChannel GeneralChannel;
        public static DiscordChannel XPChannel;
        public static DiscordChannel StatsChannel;
        public static DiscordChannel GuildChannel;

        public static DiscordRole StaffRole;
        public static DiscordRole HelpfulRole;
        public static DiscordRole PunishedRole;
        public static DiscordRole AdminRole;
        public static List<ulong> RoleIDs = new List<ulong>() { 472534171961786368, 472534239611453469, 472534316908544001, 472534335493505034, 472534573117472778 };
        public static DiscordGuild RPGuild;
        public static Random Random = new Random();
        
        public static bool FirstRun = true;
        public static bool Restarted = false;

        public static void SaveData(int saveType)
        {
            if (saveType == -1)
            {
                SaveData(1);
                SaveData(2);
                SaveData(3);
                SaveData(6);
                SaveData(7);
                SaveData(8);
                SaveData(9);
            }
            if (saveType == 1)
            {
                string output = JsonConvert.SerializeObject(Users, Formatting.Indented);
                File.WriteAllText("Data/UserData.txt", output);
            }
            else if (saveType == 2)
            {
                string output = JsonConvert.SerializeObject(SignupList, Formatting.Indented);

                File.WriteAllText("Data/SignupList.txt", output);
            }
            else if (saveType == 3)
            {
                string output = JsonConvert.SerializeObject(Guilds, Formatting.Indented);

                File.WriteAllText("Data/GuildData.txt", output);
            }
            else if (saveType == 6)
            {
                string output = JsonConvert.SerializeObject(ChannelTemplates, Formatting.Indented);
                File.WriteAllText("Data/ChannelTemplates.txt", output);
            }
            else if (saveType == 7)
            {
                string output = JsonConvert.SerializeObject(StatusList, Formatting.Indented);

                File.WriteAllText("Data/StatusData.txt", output);
            }
            else if (saveType == 8)
            {
                string output = JsonConvert.SerializeObject(ApprovalsList, Formatting.Indented);
                File.WriteAllText("Data/ApprovalsList.txt", output);
            }
            else if (saveType == 9)
            {
                string output = JsonConvert.SerializeObject(TagsList, Formatting.Indented);
                File.WriteAllText("Data/TagsList.txt", output);
            }
        }

        public static void LoadData()
        {
            if (File.Exists("Data/UserData.txt"))
            {
                List<UserObject.RootObject> input = JsonConvert.DeserializeObject<List<UserObject.RootObject>>(File.ReadAllText("Data/UserData.txt"));
                Users = input;
            }
            if (File.Exists("Data/GuildData.txt"))
            {
                List<GuildObject.RootObject> input = JsonConvert.DeserializeObject<List<GuildObject.RootObject>>(File.ReadAllText("Data/GuildData.txt"));
                Guilds = input;
            }
            if (File.Exists("Data/ChannelTemplates.txt"))
            {
                List<InstanceObject.ChannelTemplate> input = JsonConvert.DeserializeObject<List<InstanceObject.ChannelTemplate>>(File.ReadAllText("Data/ChannelTemplates.txt"));
                ChannelTemplates = input;
            }
            if (File.Exists("Data/StatusData.txt"))
            {
                List<StatusObject.RootObject> input = JsonConvert.DeserializeObject<List<StatusObject.RootObject>>(File.ReadAllText("Data/StatusData.txt"));
                StatusList = input;
            }
            if (File.Exists("Data/ApprovalsList.txt"))
            {
                Dictionary<ulong, ulong> input = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText("Data/ApprovalsList.txt"));
                ApprovalsList = input;
            }
            if (File.Exists("Data/TagsList.txt"))
            {
                List<TagObject.RootObject> input = JsonConvert.DeserializeObject<List<TagObject.RootObject>>(File.ReadAllText("Data/TagsList.txt"));
                TagsList = input;
            }
            if (File.Exists("Data/SignupList.txt"))
            {
                List<SignupObject.RootObject> input = JsonConvert.DeserializeObject<List<SignupObject.RootObject>>(File.ReadAllText("Data/SignupList.txt"));
                SignupList = input;
            }
        }
        
		public static async Task UpdateClock(MessageCreateEventArgs e, DiscordClient d)
        {
            DiscordGuild RPGuild = e.Guild;
            List<DiscordChannel> RPChannels = new List<DiscordChannel>(await e.Guild.GetChannelsAsync());
            DiscordChannel AnnouncementChannel = RPChannels.First(x => x.Id == 471749965757284354);
            DateTime y = DateTime.UtcNow.AddHours(-4);

            while (true)
            {
				await d.UpdateStatusAsync(new DiscordActivity("Time pass: " + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute.ToString("00"), ActivityType.Watching)); 

                string TimePhase = "";
                if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Hour == 6 && y.AddHours(2) < DateTime.UtcNow)
                {
                    TimePhase = "It is now dawn, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
					await AnnouncementChannel.SendMessageAsync(TimePhase);
                }
                else if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Hour == 12 && y.AddHours(2) < DateTime.UtcNow)
                {
                    TimePhase = "It is now midday, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);

                }
                else if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Hour == 18 && y.AddHours(2) < DateTime.UtcNow)
                {
                    
                    TimePhase = "It is now dusk, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);
                }

                else if (DateTime.UtcNow.Minute == 0 && (DateTime.UtcNow.Hour == 0 || DateTime.UtcNow.Hour == 24) && y.AddHours(2) < DateTime.UtcNow)
                {

                    TimePhase = "It is now midnight, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);
                }
                await Task.Delay(45000);
            }
        }
    }
}
