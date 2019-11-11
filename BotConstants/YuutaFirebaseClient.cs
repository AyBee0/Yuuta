using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Types;

namespace FirebaseHelper {

    public class YuutaFirebaseClient {

        private const string AppSecret = "i98LCHHGBPtpqd0acRoaGXSwwM3n9H18YIQP4WQh";
        public ChildQuery CurrentQuery { get; private set; }
        public FirebaseClient Client { get; private set; }
        public static Dictionary<string, Guild> Guilds { get; set; }

        public YuutaFirebaseClient() {
            Client = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/", new FirebaseOptions {
                AuthTokenAsyncFactory = () => Task.FromResult(AppSecret)
            });
            CurrentQuery = Client.Child("Root");
        }

        public YuutaFirebaseClient Child(object child) {
            //CurrentQuery = CurrentQuery.Child("/" + child.ToString());
            //return this;
            return new YuutaFirebaseClient { Client = Client, CurrentQuery = CurrentQuery.Child("/" + child.ToString()) };
        }

        public async Task<T> GetValueAsync<T>() {
            return await CurrentQuery.OnceSingleAsync<T>();
        }

        public async Task SetValueAsync<T>(T obj) {
            await CurrentQuery.PutAsync(JsonConvert.SerializeObject(obj));
        }

        public async Task UpdateValueAsync<T>(T obj) {
            await CurrentQuery.PatchAsync(JsonConvert.SerializeObject(obj));
        }

        public async Task PushValueAsync<T>(T obj) {
            await CurrentQuery.PostAsync(JsonConvert.SerializeObject(obj));
        }

        public async Task DeleteValue() {
            await CurrentQuery.DeleteAsync();
        }

    }

}
