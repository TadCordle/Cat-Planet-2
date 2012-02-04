using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Timer
	{
		Texture2D front, back;

		public Rectangle hitBox;
		public int index;
		public bool activated;
		public bool loop;

		float angle, angleSpeed;
		int maxTime;

		public Timer(Texture2D front, Texture2D back, Rectangle hitBox, int index, int maxTime, bool loop)
		{
			this.loop = loop;
			this.front = front;
			this.back = back;
			this.hitBox = hitBox;
			this.index = index;
			this.activated = false;
			this.maxTime = maxTime;

			angle = 0;
			angleSpeed = MathHelper.TwoPi / maxTime;
		}

		public void Update(Level currentLevel)
		{
			if (activated || loop)
			{
				angle += angleSpeed;
				if (angle > MathHelper.TwoPi)
					angle -= MathHelper.TwoPi;

				if (angle >= 0 && angle - angleSpeed < 0)
				{
					if (!loop)
					{
						activated = false;
						angle = 0;
						currentLevel.buttons[index].activated = false;
						currentLevel.fences[index].isDeadly = true;
					}
					else
					{
						activated = !activated;
						currentLevel.fences[index].isDeadly = !currentLevel.fences[index].isDeadly;
					}
				}
			}
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(back, hitBox, Color.White);
			sb.Draw(front, new Rectangle(hitBox.X + 24, hitBox.Y + 24, front.Width, front.Height), null, Color.White, angle, new Vector2(front.Width / 2, front.Height / 2), SpriteEffects.None, 1);
		}
	}
}
