using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Cat_Planet_2
{
	class ObstacleWall : Wall
	{
		public ObstacleWall(Rectangle hitBox)
			: base(hitBox)
		{
			this.hitable = false;
		}
	}
}
