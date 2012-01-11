using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_Planet_2
{
	class Level
	{
		public Vector2 coordinates;
		public Type type;
		public List<Wall> walls;
		public List<Cat> cats;
		public Gem gem;
		public List<Link> links;
		public List<Obstacle> obstacles;
		public Dictionary<Vector2, Vector2> restartPosition;
		Texture2D back, fore;

		public Level(StreamReader sr, Vector2 coordinates, Type type, Texture2D back, Texture2D fore, Cat[] cArray, Gem[] gArray, Dictionary<string, Texture2D[]> obTextures)
		{
			this.coordinates = coordinates;
			string coordString = ((int)coordinates.X).ToString() + " " + ((int)(coordinates.Y)).ToString();
			walls = new List<Wall>();
			cats = new List<Cat>();
			links = new List<Link>();
			obstacles = new List<Obstacle>();

			string line = "";
			while (line != coordString)
				line = sr.ReadLine();

			line = sr.ReadLine();

			/**************************************************************************************************
			 * Format of text file:
			 * # #				Specifies the coordinates of the board in the world
			 * restart [#ofpositions lfromx lfromy x y lfromx lfromy x y... etc]
			 * wall [x y width heigh]
			 * cat [x y index]
			 * gem [posx posy index]
			 * link [posx posy width height leveltox leveltoy flipx flipy]
			 * deathwall [posx posy width height]
			 * !				Denotes end of board
			 **************************************************************************************************/

			while (!(sr.EndOfStream || line == "!"))
			{
				if (line != null)
				{
					string[] splitLine = line.Split(' ');
					if (splitLine[0] == "restart")
					{
						restartPosition = new Dictionary<Vector2, Vector2>(int.Parse(splitLine[1]));
						for (int i = 2; i < splitLine.Length; i += 4)
							restartPosition[new Vector2(int.Parse(splitLine[i]), int.Parse(splitLine[i + 1]))] = new Vector2(int.Parse(splitLine[i + 2]), int.Parse(splitLine[i + 3]));
					}
					else if (splitLine[0] == "wall")
						walls.Add(new Wall(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4]))));
					else if (splitLine[0] == "cat")
					{
						cArray[int.Parse(splitLine[3])].position = new Vector2(int.Parse(splitLine[1]), int.Parse(splitLine[2]));
						cArray[int.Parse(splitLine[3])].UpdateHitBox();
						this.cats.Add(cArray[int.Parse(splitLine[3])]);
					}
					else if (splitLine[0] == "gem")
					{
						gArray[int.Parse(splitLine[3])].position = new Vector2(int.Parse(splitLine[1]), int.Parse(splitLine[2]));
						gem = gArray[int.Parse(splitLine[3])];
					}
					else if (splitLine[0] == "link")
					{
						links.Add(new Link(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4])),
										   new Vector2(int.Parse(splitLine[5]), int.Parse(splitLine[6])),
										   (int.Parse(splitLine[7]) == 1 ? true : false),
										   (int.Parse(splitLine[8]) == 1 ? true : false)));
					}
					else if (splitLine[0] == "deathwall")
					{
						obstacles.Add(new DeathWall(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4]))));
					}
				}
				line = sr.ReadLine();
			}
			this.back = back;
			this.fore = fore;
			this.type = type;
		}

		public void DrawBG(SpriteBatch sb, int w, int h)
		{
			Rectangle bounds = new Rectangle(0, 0, w, h);
			sb.Draw(back, bounds, Color.White);
		}
		public void DrawFG(SpriteBatch sb, int w, int h)
		{
			Rectangle bounds = new Rectangle(0, 0, w, h);
			sb.Draw(fore, bounds, Color.White);
		}

		public enum Type
		{
			Canyons,
			Caves,
			WarZone,
			FrozenWasteland,
			Underwater,
			UnderwaterTransition,
			FaultLine,
		}
	}
}
