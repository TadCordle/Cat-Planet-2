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
		Texture2D trailTexture;
		List<TrailPiece> trail;
		private class TrailPiece
		{
			int aliveTime;
			Vector2 position;
			Texture2D texture;

			public TrailPiece(int aliveTime, Vector2 position, Texture2D texture)
			{
				this.position = position;
				this.aliveTime = aliveTime;
				this.texture = texture;
			}

			public bool Update()
			{
				aliveTime--;
				if (aliveTime <= 0)
					return true;
				return false;
			}

			public void Draw(SpriteBatch sb)
			{
				sb.Draw(texture, position, Color.Orange);
			}
		}

		public Rocket(Rectangle hitBox, float angle, Texture2D texture, Texture2D trailTexture)
			: base(hitBox)
		{
			this.trailTexture = trailTexture;
			this.position = new Vector2(hitBox.X, hitBox.Y);
			this.texture = texture;
			this.angle = angle;
			motion = new Vector2((float)Math.Sin(angle) * 8, (float)Math.Cos(angle) * 8);
			trail = new List<TrailPiece>();
			for (int i = 0; i < 10; i++)
				trail.Add(new TrailPiece(10 - i, this.position, trailTexture));
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

			for (int i = 0; i < trail.Count; i++)
				if (trail[i].Update())
					trail.RemoveAt(i);
			trail.Insert(0, new TrailPiece(10, position, trailTexture));
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
			foreach (TrailPiece t in trail)
				t.Draw(sb);
			sb.Draw(texture, new Rectangle(hitBox.X + 8, hitBox.Y + 8, texture.Width, texture.Height), null, Color.White, angle, new Vector2(16, 8), SpriteEffects.None, 1);
		}
	}
}
