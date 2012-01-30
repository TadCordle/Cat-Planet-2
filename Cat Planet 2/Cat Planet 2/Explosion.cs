using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Explosion
	{
		Vector2 position;
		Color color;
		List<ExplosionParticle> particles;
		private class ExplosionParticle
		{
			Vector2 position, motion;
			Vector2 gravity = new Vector2(0, 0.3F);
			public int lifeTime, currentTime;

			Texture2D texture;

			public ExplosionParticle(Vector2 position, Vector2 motion, Texture2D texture)
			{
				this.position = position;
				this.motion = motion;
				this.texture = texture;

				lifeTime = new Random().Next(18) + 8;
				currentTime = 0;
			}

			public void Update()
			{
				currentTime++;
				motion += gravity;
				position += motion;
			}

			public void Draw(SpriteBatch sb, Color c)
			{
				sb.Draw(texture, position, c);
			}
		}

		public Explosion(Vector2 position, Color color, Texture2D texture)
		{
			particles = new List<ExplosionParticle>();
			this.position = position;
			this.color = color;
			Random direction = new Random();
			for (int i = 0; i < 50; i++)
				particles.Add(new ExplosionParticle(position, new Vector2((float)direction.NextDouble() * 12 - 6, (float)direction.NextDouble() * 12 - 6), texture));
		}

		public void Update()
		{
			foreach (ExplosionParticle e in particles)
				e.Update();
		}

		public void Draw(SpriteBatch sb)
		{
			foreach (ExplosionParticle e in particles)
				e.Draw(sb, color);
		}
	}
}
