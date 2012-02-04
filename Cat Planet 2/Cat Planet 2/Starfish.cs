using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Starfish
	{
		public Rectangle hitBox;
		Vector2 position;
		Vector2 motion;
		float angle;
		Texture2D texture;

		public Starfish(Rectangle hitBox, Vector2 motion, Texture2D texture)
		{
			this.hitBox = hitBox;
			this.position.X = hitBox.X;
			this.position.Y = hitBox.Y;
			this.motion = motion;
			this.texture = texture;
			angle = 0.0f;
		}

		public void Update(List<Wall> walls, List<Link> links)
		{
			position += motion;
			UpdateHitBox();
			foreach (Wall w in walls)
			{
				if (w.hitBox.Intersects(this.hitBox))
				{
					while (w.hitBox.Intersects(this.hitBox))
					{
						position -= Vector2.Normalize(motion);
						UpdateHitBox();
					}
					if (this.hitBox.Left < w.hitBox.Right && this.hitBox.Right > w.hitBox.Left)
					{
						if (this.hitBox.Bottom <= w.hitBox.Top)
							this.position.Y = w.hitBox.Y - 64;
						else
							this.position.Y = w.hitBox.Bottom;
						motion.Y *= -1;
					}

					if (this.hitBox.Top < w.hitBox.Bottom && this.hitBox.Bottom > w.hitBox.Top)
					{
						if (this.hitBox.Right <= w.hitBox.Left)
							this.position.X = w.hitBox.X - 64;
						else
							this.position.X = w.hitBox.Right;
						motion.X *= -1;
					}
				}
			}
			foreach (Link w in links)
			{
				if (w.hitBox.Intersects(this.hitBox))
				{
					while (w.hitBox.Intersects(this.hitBox))
					{
						position -= Vector2.Normalize(motion);
						UpdateHitBox();
					}
					if (this.hitBox.Left < w.hitBox.Right && this.hitBox.Right > w.hitBox.Left)
					{
						if (this.hitBox.Bottom <= w.hitBox.Top)
							this.position.Y = w.hitBox.Y - 64;
						else
							this.position.Y = w.hitBox.Bottom;
						motion.Y *= -1;
					}

					if (this.hitBox.Top < w.hitBox.Bottom && this.hitBox.Bottom > w.hitBox.Top)
					{
						if (this.hitBox.Right <= w.hitBox.Left)
							this.position.X = w.hitBox.X - 64;
						else
							this.position.X = w.hitBox.Right;
						motion.X *= -1;
					}
				}
			}
			angle += 0.05f;
		}
		private void UpdateHitBox()
		{
			this.hitBox.X = (int)position.X;
			this.hitBox.Y = (int)position.Y;
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, new Rectangle(hitBox.X + 32, hitBox.Y + 32, 64, 64), null, Color.White, angle, new Vector2(32, 32), SpriteEffects.None, 1);
		}
	}
}
