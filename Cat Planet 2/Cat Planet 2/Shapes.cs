using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	public class Shapes
	{
		public static void DrawLine(SpriteBatch sb, Vector2 point1, Vector2 point2, Texture2D pixel, Color c)
		{
			int length = (int)Vector2.Distance(point1, point2);
			Rectangle line = new Rectangle((int)point1.X, (int)point1.Y, length, 1);
			sb.Draw(pixel, line, null, c, (point2.X < point1.X ? (float)Math.PI + (float)Math.Atan((point2.Y - point1.Y) / (point2.X - point1.X)) : (float)Math.Atan((point2.Y - point1.Y) / (point2.X - point1.X))), Vector2.Zero, SpriteEffects.None, 1);
		}

		public static void DrawLines(SpriteBatch sb, Vector2[] points, Texture2D pixel, Color c)
		{
			if (points.Length <= 1)
				throw new FormatException("Cannot draw lines with less than two points.");

			for (int i = 0; i < points.Length - 1; i++)
				DrawLine(sb, points[i], points[i + 1], pixel, c);
		}
	}
}
