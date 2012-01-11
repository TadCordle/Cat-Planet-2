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
		Texture2D texture;
		List<ExplosionParticle> particles;

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
