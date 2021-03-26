using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace DuckGame.stats
{
    public static class statsWork
    {
        public static readonly HttpClient client = new HttpClient();

        public const string wepAppAdress = 1==1?"http://duck-stats.herokuapp.com/": "http://127.0.0.1:5000/";

        private static string authPath;

        private static string pin = Guid.NewGuid().ToString("n").Substring(0, 8).ToLower();

        public static async void Init()
        {
            authPath = DuckFile.profileDirectory + "\\ziggyStats.auth";
            

            if (!File.Exists(authPath))
                ValidateMeAsync();

        }
        public static async Task UpdateStatsAsync(Profile p)
        {
            if (File.Exists(authPath))
            {
                try
                {
                    JObject jo = JObject.FromObject(p.stats);
                    jo.Remove("currentTitle");
                    var response = await client.PostAsync(wepAppAdress + "upload\\" + Steam.user.id, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "json", jo.ToString()},
                    { "auth", File.ReadAllText(authPath)}
                }));

                    string resp = await response.Content.ReadAsStringAsync();

                    switch (resp[0])
                    {
                        case '1':
                            DuckStatsNotification.ShowNotification("@CHECK@Stats updated.", Color.Green);
                            SFX.Play("Ding", 0.6f);
                            break;
                        case '0':
                            DuckStatsNotification.ShowNotification("@ONLINEBAD@Invalid Authentification!", Color.Red);
                            ValidateMeAsync(p);
                            break;
                        case '2':
                            DuckStatsNotification.ShowNotification("Invalid Stats!",Color.Red);
                            break;
                        case '3':
                            DuckStatsNotification.ShowNotification("@TICKET@Unkown Duck; Authenticating...",Color.Orange);
                            ValidateMeAsync(p);
                            break;
                    }
                }
                catch
                {
                    DuckStatsNotification.ShowNotification("ERROR Connecting With Server! Better Tell Ziggy!",Color.Red);
                }
            }
            else
            {
                ValidateMeAsync(p);
            }
        }

        static private bool validating;
        public static async Task ValidateMeAsync(Profile p = null)
        {
            if (!validating)
            {
                validating = true;
                System.Diagnostics.Process.Start(wepAppAdress + "auth\\" + pin);

                MessageBox.Show("Authenticating... Click Ok when Done.");
                validating = false;
                string auth = await client.GetStringAsync(wepAppAdress + "pickup\\" + pin);
                if (auth != "" && auth[0]!='6')
                {
                    DuckStatsNotification.ShowNotification("@TINYLOCK@Local auth set to " + auth,Color.Gray,Color.White);
                    File.WriteAllText(authPath, auth);
                    if (p != null)
                    {
                        UpdateStatsAsync(p);                     
                    }
                }
                else
                {
                    if (MessageBox.Show("Validate with Steam Before Returning!\n\nTry Again?", "Authentification Failed!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        ValidateMeAsync();
                    else
                        DuckStatsNotification.ShowNotification("Auth not established", Color.Orange);
                }
            }
        }
    }
}
