using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class DeathWall
	{
		public bool isDeadly;
		public Rectangle hitBox;

		public DeathWall(Rectangle hitBox)
		{
			isDeadly = true;
			this.hitBox = hitBox;
		}
	}
}
