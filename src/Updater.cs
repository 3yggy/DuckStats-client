using System;
using Microsoft.Xna.Framework;

namespace DuckGame.stats
{
	internal class Updater : IUpdateable
	{
		public bool Enabled { get { return true; } }
		public int UpdateOrder { get { return 1; } }
		public event EventHandler<EventArgs> UpdateOrderChanged;
		public event EventHandler<EventArgs> EnabledChanged;

		public void Update(GameTime gameTime)
		{			
            if (pin && Level.current is TitleScreen && Level.current.initialized)
            {
				pin = false;

				Vec2 place = statsMod.qol ? new Vec2(240f, 122f) : new Vec2(48,46);

				Level.Add(new DuckStatsMonkey(place.x,place.y));
				DuckStatsNotification.ShowNotification("Duck Stats by Ziggy",Color.Gold);
			}
		}
		private static bool pin = true;
	}

	[BaggedProperty("canSpawn", false)]
	public class DuckStatsMonkey : Block, IPlatform
	{
		public DuckStatsMonkey(float xpos, float ypos) : base(xpos, ypos)
		{
			graphic = new Sprite(GetPath("DuckStatsMonkey"));
			graphic.CenterOrigin();
			depth = -0.4f;
			solid = false;
			scale = new Vec2(0.4f);
		}
	}
}
