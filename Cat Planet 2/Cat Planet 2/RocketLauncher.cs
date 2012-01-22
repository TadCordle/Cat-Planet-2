using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class RocketLauncher : Obstacle
	{
		float angle;
		public Rocket rocket;
		const int pauseTime = 100;
		public int pause;
		Texture2D texture;

		public RocketLauncher(Rectangle hitBox, Texture2D launchTexture, Texture2D rocketTexture)
			: base(hitBox)
		{
			angle = 0;
			rocket = new Rocket(new Rectangle(-1000, -1000, 32, 16), 0, rocketTexture);
			this.texture = launchTexture;
			pause = 0;
		}

		public override void Update()
		{
		}
		public bool Update(Angel angel, List<Wall> walls, List<Link> links)
		{
			Vector2 direction = new Vector2(angel.hitBox.X - hitBox.X, angel.hitBox.Y - hitBox.Y);
			angle = (float)Math.Acos(direction.X / direction.Length());
			if (direction.Y < 0)
				angle *= -1;

			if (pause != pauseTime)
				pause++;
			else
			{
				rocket.position.X = hitBox.X + 8;
				rocket.position.Y = hitBox.Y + 16;
				rocket.angle = angle;
				pause++;
			}
			return rocket.Update(angel, walls, links);
		}

		public override void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, hitBox, null, Color.White, angle, new Vector2(24, 24), SpriteEffects.None, 1);
		}
	}
}
