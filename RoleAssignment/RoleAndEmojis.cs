using System;
using System.Collections.Generic;

namespace RoleAssignment {
    public class GameRole {

        public ulong EmojiId { get; }
        public ulong RoleId { get; }
        public string RoleName { get; }

        public GameRole(ulong emojiId, ulong roleId, string roleName) {
            EmojiId = emojiId;
            RoleId = roleId;
            RoleName = roleName;
        }

        public static List<GameRole> ParseRole(ulong reactionId) {
            switch (reactionId) {
                #region Games
                //overwatch
                case RoleVariables.TheBeacon.Emojis.Games.Overwatch:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Overwatch, RoleVariables.TheBeacon.RoleIds.Games.Overwatch, RoleVariables.TheBeacon.Names.Games.Overwatch) };
                //Rainbow 6
                case RoleVariables.TheBeacon.Emojis.Games.Rainbow:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Rainbow, RoleVariables.TheBeacon.RoleIds.Games.Rainbow, RoleVariables.TheBeacon.Names.Games.Rainbow) };
                //Smash
                case RoleVariables.TheBeacon.Emojis.Games.Smash:
                    return new List<GameRole> {
                        new GameRole(RoleVariables.TheBeacon.Emojis.Games.Smash, RoleVariables.TheBeacon.RoleIds.Games.Smash, RoleVariables.TheBeacon.Names.Games.Smash),
                        new GameRole(RoleVariables.TheBeacon.Emojis.Games.Nintendo, RoleVariables.TheBeacon.RoleIds.Games.Nintendo, RoleVariables.TheBeacon.Names.Games.Nintendo)
                        };
                //CSGO
                case RoleVariables.TheBeacon.Emojis.Games.CSGO:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.CSGO, RoleVariables.TheBeacon.RoleIds.Games.CSGO, RoleVariables.TheBeacon.Names.Games.CSGO) };
                //DAUNTLESS
                case RoleVariables.TheBeacon.Emojis.Games.Dauntless:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Dauntless, RoleVariables.TheBeacon.RoleIds.Games.Dauntless, RoleVariables.TheBeacon.Names.Games.Dauntless) };
                //DESTINY
                case RoleVariables.TheBeacon.Emojis.Games.Destiny:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Destiny, RoleVariables.TheBeacon.RoleIds.Games.Destiny, RoleVariables.TheBeacon.Names.Games.Destiny) };
                //FORTNITE
                case RoleVariables.TheBeacon.Emojis.Games.Fortnite:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Fortnite, RoleVariables.TheBeacon.RoleIds.Games.Fortnite, RoleVariables.TheBeacon.Names.Games.Fortnite) };
                //LEAGUE
                case RoleVariables.TheBeacon.Emojis.Games.League:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.League, RoleVariables.TheBeacon.RoleIds.Games.League, RoleVariables.TheBeacon.Names.Games.League) };
                //MARIOKART
                case RoleVariables.TheBeacon.Emojis.Games.MarioKart:
                    return new List<GameRole> {
                        new GameRole(RoleVariables.TheBeacon.Emojis.Games.MarioKart, RoleVariables.TheBeacon.RoleIds.Games.MarioKart, RoleVariables.TheBeacon.Names.Games.MarioKart),
                        new GameRole(RoleVariables.TheBeacon.Emojis.Games.Nintendo, RoleVariables.TheBeacon.RoleIds.Games.Nintendo, RoleVariables.TheBeacon.Names.Games.Nintendo)
                    };
                //MINECRAFT
                case RoleVariables.TheBeacon.Emojis.Games.Minecraft:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Minecraft, RoleVariables.TheBeacon.RoleIds.Games.Minecraft, RoleVariables.TheBeacon.Names.Games.Minecraft) };
                //MTG
                case RoleVariables.TheBeacon.Emojis.Games.Magic:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Magic, RoleVariables.TheBeacon.RoleIds.Games.Magic, RoleVariables.TheBeacon.Names.Games.Magic) };
                //PUBG
                case RoleVariables.TheBeacon.Emojis.Games.Pubg:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Pubg, RoleVariables.TheBeacon.RoleIds.Games.Pubg, RoleVariables.TheBeacon.Names.Games.Pubg) };
                //ROLLER CHAMPIONS
                case RoleVariables.TheBeacon.Emojis.Games.RollerChampions:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.RollerChampions, RoleVariables.TheBeacon.RoleIds.Games.RollerChampions, RoleVariables.TheBeacon.Names.Games.RollerChampions) };
                //SMM2
                case RoleVariables.TheBeacon.Emojis.Games.SMM2:
                    return new List<GameRole> {
                        new GameRole(RoleVariables.TheBeacon.Emojis.Games.SMM2, RoleVariables.TheBeacon.RoleIds.Games.SMM2, RoleVariables.TheBeacon.Names.Games.SMM2),
                        new GameRole(RoleVariables.TheBeacon.Emojis.Games.Nintendo, RoleVariables.TheBeacon.RoleIds.Games.Nintendo, RoleVariables.TheBeacon.Names.Games.Nintendo)
                    };
                //SPLATOON
                case RoleVariables.TheBeacon.Emojis.Games.Splatoon:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Splatoon, RoleVariables.TheBeacon.RoleIds.Games.Splatoon, RoleVariables.TheBeacon.Names.Games.Splatoon),
                    new GameRole(RoleVariables.TheBeacon.Emojis.Games.Nintendo, RoleVariables.TheBeacon.RoleIds.Games.Nintendo, RoleVariables.TheBeacon.Names.Games.Nintendo)
                };
                //WARFRAME
                case RoleVariables.TheBeacon.Emojis.Games.Warframe:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Warframe, RoleVariables.TheBeacon.RoleIds.Games.Warframe, RoleVariables.TheBeacon.Names.Games.Warframe) };
                //APEX  
                case RoleVariables.TheBeacon.Emojis.Games.Apex:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Apex, RoleVariables.TheBeacon.RoleIds.Games.Apex, RoleVariables.TheBeacon.Names.Games.Apex) };
                //PALADINS
                case RoleVariables.TheBeacon.Emojis.Games.Paladins:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Games.Paladins, RoleVariables.TheBeacon.RoleIds.Games.Paladins, RoleVariables.TheBeacon.Names.Games.Paladins) };
                #endregion
                #region Platforms
                case RoleVariables.TheBeacon.Emojis.Platforms.PC:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Platforms.PC, RoleVariables.TheBeacon.RoleIds.Platforms.PC, RoleVariables.TheBeacon.Names.Platforms.PC) };
                case RoleVariables.TheBeacon.Emojis.Platforms.Xbox:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Platforms.Xbox, RoleVariables.TheBeacon.RoleIds.Platforms.Xbox, RoleVariables.TheBeacon.Names.Platforms.Xbox) };
                case RoleVariables.TheBeacon.Emojis.Platforms.PS4:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Platforms.PS4, RoleVariables.TheBeacon.RoleIds.Platforms.PS4, RoleVariables.TheBeacon.Names.Platforms.PS4) };
                #endregion
                #region Other Roles
                #region Events
                case RoleVariables.TheBeacon.Emojis.Other.Event:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.Event, RoleVariables.TheBeacon.RoleIds.Other.Event, RoleVariables.TheBeacon.Names.Other.Event) };
                case RoleVariables.TheBeacon.Emojis.Other.Movie_Night:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.Movie_Night, RoleVariables.TheBeacon.RoleIds.Other.Movie_Night, RoleVariables.TheBeacon.Names.Other.Movie_Night) };
                #endregion
                #region Regions
                case RoleVariables.TheBeacon.Emojis.Other.EU:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.EU, RoleVariables.TheBeacon.RoleIds.Other.EU, RoleVariables.TheBeacon.Names.Other.EU) };
                case RoleVariables.TheBeacon.Emojis.Other.NA:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.NA, RoleVariables.TheBeacon.RoleIds.Other.NA, RoleVariables.TheBeacon.Names.Other.NA) };
                case RoleVariables.TheBeacon.Emojis.Other.AS:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.AS, RoleVariables.TheBeacon.RoleIds.Other.AS, RoleVariables.TheBeacon.Names.Other.AS) };
                case RoleVariables.TheBeacon.Emojis.Other.OCE:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.OCE, RoleVariables.TheBeacon.RoleIds.Other.OCE, RoleVariables.TheBeacon.Names.Other.OCE) };
                #endregion
                #region LFG
                case RoleVariables.TheBeacon.Emojis.Other.LFG_EU:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.LFG_EU, RoleVariables.TheBeacon.RoleIds.Other.LFG_EU, RoleVariables.TheBeacon.Names.Other.LFG_EU) };
                case RoleVariables.TheBeacon.Emojis.Other.LFG_NA:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.LFG_NA, RoleVariables.TheBeacon.RoleIds.Other.LFG_NA, RoleVariables.TheBeacon.Names.Other.LFG_NA) };
                case RoleVariables.TheBeacon.Emojis.Other.LFG_AS:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.LFG_AS, RoleVariables.TheBeacon.RoleIds.Other.LFG_AS, RoleVariables.TheBeacon.Names.Other.LFG_AS) };
                case RoleVariables.TheBeacon.Emojis.Other.LFG_OCE:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.LFG_OCE, RoleVariables.TheBeacon.RoleIds.Other.LFG_OCE, RoleVariables.TheBeacon.Names.Other.LFG_OCE) };
                #endregion
                case RoleVariables.TheBeacon.Emojis.Other.Memes:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.Memes, RoleVariables.TheBeacon.RoleIds.Other.Memes, RoleVariables.TheBeacon.Names.Other.Memes) };
                case RoleVariables.TheBeacon.Emojis.Other.Free_Game:
                    return new List<GameRole> { new GameRole(RoleVariables.TheBeacon.Emojis.Other.Free_Game, RoleVariables.TheBeacon.RoleIds.Other.Free_Game, RoleVariables.TheBeacon.Names.Other.Free_Game) };
                #endregion
                //NULL
                default:
                    return null;
            }
        }

    }
}
