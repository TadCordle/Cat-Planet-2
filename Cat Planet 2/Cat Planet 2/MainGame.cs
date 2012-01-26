// STUFF TO DO
// break levels.txt into individual files
// alter level parser in levels.cs
// change initially loaded levels
// Uncomment LoadLevels() in Update method

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if XBOX
using Microsoft.Xna.Framework.GamerServices; 
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cat_Planet_2
{
	public class MainGame : Microsoft.Xna.Framework.Game
	{
		#region Variables
		// Objects
		Angel angel;
		Explosion deathExplosion;
		Explosion gemExplosion;
		Cat[] cats;
		Gem[] gems;
		Cat HUDCat;
		Level[,] levels;
		Level currentLevel, previousLevel;
		Vector2 creditPosition;

		// Sprites
		Texture2D[] angelNormalTexture;
		Texture2D[] angelFlyTexture;
		Texture2D[] backgrounds;
		Texture2D[] catNormalTexture;
		Texture2D[] catHitTexture;
		Texture2D gemTexture;
		Texture2D explosionTexture;
		Texture2D pixel;
		Dictionary<string, Texture2D[]> obTextures;
		SpriteFont font;

		// Sounds
		SoundEffect hitWall;
		SoundEffect flap;
		SoundEffect meow;
		SoundEffect explode;
		SoundEffect getGem;
		SoundEffect hitButton;
		SoundEffect ticking;
		SoundEffectInstance loopTick;
		SoundEffect rocketLaunch;

		// Music
		Song currentSong;
		Song canyonSong;
		Song caveSong;
		Song warSong;
		Song labSong;
		Song waterSong;
		Song finalSong;

		// lololol
		Video easterEgg;
		VideoPlayer videoPlayer;

		// Stats
		int catCount;
		int deaths;
		TimeSpan seconds;
		int wingFlaps;
		bool endGame;
		bool goTime;

		ContentManager levelTextureHandler;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		int windowWidth, windowHeight;
		#endregion

		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}
		
		protected override void Initialize()
		{
			// Set up window
			windowWidth = graphics.PreferredBackBufferWidth = 960;
			windowHeight = graphics.PreferredBackBufferHeight = 720;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();
			Window.Title = "Cat Planet 2";

			// Initialize objects
			creditPosition = new Vector2(240, windowHeight);

			angelNormalTexture = new Texture2D[2];
			angelFlyTexture = new Texture2D[3];
			backgrounds = new Texture2D[7]; // Delete this later
			catNormalTexture = new Texture2D[7];
			catHitTexture = new Texture2D[8];
			obTextures = new Dictionary<string, Texture2D[]>();

			levels = new Level[12, 12];
			cats = new Cat[80];
			gems = new Gem[5];

			catCount = 0;
			deaths = 0;
			seconds = new TimeSpan(0, 0, 0);
			wingFlaps = 0;
			endGame = false;
			goTime = true;

			videoPlayer = new VideoPlayer();
			
			base.Initialize();
		}
		
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			levelTextureHandler = new ContentManager(this.Services, "Content");
			font = Content.Load<SpriteFont>("font");

			#region Load angel
			angelNormalTexture[0] = Content.Load<Texture2D>("angel/angel normal 1");
			angelNormalTexture[1] = Content.Load<Texture2D>("angel/angel normal 2");
			angelFlyTexture[0] = Content.Load<Texture2D>("angel/angel fly 1");
			angelFlyTexture[1] = Content.Load<Texture2D>("angel/angel fly 2");
			angelFlyTexture[2] = Content.Load<Texture2D>("angel/angel fly 3");
			#endregion
			#region Load cat
			catNormalTexture[0] = catNormalTexture[6] = Content.Load<Texture2D>("cat/cat normal 1");
			catNormalTexture[1] = catNormalTexture[5] = Content.Load<Texture2D>("cat/cat normal 2");
			catNormalTexture[2] = catNormalTexture[4] = Content.Load<Texture2D>("cat/cat normal 3");
			catNormalTexture[3] = Content.Load<Texture2D>("cat/cat normal 4");
			catHitTexture[0] = catHitTexture[4] = Content.Load<Texture2D>("cat/cat hit 1");
			catHitTexture[1] = catHitTexture[3] = Content.Load<Texture2D>("cat/cat hit 2");
			catHitTexture[2] = Content.Load<Texture2D>("cat/cat hit 3");
			catHitTexture[5] = catHitTexture[7] = Content.Load<Texture2D>("cat/cat hit 4");
			catHitTexture[6] = Content.Load<Texture2D>("cat/cat hit 5");
			#endregion
			#region Load objects
			gemTexture = Content.Load<Texture2D>("objects/gem");
			pixel = Content.Load<Texture2D>("pixel");
			explosionTexture = Content.Load<Texture2D>("objects/explosion particle");

			Texture2D[] fenceTexture = new Texture2D[1];
			fenceTexture[0] = Content.Load<Texture2D>("pixel");
			obTextures.Add("fence", fenceTexture);
			Texture2D[] rockTexture = new Texture2D[1];
			rockTexture[0] = Content.Load<Texture2D>("objects/rock");
			obTextures.Add("rock", rockTexture);
			Texture2D[] buttonTexture = new Texture2D[1];
			buttonTexture[0] = Content.Load<Texture2D>("objects/button");
			obTextures.Add("button", buttonTexture);
			Texture2D[] timeFrontTexture = new Texture2D[1];
			timeFrontTexture[0] = Content.Load<Texture2D>("objects/timer front");
			obTextures.Add("timefront", timeFrontTexture);
			Texture2D[] timeBackTexture = new Texture2D[1];
			timeBackTexture[0] = Content.Load<Texture2D>("objects/timer back");
			obTextures.Add("timeback", timeBackTexture);
			Texture2D[] plasmaTexture = new Texture2D[3];
			plasmaTexture[0] = Content.Load<Texture2D>("objects/plasma ball 1");
			plasmaTexture[1] = Content.Load<Texture2D>("objects/plasma ball 2");
			plasmaTexture[2] = Content.Load<Texture2D>("objects/plasma ball 3");
			obTextures.Add("plasma", plasmaTexture);
			Texture2D[] rocketTexture = new Texture2D[1];
			rocketTexture[0] = Content.Load<Texture2D>("objects/rocket");
			obTextures.Add("rocket", rocketTexture);
			Texture2D[] launcherTexture = new Texture2D[1];
			launcherTexture[0] = Content.Load<Texture2D>("objects/launcher");
			obTextures.Add("launcher", launcherTexture);
			Texture2D[] trailTexture = new Texture2D[1];
			trailTexture[0] = explosionTexture;
			obTextures.Add("trail", trailTexture);
			#endregion
			#region Load music
			// Load sounds and music
			canyonSong = Content.Load<Song>("canyonmus");
			caveSong = Content.Load<Song>("cavemus");
			labSong = Content.Load<Song>("labmus");
			//warSong = Content.Load<Song>("warmus");
			//waterSong = Content.Load<Song>("watermus");
			//finalSong = Content.Load<Song>("finalmus");
			#endregion
			#region Load sounds
			hitWall = Content.Load<SoundEffect>("thump");
			flap = Content.Load<SoundEffect>("whoosh");
			meow = Content.Load<SoundEffect>("meow");
			//explode = Content.Load<SoundEffect>("explode");
			//getGem = Content.Load<SoundEffect>("getgem");
			//hitButton = Content.Load<SoundEffect>("hitbutton");
			//ticking = Content.Load<SoundEffect>("tick");
			//loopTick = ticking.CreateInstance();
			//loopTick.IsLooped = true;
			//rocketLaunch = Content.Load<SoundEffect>("rocketlaunch");
			#endregion
			#region Load easter egg
			easterEgg = Content.Load<Video>("easter egg");
			videoPlayer.IsLooped = true;
			videoPlayer.Volume = 1.0f;
			#endregion

			#region Instantiate cats
			HUDCat = new Cat(Vector2.Zero, -1, null,
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[0] = new Cat(Vector2.Zero, 0,
				"Welcome to Cat Planet 2!", 
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[1] = new Cat(Vector2.Zero, 1,
				"Double the cat planet!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[2] = new Cat(Vector2.Zero, 2,
				"I'm lost :(",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[3] = new Cat(Vector2.Zero, 3,
				"Explore more Cat Planet!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[4] = new Cat(Vector2.Zero, 4,
				"Cat Village was destroyed by the crows!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[5] = new Cat(Vector2.Zero, 5,
				"So we moved here!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[6] = new Cat(Vector2.Zero, 6,
				"Cat Planet Canyon Planet!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[7] = new Cat(Vector2.Zero, 7,
				"Find all of the gems of light!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[8] = new Cat(Vector2.Zero, 8,
				"Uncover the secrets of Cat Planet!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[9] = new Cat(Vector2.Zero, 9,
				"You need to collect the gems to\n" +
				"        prove yourself!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[10] = new Cat(Vector2.Zero, 10,
				"Our friend is gone!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[11] = new Cat(Vector2.Zero, 11,
				"He went up and never came back!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[12] = new Cat(Vector2.Zero, 12,
				"These caves are scary!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[13] = new Cat(Vector2.Zero, 13,
				"I'm in a ditch.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[14] = new Cat(Vector2.Zero, 14,
				"The gem is close by!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[15] = new Cat(Vector2.Zero, 15,
				"It's left from here.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[16] = new Cat(Vector2.Zero, 16,
				"It's down there!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[17] = new Cat(Vector2.Zero, 17,
				"Did you get the gem?",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[18] = new Cat(Vector2.Zero, 18,
				"Cat planet everything!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[19] = new Cat(Vector2.Zero, 19,
				"The fish are evil!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[20] = new Cat(Vector2.Zero, 20,
				"There's more danger past these spikes!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[21] = new Cat(Vector2.Zero, 21,
				"There's a secret close by.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[22] = new Cat(Vector2.Zero, 22,
				"I don't know where these rocks\n" +
				"      are coming from!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[23] = new Cat(Vector2.Zero, 23,
				"The fish have made Cat Planet\n" +
				"     a dangerous place!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[25] = new Cat(Vector2.Zero, 25,
				"Cat!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[26] = new Cat(Vector2.Zero, 26,
				"Pla!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[27] = new Cat(Vector2.Zero, 27,
				"Net!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[28] = new Cat(Vector2.Zero, 28,
				"Help!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[29] = new Cat(Vector2.Zero, 29,
				"Up there is a war zone!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[30] = new Cat(Vector2.Zero, 30,
				"Spinning plasma of doom!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[31] = new Cat(Vector2.Zero, 31,
				"Obviously work of the fish!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[32] = new Cat(Vector2.Zero, 32,
				"The gem!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[33] = new Cat(Vector2.Zero, 33,
				"Be careful here!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[34] = new Cat(Vector2.Zero, 34,
				"The cat below me is lying!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[35] = new Cat(Vector2.Zero, 35,
				"The bottom cat is\n" + 
				"telling the truth!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[36] = new Cat(Vector2.Zero, 36,
				"I'm lying!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[37] = new Cat(Vector2.Zero, 37,
				"The first cat is the same\n" +
				"  as the one above me!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[38] = new Cat(Vector2.Zero, 38,
				"Those rocket launchers are\n" +
				"       dangerous!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[39] = new Cat(Vector2.Zero, 39,
				"Explosions are cool!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[47] = new Cat(Vector2.Zero, 47,
				"Reminds me of 'Nam.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);

			#endregion
			#region Instantiate gems
			gems[0] = new Gem(Vector2.Zero, gemTexture, Color.Cyan);
			gems[1] = new Gem(Vector2.Zero, gemTexture, Color.Lime);
			gems[2] = new Gem(Vector2.Zero, gemTexture, Color.Red);
			gems[3] = new Gem(Vector2.Zero, gemTexture, Color.Yellow);
			gems[4] = new Gem(Vector2.Zero, gemTexture, Color.Violet);
			#endregion

			// Set up starting level
			StreamReader sr = new StreamReader("Content/levels/11-0.txt");
			levels[11, 0] = new Level(sr, new Vector2(11, 0), Level.Type.Canyons, Content.Load<Texture2D>("backdrops/canyons"), levelTextureHandler.Load<Texture2D>("foregrounds/11-0"), cats, gems, obTextures);
			sr.Close();
			sr = new StreamReader("Content/levels/10-0.txt");
			levels[10, 0] = new Level(sr, new Vector2(10, 0), Level.Type.Canyons, Content.Load<Texture2D>("backdrops/canyons"), levelTextureHandler.Load<Texture2D>("foregrounds/10-0"), cats, gems, obTextures);
			sr.Close();

			currentLevel = levels[11, 0]; // Start = 11, 0
			previousLevel = levels[10, 0];
			LoadLevels();

			angel = new Angel(angelNormalTexture, angelFlyTexture, hitWall, flap, new Vector2(windowWidth / 2, 459));
			currentSong = canyonSong;
			MediaPlayer.Volume = 0.85f;
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(currentSong);
		}
		
		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			// Exit when pressing Escape
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();

			if (!endGame)
			{
				wingFlaps += angel.Update(currentLevel.walls);

				#region Update collectables
				// Check cat collisions
				foreach (Cat c in currentLevel.cats)
				{
					if (angel.hitBox.Intersects(c.hitBox) && !c.hit)
					{
						c.hit = true;
						catCount++;
						meow.Play(0.4F, 0.0F, 0.0F);
					}
					c.Update();
				}

				// Check gem collisions
				if (currentLevel.gem != null)
				{
					if (angel.hitBox.Intersects(currentLevel.gem.hitBox) && !currentLevel.gem.taken)
					{
						currentLevel.gem.taken = true;
						// TODO: Play sound
						gemExplosion = new Explosion(new Vector2(currentLevel.gem.hitBox.X, currentLevel.gem.hitBox.Y), currentLevel.gem.color, explosionTexture);
					}
					currentLevel.gem.Update();
				}
				#endregion

				// Update explosions
				if (deathExplosion != null)
					deathExplosion.Update();
				if (gemExplosion != null)
					gemExplosion.Update();

				#region Update obstacles
				// Check obstacle collisions
				bool check = true;
				foreach (DeathWall o in currentLevel.deathWalls)
				{
					if (o.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						break;
					}
				}
				foreach (FallingRock o in currentLevel.rocks)
				{
					o.Update();
					if (o.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						break;
					}
				}
				foreach (ElectricFence e in currentLevel.fences)
				{
					e.Update();
					if (e.index < currentLevel.timers.Count)
						e.isDeadly = !currentLevel.timers[e.index].activated;
					if (e.hitBox.Intersects(angel.hitBox))
					{
						if (e.isDeadly)
						{
							KillAngel();
							check = false;
							break;
						}
					}
				}
				foreach (SpinningPlasma o in currentLevel.plasmas)
				{
					o.Update();
					if (o.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						break;
					}
				}
				foreach (RocketLauncher r in currentLevel.launchers)
				{
					if (r.Update(angel, currentLevel.walls, currentLevel.links, rocketLaunch))
					{
						gemExplosion = new Explosion(new Vector2(r.rocket.hitBox.X, r.rocket.hitBox.Y), Color.OrangeRed, explosionTexture);
						//Play explosion sound
						if (angel.hitBox.Intersects(r.rocket.hitBox))
						{
							KillAngel();
							check = false;
						}
						r.pause = 0;
						r.rocket.position.X = -10000;
						r.rocket.position.Y = -10000;
					}
				}
				#endregion

				#region Update timers and buttons
				foreach (Timer t in currentLevel.timers)
				{
					t.Update(currentLevel);
				}

				// Check button and timer status
				for (int i = 0; i < currentLevel.buttons.Count; i++)
				{
					if (angel.hitBox.Intersects(currentLevel.buttons[i].hitBox))
					{
						currentLevel.buttons[i].activated = true;
						currentLevel.timers[i].activated = true;
					}
				}
				#endregion

				#region Check links
				// Check level link collisions
				if (check)
					foreach (Link l in currentLevel.links)
					{
						if (angel.hitBox.Intersects(l.hitBox))
						{
							if (levels[(int)l.levelTo.X, (int)l.levelTo.Y] != null)
							{
								previousLevel = currentLevel;
								currentLevel = levels[(int)l.levelTo.X, (int)l.levelTo.Y];
								LoadLevels();

								foreach (FallingRock r in currentLevel.rocks)
								{
									r.hitBox.Y = (int)r.initialPosition.Y;
									r.motion.Y = r.initialSpeed.Y;
								}
								foreach (RocketLauncher r in currentLevel.launchers)
								{
									r.rocket.position = new Vector2(-10000, -10000);
									r.pause = 0;
								}
								if (previousLevel.type != currentLevel.type)
									ChangeSong();
							}
							else
							{
								// If no levels is found, end game or kill angel
								// Delete this out when finished with game //
								if (goTime)
								{
									endGame = true;
									break;
								}
								else
								/////////////////////////////////////////////
								{
									KillAngel();
								}
							}

							// Update angel position after switching levels
							if (l.flipx)
								angel.position.X = (angel.position.X < windowWidth / 2 ? windowWidth - 56 : -16);
							if (l.flipy)
								angel.position.Y = (angel.position.Y < windowHeight / 2 ? windowHeight - 40 : -16);
							break;
						}
					}
				#endregion

				// Update HUD count
				HUDCat.hit = false;
				HUDCat.Update();
			}
			else
			{
				// Take final time when game is over
				if (goTime)
				{
					seconds = gameTime.TotalGameTime;
					goTime = false;
				}

				// Show credits until finished scrolling
				if (creditPosition.Y < -800)
				{
					endGame = false;
					KillAngel();
				}
				else
					creditPosition.Y -= 4;
			}

			base.Update(gameTime);
		}
		private void KillAngel()
		{
			deathExplosion = new Explosion(new Vector2(angel.hitBox.X + 8, angel.hitBox.Y + 8), Color.White, explosionTexture);
			// TODO: Play sound
			angel.position = currentLevel.restartPosition[previousLevel.coordinates];
			angel.motion = Vector2.Zero;
			deaths++;
		}
		private void ChangeSong()
		{
			MediaPlayer.Stop();
			videoPlayer.Stop();
			switch (currentLevel.type)
			{
				case Level.Type.Canyons:
					currentSong = canyonSong;
					break;
				case Level.Type.Caves:
					currentSong = caveSong;
					break;
				case Level.Type.WarZone:
					currentSong = warSong;
					break;
				case Level.Type.Labs:
					currentSong = labSong;
					break;
				case Level.Type.Underwater:
					currentSong = waterSong;
					break;
				case Level.Type.UnderwaterTransition:
					currentSong = null;
					break;
				case Level.Type.Final:
					currentSong = finalSong;
					break;
				case Level.Type.EasterEgg:
					currentSong = null;
					videoPlayer.Play(easterEgg);
					break;
			}
			if (currentSong != null)
				MediaPlayer.Play(currentSong);
		}
		private void LoadLevels()
		{
			levelTextureHandler = new ContentManager(this.Services, "Content");

			for (int i = 0; i < 12; i++)
				for (int j = 0; j < 12; j++)
					if ((Math.Abs(currentLevel.coordinates.X - i) > 1 || Math.Abs(currentLevel.coordinates.Y - j) > 1) && levels[i, j] != null)
					{
						levels[i, j].fore.Dispose();
						levels[i, j] = null;
					}

			for (int i = -1; i <= 1; i++)
				for (int j = -1; j <= 1; j++)
					if (i != 0 || j != 0)
					{
						if (currentLevel.coordinates.X + i >= levels.GetLength(0) || currentLevel.coordinates.Y + j >= levels.GetLength(1) ||
							currentLevel.coordinates.X + i < 0 || currentLevel.coordinates.Y + j < 0)
							continue;
						if (levels[(int)currentLevel.coordinates.X + i, (int)currentLevel.coordinates.Y + j] != null)
							continue;

						if (File.Exists("Content/levels/" + ((int)currentLevel.coordinates.X + i).ToString() + "-" + ((int)currentLevel.coordinates.Y + j).ToString() + ".txt"))
						{
							StreamReader sr = new StreamReader("Content/levels/" + ((int)currentLevel.coordinates.X + i).ToString() + "-" + ((int)currentLevel.coordinates.Y + j).ToString() + ".txt");
							string bg = sr.ReadLine();
							levels[(int)currentLevel.coordinates.X + i, (int)currentLevel.coordinates.Y + j] = new Level(
								sr,
								new Vector2(currentLevel.coordinates.X + i, currentLevel.coordinates.Y + j),
								GetBGType(bg),
								bg != "easter egg" ? Content.Load<Texture2D>("backdrops/" + bg) : pixel,
								bg != "easter egg" ? levelTextureHandler.Load<Texture2D>("foregrounds/" + ((int)currentLevel.coordinates.X + i).ToString() + "-" + ((int)currentLevel.coordinates.Y + j).ToString()) : levelTextureHandler.Load<Texture2D>("pixel"),
								cats, gems, obTextures);
							sr.Close();
						}
					}
		}
		private Level.Type GetBGType(string bg)
		{
			switch (bg)
			{
				case "canyons":
					return Level.Type.Canyons;
				case "caves":
					return Level.Type.Caves;
				case "lab":
					return Level.Type.Labs;
				case "warzone":
					return Level.Type.WarZone;
				case "underwater":
					return Level.Type.Underwater;
				case "underwater transition":
					return Level.Type.UnderwaterTransition;
				case "final":
					return Level.Type.Final;
				default:
					return Level.Type.EasterEgg;
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin();
			if (!endGame)
			{
				currentLevel.DrawBG(spriteBatch, windowWidth, windowHeight);
				if (videoPlayer.State == MediaState.Playing)
					spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
				if (currentLevel.gem != null)
					currentLevel.gem.Draw(spriteBatch);
				foreach (Button b in currentLevel.buttons)
					b.Draw(spriteBatch);
				foreach (Timer t in currentLevel.timers)
					t.Draw(spriteBatch);
				angel.Draw(spriteBatch);
				foreach (DeathWall o in currentLevel.deathWalls)
					o.Draw(spriteBatch);
				foreach (FallingRock o in currentLevel.rocks)
					o.Draw(spriteBatch);
				foreach (RocketLauncher r in currentLevel.launchers)
				{
					r.Draw(spriteBatch);
					r.rocket.Draw(spriteBatch);
				}
				if (currentLevel != levels[0, 0] && currentLevel != levels[5, 5])
					currentLevel.DrawFG(spriteBatch, windowWidth, windowHeight);
				else
					foreach (Wall w in currentLevel.walls)
						w.Draw(pixel, spriteBatch);
				foreach (ElectricFence e in currentLevel.fences)
					e.Draw(spriteBatch);
				foreach (SpinningPlasma p in currentLevel.plasmas)
					p.Draw(spriteBatch);
				foreach (Cat c in currentLevel.cats)
					c.Draw(spriteBatch);
				if (deathExplosion != null)
					deathExplosion.Draw(spriteBatch);
				if (gemExplosion != null)
					gemExplosion.Draw(spriteBatch);
				HUDCat.Draw(spriteBatch);
				spriteBatch.DrawString(font, catCount.ToString(), new Vector2(35, 2), Color.White);
			}
			else
			{
				spriteBatch.DrawString(font,
					"             CAT PLANET 2\n" + 
					"               CREDITS\n\n" + 
					"Programming Graphics Music Design Everything\n" + 
					"              Tad Cordle\n\n" + 
					"              Tools Used\n" +
					"       XNA 4.0 for Visual Studio 2010\n" +
					"              Sibelius 6\n" + 
					"              Flash CS5\n\n" +
					"       Sound Effects from flashkit.com\n\n" + 
					"            Special Thanks\n" +
					"                Nobody\n\n" +
 					"         Cats Collected: " + catCount.ToString() + "/" + cats.Count().ToString() + "\n" +
					"              Time: " + seconds.Minutes.ToString() + ":" + (seconds.Seconds < 10 ? "0" : "") + seconds.Seconds + "\n" +
					"            Deaths: " + deaths.ToString() + "\n" +
					"             Jumps: " + wingFlaps.ToString() + "\n\n" +
					"        THANKS FOR PLAYING AND STUFF",
					creditPosition, Color.White);
			}
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
