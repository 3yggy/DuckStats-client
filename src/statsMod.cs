using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;

[assembly: AssemblyTitle("Stats")]

[assembly: AssemblyCompany("Ziggy")]

[assembly: AssemblyDescription("Share Duck Game Stats")]

[assembly: AssemblyVersion("3.1.9.7")]

namespace DuckGame.stats
{
    public class statsMod : DisabledMod
    {
		public static bool qol;
		protected override void OnPreInitialize()
		{
			base.OnPreInitialize();
			
			AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
			{
				if (args.Name.Contains("0Harmony, Version="))
				{
					Assembly A = null;
					foreach (Assembly a in (typeof(ModLoader).GetField("_modAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Dictionary<Assembly, Mod>).Keys)
					{
						if (a.GetName().Name == @"0Harmony")
						{
							A = a;
							break;
						}
					}
					if (A == null)
					{
						A = Assembly.LoadFrom(this.configuration.directory + @"\0Harmony.dll");
						(typeof(ModLoader).GetField("_modAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Dictionary<Assembly, Mod>).Add(A, new DisabledMod());
						DevConsole.Log("ziggy puedes provido el harmony lib!", Color.Green);
					}
					if (A != null)
					{
						return A;
					}
				}
				else if (args.Name.Contains("Newtonsoft.Json, Version="))
				{
					Assembly A = Assembly.LoadFrom(this.configuration.directory + @"\Newtonsoft.Json.dll");
					if (A != null)
					{
						DevConsole.Log("yo resolvo un newtonsoft", Color.Green);
						return A;
					}
				}
				DevConsole.Log(args.Name, Color.Brown);
				return null;
			};
		}
		protected override void OnPostInitialize()
		{

			base.OnPostInitialize();
			(typeof(Game).GetField("updateableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IUpdateable>).Add(new Updater());
			
			Harmony h = new Harmony("cancione de los dioeses");
			h.PatchAll();

			statsWork.Init();

			foreach (Assembly a in (typeof(ModLoader).GetField("_modAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Dictionary<Assembly, Mod>).Keys)
			{
				if (a.GetName().Name == @"QOL_compiled")
				{
					qol = true;
					break;
				}
			}
		}
	}
}
