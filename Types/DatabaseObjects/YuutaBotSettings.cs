using System;
using System.Collections.Generic;
using System.Text;
using Types.DatabaseObjects.DiscordObjects;

namespace Types.DatabaseObjects {
    public class YuutaBotSettings {

        public IEnumerable<DiscordStatus> BotStatuses { get; set; }

    }
}
