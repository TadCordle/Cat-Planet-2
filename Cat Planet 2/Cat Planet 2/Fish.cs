using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Fish
	{
		public Rectangle hitBox;
		float angle;
		AnimatedTexture texture;
		public Vector2 motion;
		public Vector2 position;
		public Vector2 startPosition;

		public Fish(Vector2 position, AnimatedTexture texture)
		{
			this.angle = 0;
			this.texture = texture;
			this.position = position;
			startPosition = position;
			this.motion = Vector2.Zero;
			hitBox = new Rectangle((int)position.X, (int)position.Y, 48, 48);
		}

		public void Update(Angel angel, List<Wall> walls, List<Fish> otherfish)
		{
			Vector2 direction = new Vector2(angel.hitBox.X - hitBox.X, angel.hitBox.Y - hitBox.Y);
			angle = (float)Math.Acos(direction.X / direction.Length());
			if (direction.Y < 0)
				angle *= -1;

			if (angel.position.X > this.position.X && this.motion.X < 7)
				this.motion.X += 0.25f;
			else if (angel.position.X < this.position.X && this.motion.X > -7)
				this.motion.X -= 0.25f;
			if (angel.position.Y > this.position.Y && this.motion.Y < 7)
				this.motion.Y += 0.25f;
			else if (angel.position.Y < this.position.Y && this.motion.Y > -7)
				this.motion.Y -= 0.25f;
			
			position += motion;
			UpdateHitBox();

			texture.Update();

			foreach (Wall w in walls)
			{
				if (this.hitBox.Intersects(w.hitBox))
				{
					Vector2 unit = motion;
					unit.Normalize();
					while (this.hitBox.Intersects(w.hitBox))
					{
						this.position -= unit;
						UpdateHitBox();
					}

					if (this.hitBox.Left < w.hitBox.Right && this.hitBox.Right > w.hitBox.Left)
					{
						if (this.hitBox.Bottom <= w.hitBox.Top)
							this.position.Y = w.hitBox.Y - 48;
						else
							this.position.Y = w.hitBox.Bottom;
						motion.Y *= -1;
					}

					if (this.hitBox.Top < w.hitBox.Bottom && this.hitBox.Bottom > w.hitBox.Top)
					{
						if (this.hitBox.Right <= w.hitBox.Left)
							this.position.X = w.hitBox.X - 48;
						else
							this.position.X = w.hitBox.Right;
						motion.X *= -1;
					}
				}
			}
			foreach (Fish w in otherfish)
			{
				if (w == this) continue;
				if (this.hitBox.Intersects(w.hitBox))
				{
					Vector2 unit = motion;
					unit.Normalize();
					while (this.hitBox.Intersects(w.hitBox))
					{
						this.position -= unit;
						UpdateHitBox();
					}

					if (this.hitBox.Left < w.hitBox.Right && this.hitBox.Right > w.hitBox.Left)
					{
						if (this.hitBox.Bottom <= w.hitBox.Top)
							this.position.Y = w.hitBox.Y - 48;
						else
							this.position.Y = w.hitBox.Bottom;
						motion.Y *= -1;
					}

					if (this.hitBox.Top < w.hitBox.Bottom && this.hitBox.Bottom > w.hitBox.Top)
					{
						if (this.hitBox.Right <= w.hitBox.Left)
							this.position.X = w.hitBox.X - 48;
						else
							this.position.X = w.hitBox.Right;
						motion.X *= -1;
					}
				}
			}
		}

		private void UpdateHitBox()
		{
			this.hitBox.X = (int)position.X;
			this.hitBox.Y = (int)position.Y;
		}

		public void Draw(SpriteBatch sb)
		{
			texture.Draw(sb, new Vector2(position.X + 24, position.Y + 24), Color.White, angle, new Vector2(24, 24), Vector2.One, Math.Abs(angle) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically : SpriteEffects.None, 1);
		}
	}
}
