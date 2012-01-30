using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Bubbles
	{
		public Vector2 motion;
		public Rectangle hitBox;
		Bubble[] bubbles;
		Random r;
		private class Bubble
		{
			Vector2 motion;
			public Vector2 position;
			Texture2D texture;

			public Bubble(Vector2 motion, Vector2 position, Texture2D texture)
			{
				this.motion = motion;
				this.position = position;
				this.texture = texture;
			}

			public void Update()
			{
				this.position += motion;
			}

			public void Draw(SpriteBatch sb)
			{
				sb.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 16, 16), Color.Azure);
			}
		}

		public Bubbles(Rectangle hitBox, Vector2 motion, Texture2D texture)
		{
			r = new Random();
			this.hitBox = hitBox;
			this.motion = motion;
			int numOfBubbles = hitBox.Width * hitBox.Height / 1024;
			bubbles = new Bubble[numOfBubbles];
			for (int i = 0; i < bubbles.Length; i++)
				bubbles[i] = new Bubble(motion, new Vector2(r.Next(hitBox.X, hitBox.X + hitBox.Width - 8), r.Next(hitBox.Y, hitBox.Y + hitBox.Height - 8)), texture);
		}

		public void Update()
		{
			foreach (Bubble b in bubbles)
			{
				b.Update();
				if (r.Next(4) == 1)
					if (motion.Y < 0 && b.position.Y < hitBox.Y)
						b.position.Y = hitBox.Bottom - 8;
					else if (motion.Y > 0 && b.position.Y > hitBox.Bottom - 8)
						b.position.Y = hitBox.Y;
					else if (motion.X < 0 && b.position.X < hitBox.X)
						b.position.X = hitBox.Right - 8;
					else if (motion.X > 0 && b.position.X > hitBox.Right - 8)
						b.position.X = hitBox.X;
			}
		}

		public void Draw(SpriteBatch sb)
		{
			foreach (Bubble b in bubbles)
				b.Draw(sb);
		}
	}
}
