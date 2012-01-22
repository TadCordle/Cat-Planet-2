using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Rocket : Obstacle
	{
		public Vector2 position;
		Vector2 motion;
		public float angle;
		Texture2D texture;

		public Rocket(Rectangle hitBox, float angle, Texture2D texture)
			: base(hitBox)
		{
			this.position = new Vector2(hitBox.X, hitBox.Y);
			this.texture = texture;
			this.angle = angle;
			motion = new Vector2((float)Math.Sin(angle) * 8, (float)Math.Cos(angle) * 8);
		}

		public override void Update()
		{
		}

		public bool Update(Angel angel, List<Wall> walls, List<Link> links)
		{
			Vector2 direction = new Vector2(angel.hitBox.X - position.X, angel.hitBox.Y - position.Y);
			float angleTo = (float)Math.Acos(direction.X / direction.Length());
			if (direction.Y < 0)
				angleTo *= -1;

			if (angleTo < 0)
				angleTo += MathHelper.TwoPi;
			else if (angleTo >= MathHelper.TwoPi)
				angleTo -= MathHelper.TwoPi;
			if (angle < 0)
				angle += MathHelper.TwoPi;
			else if (angle >= MathHelper.TwoPi)
				angle -= MathHelper.TwoPi;

			if (angleTo > angle)
				if (angleTo - angle < MathHelper.Pi)
					angle += 0.04f;
				else
					angle -= 0.04f;
			else
				if (angle - angleTo > MathHelper.Pi)
					angle += 0.04f;
				else
					angle -= 0.04f;

			motion = new Vector2((float)Math.Cos(angle) * 8, (float)Math.Sin(angle) * 8);
			position += motion;
			hitBox.X = (int)position.X;
			hitBox.Y = (int)position.Y;

			if (hitBox.Intersects(angel.hitBox))
				return true;
			foreach (Wall w in walls)
				if (hitBox.Intersects(w.hitBox))
					return true;
			foreach (Link w in links)
				if (hitBox.Intersects(w.hitBox))
					return true;

			return false;
		}

		public override void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, hitBox, null, Color.White, angle, new Vector2(16, 8), SpriteEffects.None, 1);
		}
	}
}
