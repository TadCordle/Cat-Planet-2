using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Gem
	{
		public Vector2 position;
		public Rectangle hitBox;
		public bool taken;

		Texture2D texture;
		Color c;

		public Gem(Vector2 position, Texture2D texture, Color c)
		{
			this.position = position;
			this.texture = texture;
			this.c = c;

			hitBox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
			taken = false;
		}

		public void Update()
		{
			if (!taken)
			{
				this.hitBox.X = (int)position.X;
				this.hitBox.Y = (int)position.Y;
			}
			else
			{
				this.hitBox.X = -1000;
				this.hitBox.Y = -1000;
			}
		}

		public void Draw(SpriteBatch sb)
		{
			if (!taken)
				sb.Draw(texture, hitBox, c);
		}
	}
}
