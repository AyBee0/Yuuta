using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Types;

namespace FirebaseHelper {

    public class YuutaFirebaseClient {

        private const string AppSecret = "i98LCHHGBPtpqd0acRoaGXSwwM3n9H18YIQP4WQh";
        public const string BaseURL = "https://the-beacon-team-battles.firebaseio.com/";
        public ChildQuery CurrentQuery { get; private set; }
        public FirebaseClient Client { get; private set; }
        public static YuutaBot Database { get; set; }

        /// <summary>
        /// Creates a new YuutaFirebaseClient object: Easier to use than whatever the hell Firebasedatabase.net does.
        /// </summary>
        /// <param name="withYuutaBotChild">FOT THE LOVE OF GOD NEVER PASS THIS AS FALSE UNLESS YOU'RE USING AN OBSERVABLE TO ROOT</param>
        public YuutaFirebaseClient(bool withYuutaBotChild = true) {
            try {
                Client = new FirebaseClient(BaseURL, new FirebaseOptions {
                    AuthTokenAsyncFactory = () => Task.FromResult(AppSecret)
                });
                CurrentQuery = Client.Child($"Root{(withYuutaBotChild ? "/YuutaBot" : "")}");
            } catch (System.Exception) {
                throw;
            }
        }

        public YuutaFirebaseClient Child(object child) {
            //CurrentQuery = CurrentQuery.Child("/" + child.ToString());
            //return this;
            return new YuutaFirebaseClient { Client = Client, CurrentQuery = CurrentQuery.Child("/" + child.ToString()) };
        }

        /// <summary>
        /// Get's the value of the current child and parses it into a passed class.
        /// </summary>
        /// <typeparam name="T">Class to parse value into</typeparam>
        /// <returns></returns>
        public async Task<T> GetValueAsync<T>() {
            return await CurrentQuery.OnceSingleAsync<T>();
        }

        /// <summary>
        /// Overwrite the entirety of the current child's value to a value. Classes can be passed.
        /// </summary>
        /// <typeparam name="T">Class type to set the data to.</typeparam>
        /// <param name="obj">Data to set the current child to.</param>
        /// <returns></returns>
        public async Task SetValueAsync<T>(T obj) {
            await CurrentQuery.PutAsync(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// UPDATES the current child's value to a value: Doesn't delete anything not found in the new value. Classes can be passed.
        /// </summary>
        /// <typeparam name="T">Class type to set the data to.</typeparam>
        /// <param name="obj">Data to set the current child to.</param>
        /// <returns></returns>
        public async Task UpdateValueAsync<T>(T obj) {
            await CurrentQuery.PatchAsync(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Sets the current child's value to a value, where its parent is a random key. Classes can be passed.
        /// </summary>
        /// <typeparam name="T">Class type to set the data to.</typeparam>
        /// <param name="obj">Data to set the current child to.</param>
        /// <returns></returns>
        public async Task PushValueAsync<T>(T obj) {
            await CurrentQuery.PostAsync(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Deletes the data at the current child.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteValueAsync() {
            await CurrentQuery.DeleteAsync();
        }

    }

}
