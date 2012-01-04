using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Link
	{
		public Rectangle hitBox;
		public Vector2 levelTo;
		public bool flipx, flipy;

		public Link(Rectangle hitBox, Vector2 levelTo, bool flipx, bool flipy)
		{
			this.hitBox = hitBox;
			this.levelTo = levelTo;
			this.flipx = flipx;
			this.flipy = flipy;
		}
	}
}
