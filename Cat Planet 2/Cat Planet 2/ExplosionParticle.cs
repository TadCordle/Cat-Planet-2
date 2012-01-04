using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class ExplosionParticle
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

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, position, Color.White);
		}
	}
}
