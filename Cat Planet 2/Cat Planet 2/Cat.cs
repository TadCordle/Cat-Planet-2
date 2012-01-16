using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Cat
	{
		int index;
		string message;
		int newLines, longestLine;

		public bool hit;
		public Vector2 position;

		AnimatedTexture normalTexture, hitTexture;
		SpriteFont font;

		public Rectangle hitBox;

		public Cat(Vector2 position, int index, string message, AnimatedTexture normalTexture, AnimatedTexture hitTexture, SpriteFont font)
		{
			this.position = position;
			this.index = index;
			this.message = message;
			this.normalTexture = normalTexture;
			this.hitTexture = hitTexture;
			this.font = font;
			hit = false;
			hitBox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
			ProcessText();
		}

		private void ProcessText()
		{
			newLines = 1;
			longestLine = 0;
			int longestTemp = 0;
			if (message != null)
			{
				int i = 0;
				while (i < message.Length)
				{
					if (message[i] == '\n')
					{
						newLines++;
						if (longestTemp > longestLine)
							longestLine = longestTemp;
						longestTemp = 0;
					}
					else
						longestTemp++;
					i++;
				}
				if (newLines == 1)
					longestLine = message.Length;
			}
		}

		public void Update()
		{
			if (hit)
				hitTexture.Update();
			else
				normalTexture.Update();
		}
		public void UpdateHitBox()
		{
			hitBox.X = (int)position.X - 16;
			hitBox.Y = (int)position.Y - 16;
		}

		public void Draw(SpriteBatch sb)
		{
			if (!hit)
				normalTexture.Draw(sb, position, Color.White);
			else
			{
				hitTexture.Draw(sb, position, Color.White);
				sb.DrawString(font, message, new Vector2(position.X - longestLine * 4.9F, position.Y - 26 * newLines), Color.White);
			}
		}
	}
}
