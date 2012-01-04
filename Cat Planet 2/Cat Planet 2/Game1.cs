using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cat_Planet_2
{
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		// Objects
		Angel angel;
		List<ExplosionParticle> explosion;
		Cat[] cats;
		Cat HUDCat;
		Level[,] levels;
		Level currentLevel, previousLevel;

		// Sprites
		Texture2D[] angelNormal;
		Texture2D[] angelFly;
		Texture2D[] backgrounds;
		Texture2D[] catNormal;
		Texture2D[] catHit;
		Texture2D explosionTexture;
		Dictionary<string, Texture2D[]> obTextures;
		SpriteFont font;

		// Sounds
		SoundEffect hitWall;
		SoundEffect flap;
		SoundEffect meow;

		// Stats
		int catCount;
		int deaths;
		TimeSpan seconds;
		int wingFlaps;
		bool endGame;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		int windowWidth, windowHeight;

		public Game1()
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

			angelNormal = new Texture2D[2];
			angelFly = new Texture2D[3];
			backgrounds = new Texture2D[7];
			catNormal = new Texture2D[7];
			catHit = new Texture2D[8];
			obTextures = new Dictionary<string, Texture2D[]>();

			explosion = new List<ExplosionParticle>();
			levels = new Level[12, 12];
			cats = new Cat[80];

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
			angelNormal[0] = Content.Load<Texture2D>("angel/angel normal 1");
			angelNormal[1] = Content.Load<Texture2D>("angel/angel normal 2");
			angelFly[0] = Content.Load<Texture2D>("angel/angel fly 1");
			angelFly[1] = Content.Load<Texture2D>("angel/angel fly 2");
			angelFly[2] = Content.Load<Texture2D>("angel/angel fly 3");

			catNormal[0] = catNormal[6] = Content.Load<Texture2D>("cat/cat normal 1");
			catNormal[1] = catNormal[5] = Content.Load<Texture2D>("cat/cat normal 2");
			catNormal[2] = catNormal[4] = Content.Load<Texture2D>("cat/cat normal 3");
			catNormal[3] = Content.Load<Texture2D>("cat/cat normal 4");
			catHit[0] = catHit[4] = Content.Load<Texture2D>("cat/cat hit 1");
			catHit[1] = catHit[3] = Content.Load<Texture2D>("cat/cat hit 2");
			catHit[2] = Content.Load<Texture2D>("cat/cat hit 3");
			catHit[5] = catHit[7] = Content.Load<Texture2D>("cat/cat hit 4");
			catHit[6] = Content.Load<Texture2D>("cat/cat hit 5");

			explosionTexture = Content.Load<Texture2D>("explosion particle");

			// Load backgrounds
			backgrounds[0] = Content.Load<Texture2D>("backdrops/canyons");
			backgrounds[2] = Content.Load<Texture2D>("backdrops/caves");
			backgrounds[3] = Content.Load<Texture2D>("backdrops/warzone");
			backgrounds[4] = Content.Load<Texture2D>("backdrops/frozenwasteland");
			backgrounds[5] = Content.Load<Texture2D>("backdrops/underwater");
			backgrounds[6] = Content.Load<Texture2D>("backdrops/underwater transition");
			//backgrounds[7] = Content.Load<Texture2D>("backdrops/fault line");

			// Load sounds and music
			hitWall = Content.Load<SoundEffect>("thump");
			flap = Content.Load<SoundEffect>("whoosh");
			meow = Content.Load<SoundEffect>("meow");

			// Load cats
			HUDCat = new Cat(Vector2.Zero, -1, null,
				new AnimatedTexture(catNormal, 4, false), new AnimatedTexture(catHit, 4, false), font);
			cats[0] = new Cat(Vector2.Zero, 0,
				"                  Welcome to Cat Planet 2!\n" + 
				"This is an extra ammount of text to test my drawing method!\n" +
				"                          BEANS", 
				new AnimatedTexture(catNormal, 4, false), new AnimatedTexture(catHit, 4, false), font);
			cats[1] = new Cat(Vector2.Zero, 1,
				"Double the cat planet!",
				new AnimatedTexture(catNormal, 4, false), new AnimatedTexture(catHit, 4, false), font);
			cats[2] = new Cat(Vector2.Zero, 2,
				"Idk!",
				new AnimatedTexture(catNormal, 4, false), new AnimatedTexture(catHit, 4, false), font);

			// Load levels
			levels[11, 0] = new Level(new Vector2(11, 0), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-0"), cats, obTextures);
			levels[11, 1] = new Level(new Vector2(11, 1), Level.Type.Canyons, backgrounds[0], Content.Load<Texture2D>("foregrounds/11-1"), cats, obTextures);

			currentLevel = levels[11, 0];
			previousLevel = levels[10, 0];
			angel = new Angel(angelNormal, angelFly, hitWall, flap, new Vector2(windowWidth / 2, windowHeight - 128));
		}

		protected override void UnloadContent()
		{
			Content.Unload();
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();

			if (endGame) return;

			seconds = gameTime.TotalGameTime;

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

			if (explosion.Count > 0)
				foreach (ExplosionParticle e in explosion)
					e.Update();

			bool check = true;
			foreach (Obstacle o in currentLevel.obstacles)
			{
				o.Update();
				if (o.hitBox.Intersects(angel.hitBox))
				{
					Random direction = new Random();
					for (int i = 0; i < 50; i++)
						explosion.Add(new ExplosionParticle(new Vector2(angel.hitBox.X + 20, angel.hitBox.Y + 12), new Vector2((float)direction.NextDouble() * 12 - 6, (float)direction.NextDouble() * 12 - 6), explosionTexture));
					// TODO: Play sound
					angel.position = currentLevel.restartPosition[previousLevel.coordinates];
					deaths++;
					check = false;
					break;
				}
			}

			if (check)
				foreach (Link l in currentLevel.links)
				{
					if (angel.hitBox.Intersects(l.hitBox))
					{
						previousLevel = currentLevel;
						currentLevel = levels[(int)l.levelTo.X, (int)l.levelTo.Y];
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

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			spriteBatch.Begin();
			currentLevel.DrawBG(spriteBatch, windowWidth, windowHeight);
			foreach (Cat c in currentLevel.cats)
				c.Draw(spriteBatch);
			angel.Draw(spriteBatch);
			foreach (Obstacle o in currentLevel.obstacles)
				o.Draw(spriteBatch);
			currentLevel.DrawFG(spriteBatch, windowWidth, windowHeight);
			if (explosion.Count > 0)
				foreach (ExplosionParticle e in explosion)
					e.Draw(spriteBatch);
			HUDCat.Draw(spriteBatch);
			spriteBatch.DrawString(font, catCount.ToString(), new Vector2(35, 2), Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
