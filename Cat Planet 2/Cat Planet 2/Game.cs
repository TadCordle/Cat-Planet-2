using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cat_Planet_2
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		// Objects
		Angel angel;
		Explosion deathExplosion;
		Cat[] cats;
		Gem[] gems;
		Cat HUDCat;
		Level[,] levels;
		Level currentLevel, previousLevel;

		// Sprites
		Texture2D[] angelNormalTexture;
		Texture2D[] angelFlyTexture;
		Texture2D[] backgrounds;
		Texture2D[] catNormalTexture;
		Texture2D[] catHitTexture;
		Texture2D gemTexture;
		Texture2D explosionTexture;
		Dictionary<string, Texture2D[]> obTextures;
		SpriteFont font;

		// Sounds
		SoundEffect hitWall;
		SoundEffect flap;
		SoundEffect meow;
		SoundEffect explode;
		SoundEffect getGem;

		// Music
		Song canyonSong;
		Song caveSong;
		Song warSong;
		Song labSong;
		Song waterSong;
		Song finalSong;

		// Stats
		int catCount;
		int deaths;
		TimeSpan seconds;
		int wingFlaps;
		bool endGame;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		int windowWidth, windowHeight;

		public Game()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			windowWidth = graphics.PreferredBackBufferWidth = 960;
			windowHeight = graphics.PreferredBackBufferHeight = 720;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();
			Window.Title = "Cat Planet 2";

			angelNormalTexture = new Texture2D[2];
			angelFlyTexture = new Texture2D[3];
			backgrounds = new Texture2D[7];
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

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont>("font");

			// Load sprites
			angelNormalTexture[0] = Content.Load<Texture2D>("angel/angel normal 1");
			angelNormalTexture[1] = Content.Load<Texture2D>("angel/angel normal 2");
			angelFlyTexture[0] = Content.Load<Texture2D>("angel/angel fly 1");
			angelFlyTexture[1] = Content.Load<Texture2D>("angel/angel fly 2");
			angelFlyTexture[2] = Content.Load<Texture2D>("angel/angel fly 3");

			catNormalTexture[0] = catNormalTexture[6] = Content.Load<Texture2D>("cat/cat normal 1");
			catNormalTexture[1] = catNormalTexture[5] = Content.Load<Texture2D>("cat/cat normal 2");
			catNormalTexture[2] = catNormalTexture[4] = Content.Load<Texture2D>("cat/cat normal 3");
			catNormalTexture[3] = Content.Load<Texture2D>("cat/cat normal 4");
			catHitTexture[0] = catHitTexture[4] = Content.Load<Texture2D>("cat/cat hit 1");
			catHitTexture[1] = catHitTexture[3] = Content.Load<Texture2D>("cat/cat hit 2");
			catHitTexture[2] = Content.Load<Texture2D>("cat/cat hit 3");
			catHitTexture[5] = catHitTexture[7] = Content.Load<Texture2D>("cat/cat hit 4");
			catHitTexture[6] = Content.Load<Texture2D>("cat/cat hit 5");

			gemTexture = Content.Load<Texture2D>("gem");

			explosionTexture = Content.Load<Texture2D>("explosion particle");

			// Load backgrounds
			backgrounds[0] = Content.Load<Texture2D>("backdrops/canyons");
			backgrounds[2] = Content.Load<Texture2D>("backdrops/caves");
			backgrounds[3] = Content.Load<Texture2D>("backdrops/warzone");
			//backgrounds[4] = Content.Load<Texture2D>("backdrops/lab");
			backgrounds[5] = Content.Load<Texture2D>("backdrops/underwater");
			backgrounds[6] = Content.Load<Texture2D>("backdrops/underwater transition");
			//backgrounds[7] = Content.Load<Texture2D>("backdrops/final");

			// Load sounds and music
			hitWall = Content.Load<SoundEffect>("thump");
			flap = Content.Load<SoundEffect>("whoosh");
			meow = Content.Load<SoundEffect>("meow");

			// Load cats
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

			// Load gems
			gems[0] = new Gem(Vector2.Zero, gemTexture, Color.Cyan);
			gems[1] = new Gem(Vector2.Zero, gemTexture, Color.Lime);
			gems[2] = new Gem(Vector2.Zero, gemTexture, Color.Red);
			gems[3] = new Gem(Vector2.Zero, gemTexture, Color.Yellow);
			gems[4] = new Gem(Vector2.Zero, gemTexture, Color.Violet);

			// Load levels
			StreamReader sr = new StreamReader("Content/levels.txt");
			levels[9, 0] = new Level(sr, new Vector2(9, 0), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/9-0"), cats, gems, obTextures);
			levels[9, 1] = new Level(sr, new Vector2(9, 1), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/9-1"), cats, gems, obTextures);
			levels[9, 2] = new Level(sr, new Vector2(9, 2), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/9-2"), cats, gems, obTextures);
			levels[9, 3] = new Level(sr, new Vector2(9, 3), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/9-3"), cats, gems, obTextures);
			levels[9, 4] = new Level(sr, new Vector2(9, 4), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/9-4"), cats, gems, obTextures);
			levels[10, 0] = new Level(sr, new Vector2(10, 0), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/10-0"), cats, gems, obTextures);
			levels[10, 1] = new Level(sr, new Vector2(10, 1), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/10-1"), cats, gems, obTextures);
			levels[10, 2] = new Level(sr, new Vector2(10, 2), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/10-2"), cats, gems, obTextures);
			levels[10, 3] = new Level(sr, new Vector2(10, 3), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/10-3"), cats, gems, obTextures);
			levels[10, 4] = new Level(sr, new Vector2(10, 4), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/10-4"), cats, gems, obTextures);
			levels[11, 0] = new Level(sr, new Vector2(11, 0), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-0"), cats, gems, obTextures);
			levels[11, 1] = new Level(sr, new Vector2(11, 1), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-1"), cats, gems, obTextures);
			levels[11, 2] = new Level(sr, new Vector2(11, 2), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-2"), cats, gems, obTextures);
			levels[11, 3] = new Level(sr, new Vector2(11, 3), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-3"), cats, gems, obTextures);
			levels[11, 4] = new Level(sr, new Vector2(11, 4), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-4"), cats, gems, obTextures);

			currentLevel = levels[11, 0];
			previousLevel = levels[10, 0];
			angel = new Angel(angelNormalTexture, angelFlyTexture, hitWall, flap, new Vector2(windowWidth / 2, windowHeight - 128));
		}

		protected override void UnloadContent()
		{
			Content.Unload();
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();

			if (endGame)
			{
				seconds = gameTime.TotalGameTime;
				return;
			}

			wingFlaps += angel.Update(currentLevel.walls);
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

			if (currentLevel.gem != null)
			{
				if (angel.hitBox.Intersects(currentLevel.gem.hitBox) && !currentLevel.gem.taken)
				{
					currentLevel.gem.taken = true;
					// TODO: Play sound, maybe make effect
				}
				currentLevel.gem.Update();
			}

			if (deathExplosion != null)
				deathExplosion.Update();

			bool check = true;
			foreach (Obstacle o in currentLevel.obstacles)
			{
				o.Update();
				if (o.hitBox.Intersects(angel.hitBox) && o.isDeadly)
				{
					KillAngel();
					check = false;
					break;
				}
			}

			if (check)
				foreach (Link l in currentLevel.links)
				{
					if (angel.hitBox.Intersects(l.hitBox))
					{
						if (levels[(int)l.levelTo.X, (int)l.levelTo.Y] != null)
						{
							previousLevel = currentLevel;
							currentLevel = levels[(int)l.levelTo.X, (int)l.levelTo.Y];
						}
						else
						{
							KillAngel();
							break;
						}
						if (l.flipx)
							angel.position.X = (angel.position.X < windowWidth / 2 ? windowWidth - 56 : -16);
						if (l.flipy)
							angel.position.Y = (angel.position.Y < windowHeight / 2 ? windowHeight - 40 : -16);
						break;
					}
				}

			HUDCat.hit = false;
			HUDCat.Update();

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

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			spriteBatch.Begin();
			currentLevel.DrawBG(spriteBatch, windowWidth, windowHeight);
			if (currentLevel.gem != null)
				currentLevel.gem.Draw(spriteBatch);
			angel.Draw(spriteBatch);
			foreach (Obstacle o in currentLevel.obstacles)
				o.Draw(spriteBatch);
			currentLevel.DrawFG(spriteBatch, windowWidth, windowHeight);
			foreach (Cat c in currentLevel.cats)
				c.Draw(spriteBatch);
			if (deathExplosion != null)
				deathExplosion.Draw(spriteBatch);
			HUDCat.Draw(spriteBatch);
			spriteBatch.DrawString(font, catCount.ToString(), new Vector2(35, 2), Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
