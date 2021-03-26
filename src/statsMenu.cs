using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;

namespace DuckGame.stats
{
    public static class StatsMenu
    {
        const float statScale = 0.36f;

        static UIMenu statsMenu;
        public static async void OpenStatsViewer(Profile p,UIMenu closeThereFrom)
        {
            try
            {
                string s = await statsWork.client.GetStringAsync(statsWork.wepAppAdress + "stats\\" + p.steamID);

                if (s == "" || s[0] == '8')
                {
                    DuckStatsNotification.ShowNotification("This Duck Has Not Uploaded His Stats");
                }
                else
                {
                    Dictionary<string, string> stats = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);

                    statsMenu = new UIMenu("@PLANET@" + p.name + "'s Stats@PLANET@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, -1, -1);

                    foreach (KeyValuePair<string, string> kv in stats)
                    {
                        UIText i = new UIText(kv.Key + " |GRAY|: |DGBLUE|" + kv.Value, Color.Yellow);
                        i.scale = new Vec2(statScale, statScale);
                        i.collisionSize *= statScale;
                        statsMenu.Add(i);
                    }

                    statsMenu.SetBackFunction(new UIMenuActionCallFunction(delegate ()
                    {
                        CloseStatsViewer(p);
                    }));

                    closeThereFrom?.Close();

                    statsMenu.Resize();
                    MonoMain.pauseMenu = statsMenu;
                    statsMenu.Open();
                    Level.Add(statsMenu);
                }
            }
            catch
            {
                DuckStatsNotification.ShowNotification("@UNPLUG@ERROR viewing stats! Better Tell Ziggy!",Color.Red);
                CloseStatsViewer(p);
                return;
            }
        }

        public static void CloseStatsViewer(Profile p)
        {
            if (statsMenu != null && statsMenu.open)
            {
                statsMenu.Close();
                Level.Remove(statsMenu);
                MonoMain.pauseMenu = null;
            }
            typeof(DuckNetwork).GetMethod("OpenMenu", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { p });            
        }       
    }
    
    public class DuckStatsNotification :Thing
    {
        string msg;
        Color c; Color co;
        public DuckStatsNotification(string msg,Color c,Color co) { 
            this.msg = msg;this.c = c; this.co = co;
            layer = Layer.HUD;
            alpha = 1.4f;
        }
        public override void Draw()
        {
            Graphics.DrawStringOutline(msg, new Vec2(4),c, co, 0.5f, null,1);
            

            if (alpha > 0)
                alpha -= 0.01f;
            else
            {
                c.a-=2;
                co.a = c.a;
                if (c.a < 2)
                    Level.Remove(this);
            }  
        }

        public static void ShowNotification(string message,Color c=default,Color cc=default)
        {
            DevConsole.Log(message, c);
            if (duckStatsNotification != null)
                Level.Remove(duckStatsNotification);
            duckStatsNotification = new DuckStatsNotification(message,c,cc== default?Color.Black:cc);
            Level.Add(duckStatsNotification);
        }
        static DuckStatsNotification duckStatsNotification;
    }

    [HarmonyPatch(typeof(UIConnectionInfo), "Activate", typeof(string))]
    internal class UICActivate
    {
        public static FieldInfo _profile = typeof(UIConnectionInfo).GetField("_profile",BindingFlags.NonPublic|BindingFlags.Instance);
        private  static void Postfix(UIConnectionInfo __instance, string trigger)
        {
            Profile p = (Profile)_profile.GetValue(__instance);
            if (trigger == "STRAFE")//view stats
            {
                StatsMenu.OpenStatsViewer(p, __instance.rootMenu);
            }
            else if (trigger == "MENU2" && p.localPlayer) //auth/ update
            {
                if (Keyboard.Down(Keys.LeftShift))           //reset auth; dont update
                    statsWork.ValidateMeAsync();
                else                                      //auth&auth
                    statsWork.UpdateStatsAsync(p);
            }
            else if (trigger == "RAGDOLL")                //view in browser
            {
                System.Diagnostics.Process.Start(statsWork.wepAppAdress + "/stats?search=" + p.steamID);
            }
        }
    }

    [HarmonyPatch(typeof(UIConnectionInfo), "UpdateName")]
    internal class UICUpdateName
    {
        private static void Postfix(UIConnectionInfo __instance)
        {
            if (((Profile)UICActivate._profile.GetValue(__instance)).localPlayer)
                __instance.controlString += " @MENU2@Upload@SAVEICONTINY@";
            __instance.controlString += " @STRAFE@Stats@RANDOMICON@";
            __instance.controlString += " @RAGDOLL@@PLANET@-@MOON@";
        }
    }
}
