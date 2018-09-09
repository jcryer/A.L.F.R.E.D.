using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DSharpPlus.Net.WebSocket;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using PasteSharp;

namespace RPBot
{
    internal sealed class RPBot
    {
        private Config Config { get; }
        public DiscordClient Discord;
        private CommandsNextExtension CommandsNextService { get; }
		private InteractivityExtension InteractivityService { get; }
        private Timer GameGuard { get; set; }

        public RPBot(Config cfg, int shardid)
        {
            // global bot config
            this.Config = cfg;

            // discord instance config and the instance itself
            var dcfg = new DiscordConfiguration
            {
                AutoReconnect = true,
                LargeThreshold = 250,
                LogLevel = LogLevel.Info,
                Token = this.Config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false,
                ShardId = shardid,
                ShardCount = this.Config.ShardCount,
            };
            Discord = new DiscordClient(dcfg);
			//Discord.SetWebSocketClient<WebSocketSharpClient>();            
            // events
            Discord.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
            Discord.Ready += this.Discord_Ready;
            Discord.GuildAvailable += this.Discord_GuildAvailable;
            Discord.MessageCreated += this.Discord_MessageCreated;
            Discord.MessageDeleted += this.Discord_MessageDeleted;
            Discord.MessageUpdated += this.Discord_MessageUpdated;

            Discord.MessageReactionAdded += this.Discord_MessageReactionAdd;
            Discord.MessageReactionsCleared += this.Discord_MessageReactionRemoveAll;
            Discord.PresenceUpdated += this.Discord_PresenceUpdate;
            Discord.SocketClosed += this.Discord_SocketClose;
            Discord.GuildMemberAdded += this.Discord_GuildMemberAdded;
            Discord.GuildMemberRemoved += this.Discord_GuildMemberRemoved;
            Discord.SocketErrored += this.Discord_SocketError;
            Discord.VoiceStateUpdated += this.Discord_VoiceStateUpdated;
            Discord.ClientErrored += this.Discord_ClientErrored;

            // commandsnext config and the commandsnext service itself
            var cncfg = new CommandsNextConfiguration
            {
                StringPrefixes = new List<string>() { Config.CommandPrefix, "" },
                EnableDms = true,
                EnableMentionPrefix = true,
                CaseSensitive = false
            };

            this.CommandsNextService = Discord.UseCommandsNext(cncfg);
            this.CommandsNextService.CommandErrored += this.CommandsNextService_CommandErrored;
            this.CommandsNextService.CommandExecuted += this.CommandsNextService_CommandExecuted;

            CommandsNextService.RegisterCommands(typeof(InstanceClass));
            CommandsNextService.RegisterCommands(typeof(MoneyClass));
            CommandsNextService.RegisterCommands(typeof(CommandsClass));
            CommandsNextService.RegisterCommands(typeof(GuildClass));
            CommandsNextService.RegisterCommands(typeof(LogClass));
            CommandsNextService.RegisterCommands(typeof(ModClass));
            CommandsNextService.RegisterCommands(typeof(TagClass));
            CommandsNextService.RegisterCommands(typeof(WikiClass));
            CommandsNextService.RegisterCommands(typeof(StatsClass));
            CommandsNextService.RegisterCommands(typeof(SignupClass));
            CommandsNextService.RegisterCommands(typeof(XPClass));
            CommandsNextService.RegisterCommands(typeof(Status));


            // WikiClass.InitWiki();

            InteractivityConfiguration icfg = new InteractivityConfiguration();
			this.InteractivityService = Discord.UseInteractivity(icfg);
        }

        public async Task RunAsync()
        {
            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[{0:yyyy-MM-dd HH:mm:ss zzz}] ", e.Timestamp.ToLocalTime());

            var tag = e.Application;
            if (tag.Length > 12)
                tag = tag.Substring(0, 12);
            if (tag.Length < 12)
                tag = tag.PadLeft(12, ' ');
            Console.Write("[{0}] ", tag);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[{0}] ", string.Concat("SHARD ", this.Discord.ShardId.ToString("00")));

            switch (e.Level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }
            Console.Write("[{0}] ", e.Level.ToString().PadLeft(11));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(e.Message);
        }

        private async Task Discord_Ready(ReadyEventArgs e)
        {
            await Task.Delay(0);
        }

        private async Task Discord_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            
            await Task.Delay(0);

        }

        private async Task Discord_SocketClose(SocketCloseEventArgs e)
        {
            try
            {
                await RPClass.LogChannel.SendMessageAsync("Restarting: Socket Close");
            }
            catch
            {
            }
            RPClass.SaveData(-1);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"dotnet RPBot-Core.dll\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            Environment.Exit(-1);
            await Task.Delay(0);
        }

        private async Task Discord_ClientErrored(ClientErrorEventArgs e)
        {
            try
            {
                await RPClass.LogChannel.SendMessageAsync("Restarting: Client Error");
            }
            catch
            {
            }
            RPClass.SaveData(-1);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"dotnet RPBot-Core.dll\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            Environment.Exit(-1);
            await Task.Delay(0);
            
        }

        private async Task Discord_SocketError(SocketErrorEventArgs e)
        {
            try
            {
                await RPClass.LogChannel.SendMessageAsync("Restarting: Socket Error");
            }
            catch
            {
            }
            RPClass.SaveData(-1);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"dotnet RPBot-Core.dll\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            Environment.Exit(-1);
            await Task.Delay(0);
        }

        private async Task Discord_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder
                {
                    Title = "Welcome!"
                }
                .AddField("Welcome to the Prometheus RP Server, " + e.Member.Username + "!", @"
Please read " + e.Guild.Channels.First(x => x.Id == 473596208565190659).Mention + @" then go for " + e.Guild.Channels.First(x => x.Id == 472503660174245908).Mention + @", followed by " + e.Guild.Channels.First(x => x.Id == 471840372717387787).Mention + @". 
Then, once you have decided the character(and filled out the template), ask for an approval mod to give you a channel. If you have any questions ask in " + e.Guild.Channels.First(x => x.Id == 472512797096542236).Mention + @".

Hope you enjoy your time here " + e.Member.Mention + "!");

                await e.Member.SendMessageAsync("", embed: b);
                await RPClass.GeneralChannel.SendMessageAsync("", embed: b);

                DiscordEmbedBuilder c = new DiscordEmbedBuilder
                {
                    Title = "Member Joined",
                    Color = DiscordColor.Green
                }
                .AddField("Member", e.Member.DisplayName + "#" + e.Member.Discriminator + " (" + e.Member.Id + ")", true)
                .AddField("Timestamp", e.Member.JoinedAt.ToString(), true);

                await RPClass.LogChannel.SendMessageAsync(embed: c);
            }
        }

        private async Task Discord_GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder
                {
                    Title = "Goodbye!"
                }
                .AddField("Bye " + e.Member.DisplayName, "We didn't like them anyway.");
                await RPClass.GeneralChannel.SendMessageAsync("", embed: b);

                DiscordEmbedBuilder c = new DiscordEmbedBuilder
                {
                    Title = "Member Left",
                    Color = DiscordColor.Red
                }
                .AddField("Member", e.Member.DisplayName + "#" + e.Member.Discriminator + " (" + e.Member.Id + ")", true)
                .AddField("Timestamp", DateTime.UtcNow.ToString(), true);

                await RPClass.LogChannel.SendMessageAsync(embed: c);
            }
        }

        public async Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            if (e.Guild.Id == 471749965757284352)
            {
                if (RPClass.Restarted)
                {
                    DiscordChannel c = e.Guild.GetChannel(472049016012668949);
                    DiscordMember me = await e.Guild.GetMemberAsync(126070623855312896);
                    await c.SendMessageAsync("Restarted successfully, " + me.Mention + "!");
                }
                RPClass.ApprovalsCategory = e.Guild.GetChannel(471836412996747274);
                RPClass.InstanceCategory = e.Guild.GetChannel(473597059316973579);
                RPClass.XPChannel = e.Guild.GetChannel(487627384217141250);
                RPClass.StatsChannel = e.Guild.GetChannel(487659258134134784);
                RPClass.GuildChannel = e.Guild.GetChannel(487627478010036244);
                RPClass.StaffRole = e.Guild.GetRole(471840828449357879);

                RPClass.HelpfulRole = e.Guild.GetRole(472504792879595520);
                RPClass.PunishedRole = e.Guild.GetRole(473496184401428502);
                RPClass.AdminRole = e.Guild.GetRole(471850713111068682);
                RPClass.LogChannel = e.Guild.GetChannel(472054489512149032);
                RPClass.GeneralChannel = e.Guild.GetChannel(471749965757284354);
                RPClass.RPGuild = e.Guild;
                await UpdateUserList(e.Guild, true);
            }
            this.GameGuard = new Timer(TimerCallback, null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(15));

            this.Discord.DebugLogger.LogMessage(LogLevel.Info, "DSPlus", $"Guild available: {e.Guild.Name}", DateTime.UtcNow);
        }

        private Task Discord_PresenceUpdate(PresenceUpdateEventArgs e)
        {
            var user = RPClass.StatusList.FirstOrDefault(x => x.UserID == e.User.Id);
            if (user == null)
            {
                RPClass.StatusList.Add(new StatusObject.RootObject(e.User.Id));
                user = RPClass.StatusList.Last();
            }
            if (e.PresenceAfter != null)
            {
                user.AddStatus(DateTime.Now, e.PresenceAfter.Status);
                RPClass.SaveData(7);
            }
            
            return Task.Delay(0);
        }

        private async Task Discord_MessageDeleted(MessageDeleteEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild) {
                if (e.Message.Author != e.Client.CurrentUser)
                {
                    try
                    {
                        if (!e.Message.Content.StartsWith("!"))
                        {
                            DiscordEmbedBuilder b = new DiscordEmbedBuilder
                            {
                                Title = "Message Deleted",
                                Color = DiscordColor.Red
                            }
                            .AddField("Member", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
                            .AddField("Channel", e.Message.Channel.Name, true)
                            .AddField("Creation Timestamp", e.Message.CreationTimestamp.ToString(), true)
                            .AddField("Deletion Timestamp", e.Message.Timestamp.ToString(), true)
                            .AddField("Message", e.Message.Content.Any() ? e.Message.Content : "-", false)
                            .AddField("Attachments", e.Message.Attachments.Any() ? string.Join("\n", e.Message.Attachments.Select(x => x.Url)) : "-", false);

                            await RPClass.LogChannel.SendMessageAsync(embed: b);
                        }
                    }
                    catch { }
                }
            }
        }

        private async Task Discord_MessageUpdated(MessageUpdateEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            {
                if (e.Message.Author != e.Client.CurrentUser)
                {
                    try
                    {
                        if (!e.Message.Content.StartsWith("!"))
                        {
                            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                            {
                                Title = "Message Edited",
                                Color = DiscordColor.Orange
                            }
                            .AddField("Member", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
                            .AddField("Channel", e.Message.Channel.Name, true)
                            .AddField("Creation Timestamp", e.Message.CreationTimestamp.ToString(), true)
                            .AddField("Edit Timestamp", e.Message.EditedTimestamp.ToString(), true)
                            .AddField("Old Message", e.MessageBefore.Content.Any() ? e.MessageBefore.Content : "-", false)
                            .AddField("New Message", e.Message.Content.Any() ? e.Message.Content : "-", false)
                            .AddField("Attachments", e.Message.Attachments.Any() ? string.Join("\n", e.Message.Attachments.Select(x => x.Url)) : "-", false);

                            await RPClass.LogChannel.SendMessageAsync(embed: embed);
                        }
                    }
                    catch { }
                }
            }
        }

        public async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild) { 
                if (RPClass.FirstRun)
                {
                    try
                    {
                        RPClass.FirstRun = false;

                        Thread myNewThread = new Thread(async () => await RPClass.UpdateClock(e, Discord));
                        myNewThread.Start();
                    }
                    catch { }
                }
                if (!e.Message.Content.StartsWith("!"))
                {
                    if (e.Message.ChannelId == 312918289988976653 && e.Author.Id != e.Client.CurrentUser.Id)
                    {
                        if (RPClass.slowModeTime > 0)
                        {
                            var u = RPClass.slowModeList.FirstOrDefault(x => x.Key == e.Message.Author);
                            if (u.Key != null)
                            {

                                if (Math.Abs((u.Value - DateTime.UtcNow).TotalSeconds) <= RPClass.slowModeTime)
                                {
                                    if (!(e.Author as DiscordMember).Roles.Any(x => x == RPClass.AdminRole))
                                    {
                                        await e.Message.DeleteAsync();
                                    }
                                }
                                else
                                {
                                    RPClass.slowModeList[e.Message.Author as DiscordMember] = DateTime.UtcNow;
                                }
                            }
                            else
                            {
                                RPClass.slowModeList.Add(e.Message.Author as DiscordMember, DateTime.UtcNow);
                            }
                        }
                    }
                }
            }
        }

        private Task Discord_MessageReactionAdd(MessageReactionAddEventArgs e)
        {
            return Task.Delay(0);
        }

        private Task Discord_MessageReactionRemoveAll(MessageReactionsClearEventArgs e)
        {
            return Task.Delay(0);
        }

        private void TimerCallback(object _)
        {

        }

        private bool IsCommandMethod(MethodInfo method, Type return_type, params Type[] arg_types)
        {
            if (method.ReturnType != return_type)
                return false;

            var prms = method.GetParameters();
            if (prms.Length != arg_types.Length)
                return false;

            for (var i = 0; i < arg_types.Length; i++)
                if (prms[i].ParameterType != arg_types[i])
                    return false;

            return true;
        }

        private async Task CommandsNextService_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException)
                return;

            Discord.DebugLogger.LogMessage(LogLevel.Error, "CommandsNext", $"{e.Exception.GetType()}: {e.Exception.Message}", DateTime.UtcNow);
            
            var ms = e.Exception.Message;
            var st = e.Exception.StackTrace;

            ms = ms.Length > 1000 ? ms.Substring(0, 1000) : ms;
            st = st.Length > 1000 ? st.Substring(0, 1000) : st;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Title = "RIP",
                Description = ";-;",
                Color = new DiscordColor(0xFF0000),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Prometheus RP")
            .AddField("Command errored", $"```{e.Exception.GetType()} occured when executing `{ e.Command.Name }`.\n" + ms + "```")
            .AddField("Stack trace", $"```cs\n{st}\n```")
            .AddField("Source", e.Exception.Source)
            .AddField("Message", e.Exception.Message);
            await e.Context.RespondAsync("Command errored.");
            await RPClass.LogChannel.SendMessageAsync("", embed: embed);
        }

        private async Task CommandsNextService_CommandExecuted(CommandExecutionEventArgs e)
        {
            Discord.DebugLogger.LogMessage(LogLevel.Info, "CommandsNext", $"{e.Context.User.Username} executed {e.Command.Name} in {e.Context.Channel.Name}", DateTime.UtcNow);
            await Task.Delay(0);
        }

        public static async Task UpdateUserList(DiscordGuild e, bool update)
        {
            var members = await e.GetAllMembersAsync();
            foreach (var user in members)
            {
                if (!RPClass.Users.Any(x => x.UserData.UserID == user.Id))
                {
                    RPClass.Users.Add(new UserObject.RootObject(user.Id));
                }
            }

            if (update)
            {
                await XPClass.UpdateStats();
                await XPClass.UpdatePlayerRanking(e);
                await XPClass.UpdateGuildRanking(e);
            }
           
        }
    }
}
