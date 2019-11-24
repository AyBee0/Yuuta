using DSharpPlus.EventArgs;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Types.DatabaseObjects;
using static FirebaseHelper.YuutaFirebaseClient;

namespace DiscordEvents {
    public class GuildMessageCreateAndEditEvents {

        public static async Task OnMessageCreated(MessageCreateEventArgs e) {
            //A message is created
            if (e.Message.Author.IsBot) {
                return;
            }
            var guilds = Database?.Guilds;
            var content = e.Message.Content.Trim();
            if (content == null || guilds == null || !guilds.ContainsKey(e.Guild.Id.ToString())) {
                return;
            }
            if (content.StartsWith(Guild.MacroPrefix) && !content.Contains(" ")) {
                var guild = guilds[e.Guild.Id.ToString()];
                var sentMacro = content.ToLower();
                foreach (var guildMacro in guild.GuildMacros) {
                    if (guildMacro.Value.Macro.ToLower().StartsWith(sentMacro)) {
                        await e.Channel.TriggerTypingAsync();
                        var response = guildMacro.Value.MessageResponse;
                        var attachments = guildMacro.Value.Attachments;
                        var restClient = new RestClient();
                        Dictionary<string, Stream> files = new Dictionary<string, Stream>();
                        if (attachments != null) {
                            foreach (var attachment in attachments) {
                                var attachmentID = attachment.Key;
                                var url = attachment.Value.AttachmentURL;
                                var req = new RestRequest(url);
                                var pathSplitter = PathUtils.PathUtils.PathSplitter;
                                var folderPath = $"{AppDomain.CurrentDomain.BaseDirectory}images{pathSplitter}{guildMacro.Key}{pathSplitter}";
                                Directory.CreateDirectory(folderPath);
                                string imagePath = $"{folderPath}{attachmentID}{Path.GetExtension(url)}";
                                if (!File.Exists(imagePath)) {
                                    restClient.DownloadData(req, false).SaveAs(imagePath);
                                }
                                files.Add($"{attachmentID}{Path.GetExtension(url)}", File.OpenRead(imagePath));
                            }
                        }
                        if (guildMacro.Value.DeleteCommand) {
                            await e.Message.DeleteAsync();
                        }
                        await e.Channel.SendMultipleFilesAsync(files, response);
                        foreach (var keyValuePair in files) {
                            var fs = keyValuePair.Value;
                            fs.Close();
                        }
                        break;
                    }
                }
            }
        }

    }
}
