using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class SpinningPlasma
	{
		public Rectangle hitBox;
		public bool isDeadly;
		bool counterClockwise;
		float radius;
		float time;
		AnimatedTexture texture;
		Vector2 position;

		public SpinningPlasma(Rectangle hitBox, float radius, float startTime, bool counterCLockwise, AnimatedTexture texture)
		{
			isDeadly = true;
			this.hitBox = hitBox;
			time = startTime;
			radius *= 3.0f / 54.0f;
			this.radius = radius;
			this.texture = texture;
			this.counterClockwise = counterCLockwise;
			position = new Vector2(hitBox.X, hitBox.Y);
		}

		public void Update()
		{
			time += 0.06f;
			position.X += (float)(radius * Math.Cos(time)) * (counterClockwise ? -1 : 1);
			position.Y += (float)(radius * Math.Sin(time));
			hitBox.X = (int)position.X;
			hitBox.Y = (int)position.Y;

			texture.Update();
		}

		public void Draw(SpriteBatch sb)
		{
			texture.Draw(sb, position, Color.White);
		}
	}
}
