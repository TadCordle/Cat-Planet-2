using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Cat_Planet_2
{
	class Angel
	{
		AnimatedTexture normalSprite, flySprite;
		SoundEffect hitWall, swish;
		const int normalTime = 15, flyTime = 4;
		bool isNormal;
		bool flipped;

		public Vector2 position, motion;
		const float maxHSpeed = 7.2F, hAcceleration = 0.3F;
		bool highJump, lowJump;
		int keyCount, previous;

		public Rectangle hitBox;

		public Angel(Texture2D[] normalSprite, Texture2D[] jumpSprite, SoundEffect hitWall, SoundEffect swish, Vector2 position)
		{
			this.normalSprite = new AnimatedTexture(normalSprite, normalTime, false);
			this.flySprite = new AnimatedTexture(jumpSprite, flyTime, true);
			this.flipped = false;
			this.isNormal = true;

			this.position = position;
			this.motion = Vector2.Zero;
			this.highJump = this.lowJump = false;
			
			this.hitWall = hitWall;
			this.swish = swish;

			keyCount = 0;
			previous = 0;

			hitBox = new Rectangle((int)position.X + 8, (int)position.Y, 48, 48);
		}

		public int Update(List<Wall> walls)
		{
			int jump;

			if (isNormal)
				normalSprite.Update();
			else
				isNormal = flySprite.Update();
			
			jump = ProcessKeyboard();

			motion.Y += 0.3F;

			position += motion;
			UpdateHitBox();

			#region Wall collision checking
			foreach (Wall w in walls)
			{
				if (this.hitBox.Intersects(w.hitBox) && w.hitable)
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
						motion.Y -= motion.Y / 3F;
						if (Math.Abs(motion.Y) > 1)
							hitWall.Play(0.2F, 0.0F, 0.0F);
					}

					if (this.hitBox.Top < w.hitBox.Bottom && this.hitBox.Bottom > w.hitBox.Top)
					{
						if (this.hitBox.Right <= w.hitBox.Left)
							this.position.X = w.hitBox.X - 57;
						else
							this.position.X = w.hitBox.Right - 7;
						motion.X *= -1;
						motion.X -= motion.X / 3F;
						if (Math.Abs(motion.X) > 1)
							hitWall.Play(0.2F, 0.0F, 0.0F);
					}
				}
			}
			#endregion
			
			return jump;
		}

		private int ProcessKeyboard()
		{
			#region Check Movement Keys
			highJump = lowJump = false;
			int keysPressed = 0;
			KeyboardState keys = Keyboard.GetState();
			if (keys.IsKeyDown(Keys.Left))
			{
				motion.X -= hAcceleration;
				flipped = true;
				keysPressed++;
			}
			if (keys.IsKeyDown(Keys.Right))
			{
				motion.X += hAcceleration;
				flipped = false;
				keysPressed++;
			}

			if (motion.X > maxHSpeed)
				motion.X = maxHSpeed;
			else if (motion.X < -1 * maxHSpeed)
				motion.X = -1 * maxHSpeed;

			if (keys.IsKeyDown(Keys.Up))
			{
				keysPressed++;
				highJump = true;
			}

			if (keys.IsKeyDown(Keys.Down))
			{
				keysPressed++;
				lowJump = true;
			}
			#endregion

			#region Check For Jumping

			if (keys.GetPressedKeys().Count() > keysPressed)
			{
				previous = keyCount;
				keyCount = keys.GetPressedKeys().Count() - keysPressed;
				if (keyCount > previous)
				{
					Jump();
					return 1;
				}
			}
			else
				keyCount = 0;

			return 0;
			#endregion
		}

		private void Jump()
		{
			isNormal = false;
			swish.Play(0.7F, 0.0F, 0.0F);
			if (highJump && !lowJump)
				motion.Y = -10F;
			else if (lowJump && !highJump)
				motion.Y = -3F;
			else
				motion.Y = -5.5F;
		}
		
		public void UpdateHitBox()
		{
			hitBox.X = (int)position.X + 8;
			hitBox.Y = (int)position.Y;
		}

		public void Draw(SpriteBatch sb)
		{
			if (isNormal)
				normalSprite.Draw(sb, position, Color.White, 0, Vector2.Zero, Vector2.One, (flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1);
			else
				flySprite.Draw(sb, position, Color.White, 0, Vector2.Zero, Vector2.One, (flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1);
		}
	}
}
