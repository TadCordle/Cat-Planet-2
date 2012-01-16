using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class ElectricFence : Obstacle
	{
		Vector2[] points;
		bool horizontal;
		Random generator;
		Texture2D texture;
		Color color;

		public int index;

		public ElectricFence(Rectangle hitBox, Texture2D texture, Color color, int index) : base(hitBox)
		{
			if (hitBox.Width > hitBox.Height)
			{
				points = new Vector2[hitBox.Width / 32];
				horizontal = true;
			}
			else
			{
				points = new Vector2[hitBox.Height / 32];
				horizontal = false;
			}
			generator = new Random();
			this.index = index;
			this.texture = texture;
			this.color = color;
		}

		public override void Update()
		{
			int x = hitBox.X + 16;
			int y = hitBox.Y + 16;

			if (isDeadly)
			{
				points[0] = new Vector2(x, y);
				for (int i = 1; i < points.Length - 1; i++)
				{
					if (horizontal)
						x += 32;
					else
						y += 32;

					points[i] = new Vector2(x - 8 + generator.Next(16), y - 8 + generator.Next(16));
				}
				points[points.Length - 1] = new Vector2(hitBox.X + hitBox.Width - 16, hitBox.Y + hitBox.Height - 16);
			}
		}

		public override void Draw(SpriteBatch sb)
		{
			if (isDeadly)
				Shapes.DrawLines(sb, points, texture, color);
			sb.Draw(texture, new Rectangle(hitBox.X, hitBox.Y, 32, 32), isDeadly ? color : Color.Black);
			sb.Draw(texture, new Rectangle(hitBox.X + hitBox.Width - 32, hitBox.Y + hitBox.Height - 32, 32, 32), isDeadly ? color : Color.Black);
		}
	}
}
