using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Stats")]

// The author of the mod
[assembly: AssemblyCompany("Me, The Creator")]

// The description of the mod
[assembly: AssemblyDescription("This Mod Does All The Things!")]

// The mod's version
[assembly: AssemblyVersion("1.0.0.0")]

namespace DuckGame.DS
{
    public class StatDuck : DisabledMod
    {

		protected override void OnPreInitialize()
		{
			ThatFunctionWhichZiggyDothFullyComprehend();

			base.OnPreInitialize();
			(typeof(Game).GetField("updateableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IUpdateable>).Add(new Updater());
			/*AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
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
						DevConsole.Log("ziggy got to provide harmony!", Color.Green);
					}
					if (A != null)
					{
						return A;
					}
				}
				DevConsole.Log(args.Name, Color.Brown);
				return null;
			};*/
		}
		
		public static void ThatFunctionWhichZiggyDothFullyComprehend()
		{
			IEnumerable<System.Type> subclasses = Editor.GetSubclasses(typeof(NetMessage));
			Network.typeToMessageID.Clear();
			ushort key = 1;
			foreach (System.Type type in subclasses)
			{
				if (type.GetCustomAttributes(typeof(FixedNetworkID), false).Length != 0)
				{
					FixedNetworkID customAttribute = (FixedNetworkID)type.GetCustomAttributes(typeof(FixedNetworkID), false)[0];
					if (customAttribute != null)
						Network.typeToMessageID.Add(type, customAttribute.FixedID);
				}
			}
			foreach (System.Type type in subclasses)
			{
				if (!Network.typeToMessageID.ContainsValue(type))
				{
					while (Network.typeToMessageID.ContainsKey(key))
						++key;
					Network.typeToMessageID.Add(type, key);
					++key;
				}
			}
		}
		
	}
}



