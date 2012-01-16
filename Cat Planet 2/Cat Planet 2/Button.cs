using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Button
	{
		Texture2D texture;
		public Rectangle hitBox;
		Color color;
		public int index;
		public bool activated;

		public Button(Texture2D texture, Rectangle hitBox, Color color, int index)
		{
			this.texture = texture;
			this.hitBox = hitBox;
			this.color = color;
			this.index = index;
			this.activated = false;
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, hitBox, activated ? Color.White : color);
		}
	}
}
