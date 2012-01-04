﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	abstract class Obstacle
	{
		public Rectangle hitBox;

		public Obstacle(Rectangle hitBox)
		{
			this.hitBox = hitBox;
		}

		public abstract void Update();
		
		public abstract void Draw(SpriteBatch sb);
	}
}
