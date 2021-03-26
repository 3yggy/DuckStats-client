using DuckGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using HarmonyLib;
using System.Reflection;
using System.Net;
using System.Net.Sockets;

namespace DuckGame.DS
{
    public static class DuckStats
    {

        static UIMenu StatBrowser;
        static UIMenu HisShitViewer;
        public static bool browserOpen;
        public static bool viewerOpen;

        public static Dictionary<Profile, bool> Updated = new Dictionary<Profile, bool>();
        public static void Init()
        {
            
        }
        public static void OpenStatsBrowser()
        {
            MenuClose(null, true);
            browserOpen = true;
            StatBrowser = new UIMenu("stats", Layer.HUD.camera.width /2f, Layer.HUD.camera.height /2f, 160f, -1f, "", null, false);

            Updated.Clear();

           // int l=0;
            foreach (Profile p in Profiles.active)
            {
              //  if (p.name.Length > l)
                 //   l = p.name.Length;
                Updated.Add(p, p.localPlayer);
                StatBrowser.Add(new UIMenuItem(p.name,new UIMenuActionCallFunction(delegate ()
                {
                    Fucker = p;
                    OpenStatsViewer();
                }),UIAlign.Top,p.persona.colorUsable),true);

                
                Send.Message(new NMSendStatsBB(), NetMessagePriority.ReliableOrdered, p.connection);
            }
            StatBrowser.Resize();

            StatBrowser.Add(new UIMenuItem("Close", new UIMenuActionCallFunction(delegate ()
            {
                MenuClose(null, true);
                Updater.open = false;
            }),default,Color.Red,true));

            MenuOpen(StatBrowser);
        }

        public static Profile Fucker;

        const float statScale =0.4f; 
        static void OpenStatsViewer(int indx = 0)
        {
            MenuClose(null, true);
            HisShitViewer = new UIMenu(Fucker.name+ (Updated[Fucker] ? "@PLUG@" : " @UNPLUG@|RED| OUTDATED!"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f);
            //   HisShitViewer.Add(new UIMenuItem("Jumps: " + Fucker.stats.timesJumped, Fucker.persona.colorUsable,UIAlign.Top));

         //   HisShitViewer.scale = new Vec2(0.5f, 0.5f) ;

            foreach (PropertyInfo property in typeof(ProfileStats).GetProperties())
            {
                object o = property.GetValue(Fucker.stats);
                string val = o.ToString();
                //  if (o is float || o is double || o is int)
                //    val = val.Substring(0, (int)Math.Round((float)o) + 2);
                Vec3 c = Fucker.persona.color;
             /*   float r = (c.x+35)%255;
                float g = (c.y + 35) % 255;
                float b = (c.z + 35) % 255;*/
             //   DevConsole.Log(r + " "+g + " " + b, cc);
                UIText uitxt = new UIText(property.Name + "|BLUE|: "+ (c == Persona.Duck1.color ? "|WHITE|" : (c == Persona.Duck2.color ? "|LIGHTGRAY|" : (c == Persona.Duck3.color ? "|YELLOW|" : "|ORANGE|"))) + val, new Color(0.002f* c.x, 0.002f * c.y+0.2f, 0.002f * c.z) , UIAlign.Top,-0.5f);
       
                 uitxt.scale = new Vec2(statScale, statScale);
                uitxt.collisionSize *= statScale;        HisShitViewer.Add(uitxt);
            }



            HisShitViewer.Add(new UIMenuItem("Back", new UIMenuActionCallFunction(delegate ()
            {
                MenuClose(HisShitViewer);
                MenuOpen(StatBrowser);
            }), default, Color.Red, true));

            HisShitViewer.Resize();

            MenuOpen(HisShitViewer);
        }

        public static void MenuOpen(UIMenu menu)
        {
            menu.Open();
            MonoMain.pauseMenu = menu;
            Level.Add(menu);
        }
        public static void MenuClose(UIMenu menu = null,bool all = false)
        {
            if (all)
            {
                if (StatBrowser != null)
                    MenuClose(StatBrowser);

                if (HisShitViewer != null)
                    MenuClose(HisShitViewer);
            }
            else
            {
                StatBrowser.Close();
                Level.Remove(menu);
            }
        }

        internal static void OpenStatsViewer()
        {
            throw new NotImplementedException();
        }





    }

    public static class StatsNet
    {
        public static void BegStats()
        {
        

        }

        
    }
    public class NMSendStatsBB : NMEvent
    {
        public NMSendStatsBB()
        {
            Him = DuckNetwork.localConnection;
            DevConsole.Log("asking bb for stats", Color.Honeydew);
        }
        public override void Activate()
        {
            base.Activate();
            Send.Message(new NMBBHereStats(), Him);
            DevConsole.Log("returning stats to" + Him.name, Color.Pink);
        }

        NetworkConnection Him;
    }
    public class NMBBHereStats : NMEvent
    {
        public NMBBHereStats()
        {
            Profile P = Profiles.active.Where(p => p.localPlayer).FirstOrDefault();
            HereBabe = P.stats;
            Him = P.networkIndex;
        }

        public override void Activate()
        {
            base.Activate();
            Profile p = Profiles.all.ToArray()[Him];
            DuckStats.Updated[p] = true;
            p.stats = HereBabe;
            if (p == DuckStats.Fucker)
            {
                DuckStats.OpenStatsViewer();
            }
            DevConsole.Log(p.name + "'s jump is at:" + HereBabe.timesJumped, p.persona.colorUsable);
        }
        ProfileStats HereBabe;
        byte Him;
    }
}
