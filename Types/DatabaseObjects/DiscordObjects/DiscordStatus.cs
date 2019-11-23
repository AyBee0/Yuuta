using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Types.DatabaseObjects.DiscordObjects {

    public class DiscordStatus {

        [JsonIgnore]
        public DiscordActivity Activity { get; set; }

        [JsonProperty("ActivityText")]
        private string StatusActivityText
        {
            get {
                return Activity.Name;
            }
            set {
                Activity = Activity ?? DefaultDiscordActivity();
                Activity.Name = value;
            }
        }

        [JsonProperty("ActivityType")]
        private ActivityType StatusActivityType
        {
            get {
                return Activity.ActivityType;
            }
            set {
                Activity = Activity ?? DefaultDiscordActivity();
                Activity.ActivityType = value; 
            }
        }

        public bool Current { get; set; } = false;

        public static DiscordActivity DefaultDiscordActivity(string name = "Do I overrate chuunibyou? No. Stick your tongue into a power outlet.", ActivityType activityType = ActivityType.Playing) {
            return new DiscordActivity(name, activityType);
        }
    }

}
