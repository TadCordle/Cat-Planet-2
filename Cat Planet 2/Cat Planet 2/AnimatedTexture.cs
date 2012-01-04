using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class AnimatedTexture
	{
		Texture2D[] images;
		bool playOnce;
		int timePerFrame, currentTime, frame;

		public AnimatedTexture(Texture2D[] images, int timePerFrame, bool playOnce)
		{
			this.images = images;
			this.timePerFrame = timePerFrame;
			this.playOnce = playOnce;
			currentTime = 0;
			frame = 0;
		}

		// Returns whether or not the animation is finished
		public bool Update()
		{
			currentTime++;
			if (currentTime > timePerFrame)
			{
				currentTime = 0;
				frame++;
				if (frame >= images.Length)
					frame = 0;
				return playOnce;
			}
			return false;
		}

		public void Draw(SpriteBatch sb, Vector2 position, Color c)
		{
			sb.Draw(images[frame], position, c);
		}

		public void Draw(SpriteBatch sb, Vector2 position, Color c, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			sb.Draw(images[frame], position, null, c, rotation, origin, scale, effects, layerDepth);
		}
	}
}
