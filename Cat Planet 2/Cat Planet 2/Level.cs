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
		public List<DeathWall> deathWalls;
		public List<FallingRock> rocks;
		public List<ElectricFence> fences;
		public List<Button> buttons;
		public List<Timer> timers;
		public List<SpinningPlasma> plasmas;
		public List<RocketLauncher> launchers;
		public List<Bubbles> bubbles;
		public Dictionary<Vector2, Vector2> restartPosition;
		public Dictionary<string, Color> colors;
		public Texture2D back, fore;

		public Level(StreamReader sr, Vector2 coordinates, Type type, Texture2D back, Texture2D fore, Cat[] cArray, Gem[] gArray, Dictionary<string, Texture2D[]> obTextures)
		{
			this.coordinates = coordinates;
			walls = new List<Wall>();
			cats = new List<Cat>();
			links = new List<Link>();
			deathWalls = new List<DeathWall>();
			rocks = new List<FallingRock>();
			fences = new List<ElectricFence>();
			buttons = new List<Button>();
			timers = new List<Timer>();
			plasmas = new List<SpinningPlasma>();
			launchers = new List<RocketLauncher>();
			bubbles = new List<Bubbles>();

			colors = new Dictionary<string, Color>();
			colors.Add("red", Color.Red);
			colors.Add("orange", Color.Orange);
			colors.Add("yellow", Color.Yellow);
			colors.Add("lime", Color.Lime);
			colors.Add("cyan", Color.Cyan);
			colors.Add("purple", Color.Purple);

			string line = "";

			/**************************************************************************************************
			 * Format of text file:
			 * # #				Specifies the coordinates of the board in the world
			 * restart [#ofpositions lfromx lfromy x y lfromx lfromy x y... etc]
			 * wall [posx posy width heigh]
			 * cat [posx posy index]
			 * gem [posx posy index]
			 * link [posx posy width height leveltox leveltoy flipx? flipy?]
			 * deathwall [posx posy width height]
			 * rock [posx posy speedx speedy]
			 * fence [posx posy width height color index]
			 * button [posx posy color index]
			 * timer [posx posy index time loop?] // make radius in multiples of 3
			 * plasma [posx posy radius counterclockwise?]
			 * launcher [posx posy]
			 * !				Denotes end of board
			 **************************************************************************************************/

			while (!sr.EndOfStream)
			{
				line = sr.ReadLine();

				if (line != null)
				{
					string[] splitLine = line.Trim().Split(' ');
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
						deathWalls.Add(new DeathWall(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4]))));
					}
					else if (splitLine[0] == "obstaclewall")
					{
						walls.Add(new ObstacleWall(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4]))));
					}
					else if (splitLine[0] == "fence")
					{
						fences.Insert(int.Parse(splitLine[6]), new ElectricFence(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4])), obTextures["fence"][0], colors[splitLine[5]], int.Parse(splitLine[6])));
					}
					else if (splitLine[0] == "rock")
					{
						rocks.Add(new FallingRock(obTextures["rock"][0], new Vector2(int.Parse(splitLine[1]), int.Parse(splitLine[2])), new Vector2(int.Parse(splitLine[3]), int.Parse(splitLine[4])), new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), 64, 64)));
					}
					else if (splitLine[0] == "button")
					{
						buttons.Insert(int.Parse(splitLine[4]), new Button(obTextures["button"][0], new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), 32, 32), colors[splitLine[3]], int.Parse(splitLine[4])));
					}
					else if (splitLine[0] == "timer")
					{
						timers.Insert(int.Parse(splitLine[3]), new Timer(obTextures["timefront"][0], obTextures["timeback"][0], new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), 48, 48), int.Parse(splitLine[3]), int.Parse(splitLine[4]), int.Parse(splitLine[5]) == 1));
					}
					else if (splitLine[0] == "plasma")
					{
						plasmas.Add(new SpinningPlasma(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), 32, 32), int.Parse(splitLine[3]), float.Parse(splitLine[4]), splitLine[5] == "1", new AnimatedTexture(obTextures["plasma"], 1, false)));
					}
					else if (splitLine[0] == "launcher")
					{
						launchers.Add(new RocketLauncher(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), 48, 48), obTextures["launcher"][0], obTextures["rocket"][0], obTextures["trail"][0]));
					}
					else if (splitLine[0] == "bubbles")
					{
						bubbles.Add(new Bubbles(new Rectangle(int.Parse(splitLine[1]), int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4])), new Vector2(int.Parse(splitLine[5]), int.Parse(splitLine[6])), obTextures["bubble"][0]));
					}
				}
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
			if (fore == null)
				return;
			Rectangle bounds = new Rectangle(0, 0, w, h);
			sb.Draw(fore, bounds, Color.White);
		}

		public enum Type
		{
			Canyons,
			Caves,
			WarZone,
			Labs,
			Underwater,
			UnderwaterTransition,
			Final,
			EasterEgg
		}
	}
}
