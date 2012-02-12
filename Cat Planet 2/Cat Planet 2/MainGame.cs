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
		ElectricFence[] finalFences;
		Cat HUDCat;
		Level[,] levels;
		Level currentLevel, previousLevel;
		Vector2 creditPosition;

		// Sprites
		Texture2D[] angelNormalTexture;
		Texture2D[] angelFlyTexture;
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
		SoundEffect die;
		SoundEffect meow;
		SoundEffect explode;
		SoundEffect getGem;
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
			catNormalTexture = new Texture2D[7];
			catHitTexture = new Texture2D[8];
			obTextures = new Dictionary<string, Texture2D[]>();

			levels = new Level[12, 12];
			cats = new Cat[68];
			gems = new Gem[5];
			finalFences = new ElectricFence[5];

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
			Texture2D[] bubbleTexture = new Texture2D[1];
			bubbleTexture[0] = explosionTexture;
			obTextures.Add("bubble", bubbleTexture);
			Texture2D[] starTexture = new Texture2D[1];
			starTexture[0] = Content.Load<Texture2D>("objects/starfish");
			obTextures.Add("starfish", starTexture);
			Texture2D[] fishTexture = new Texture2D[2];
			fishTexture[0] = Content.Load<Texture2D>("objects/fish 1");
			fishTexture[1] = Content.Load<Texture2D>("objects/fish 2");
			obTextures.Add("fish", fishTexture);
			#endregion
			#region Load music
			canyonSong = Content.Load<Song>("canyonmus");
			caveSong = Content.Load<Song>("cavemus");
			labSong = Content.Load<Song>("labmus");
			warSong = Content.Load<Song>("warmus");
			waterSong = Content.Load<Song>("watermus");
			finalSong = Content.Load<Song>("finalmus");
			#endregion
			#region Load sounds
			hitWall = Content.Load<SoundEffect>("thump");
			flap = Content.Load<SoundEffect>("whoosh");
			meow = Content.Load<SoundEffect>("meow");
			explode = Content.Load<SoundEffect>("explosion");
			die = Content.Load<SoundEffect>("dead");
			getGem = Content.Load<SoundEffect>("get gem");
			ticking = Content.Load<SoundEffect>("ticking");
			loopTick = ticking.CreateInstance();
			loopTick.IsLooped = true;
			loopTick.Volume = 0.25f;
			rocketLaunch = Content.Load<SoundEffect>("rocket launch");
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
				"I wasn't in the first game!",
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
			cats[24] = new Cat(Vector2.Zero, 24,
				"War is not the answer!",
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
			cats[40] = new Cat(Vector2.Zero, 40,
				"More rocks!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[41] = new Cat(Vector2.Zero, 41,
				"   The crows are no more,\n" +
				"but the fish are much worse.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[42] = new Cat(Vector2.Zero, 42,
				"So much destruction!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[43] = new Cat(Vector2.Zero, 43,
				"We're losing!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[44] = new Cat(Vector2.Zero, 44,
				"The gem here is well\n" +
				"     protected!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[45] = new Cat(Vector2.Zero, 45,
				"My friend is trapped!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[46] = new Cat(Vector2.Zero, 46,
				"I'm actually safe from\n" +
				" the rockets up here!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[47] = new Cat(Vector2.Zero, 47,
				"Reminds me of 'Nam.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[48] = new Cat(Vector2.Zero, 48,
				"I think that's what we're\n" +
				"    fighting over...",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[49] = new Cat(Vector2.Zero, 49,
				"They're everywhere!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[50] = new Cat(Vector2.Zero, 50,
				"Imperialist fish!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[51] = new Cat(Vector2.Zero, 51,
				"We're underwater!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[52] = new Cat(Vector2.Zero, 52,
				"Cat Planet Ocean!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[53] = new Cat(Vector2.Zero, 53,
				"The bubbles carry\n" +
				"   you places!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[54] = new Cat(Vector2.Zero, 54,
				"Cats don't like water!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[55] = new Cat(Vector2.Zero, 55,
				"BUBBLES!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[56] = new Cat(Vector2.Zero, 56,
				"Time it right!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[57] = new Cat(Vector2.Zero, 57,
				"Starfish are dangerous!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[58] = new Cat(Vector2.Zero, 58,
				"But they're nothing \n" +
				"  compared to the\n" +
				"       fish!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[59] = new Cat(Vector2.Zero, 59,
				"You're close to the\n" + 
				"  final struggle!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[60] = new Cat(Vector2.Zero, 60,
				"You need all the gems\n" +
				"of light to continue!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[61] = new Cat(Vector2.Zero, 61,
				"  Do you have 62 cats?\n" +
				"There's no turning back.",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[62] = new Cat(Vector2.Zero, 62,
				"It's the fish!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[63] = new Cat(Vector2.Zero, 63,
				"The fish are dangerous!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[64] = new Cat(Vector2.Zero, 64,
				"Keep going!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[65] = new Cat(Vector2.Zero, 65,
				"You're getting close!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[66] = new Cat(Vector2.Zero, 66,
				"You have to go up!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			cats[67] = new Cat(Vector2.Zero, 67,
				"You did it!",
				new AnimatedTexture(catNormalTexture, 4, false), new AnimatedTexture(catHitTexture, 4, false), font);
			#endregion
			#region Instantiate gems
			gems[0] = new Gem(Vector2.Zero, gemTexture, Color.Cyan);
			gems[1] = new Gem(Vector2.Zero, gemTexture, Color.Lime);
			gems[2] = new Gem(Vector2.Zero, gemTexture, Color.Red);
			gems[3] = new Gem(Vector2.Zero, gemTexture, Color.Yellow);
			gems[4] = new Gem(Vector2.Zero, gemTexture, Color.Violet);
			#endregion
			#region Instantiate final fences
			finalFences[0] = new ElectricFence(new Rectangle(319, 478, 349, 32), obTextures["fence"][0], Color.Cyan, 0);
			finalFences[1] = new ElectricFence(new Rectangle(319, 527, 349, 32), obTextures["fence"][0], Color.Lime, 1);
			finalFences[2] = new ElectricFence(new Rectangle(319, 576, 349, 32), obTextures["fence"][0], Color.Red, 2);
			finalFences[3] = new ElectricFence(new Rectangle(319, 625, 349, 32), obTextures["fence"][0], Color.Yellow, 3);
			finalFences[4] = new ElectricFence(new Rectangle(319, 674, 349, 32), obTextures["fence"][0], Color.Violet, 4);
			#endregion

			StreamReader sr = new StreamReader("Content/levels/11-0.txt");
			levels[11, 0] = new Level(sr, new Vector2(11, 0), Level.Type.Canyons, Content.Load<Texture2D>("backdrops/canyons"), levelTextureHandler.Load<Texture2D>("foregrounds/11-0"), cats, gems, obTextures);
			sr.Close();
			sr = new StreamReader("Content/levels/10-0.txt");
			levels[10, 0] = new Level(sr, new Vector2(10, 0), Level.Type.Canyons, Content.Load<Texture2D>("backdrops/canyons"), levelTextureHandler.Load<Texture2D>("foregrounds/10-0"), cats, gems, obTextures);
			sr.Close();
			sr = new StreamReader("Content/levels/0-11.txt");
			levels[0, 11] = new Level(sr, new Vector2(0, 11), Level.Type.Final, Content.Load<Texture2D>("backdrops/final"), levelTextureHandler.Load<Texture2D>("foregrounds/0-11"), cats, gems, obTextures);
			sr.Close();
			sr = new StreamReader("Content/levels/0-10.txt");
			levels[0, 10] = new Level(sr, new Vector2(0, 10), Level.Type.UnderwaterTransition, Content.Load<Texture2D>("backdrops/underwater transition"), levelTextureHandler.Load<Texture2D>("foregrounds/0-10"), cats, gems, obTextures);
			sr.Close();
			sr = new StreamReader("Content/levels/0-0.txt");
			levels[0, 0] = new Level(sr, new Vector2(0, 0), Level.Type.Final, Content.Load<Texture2D>("backdrops/final"), levelTextureHandler.Load<Texture2D>("pixel"), cats, gems, obTextures);
			sr.Close();
			sr.Dispose();

			//currentLevel = levels[11, 0]; // Start
			currentLevel = levels[0, 11]; // Fish lair tests
			//currentLevel = levels[0, 0]; // New objects test
			//previousLevel = levels[10, 0]; // Start/New objects tests
			previousLevel = levels[0, 10]; // Fish lair tests
			LoadLevels();

			angel = new Angel(angelNormalTexture, angelFlyTexture, hitWall, flap, new Vector2(windowWidth / 2, 459) /*new Vector2(482, 214)*/);
			MediaPlayer.Volume = 0.85f;
			MediaPlayer.IsRepeating = true;
			ChangeSong();
		}
		
		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();

			if (!endGame)
			{
				wingFlaps += angel.Update(currentLevel.walls);

				#region Lol glitch
				if (angel.position.Y > 2000 && currentLevel.type != Level.Type.EasterEgg)
				{
					currentLevel.type = Level.Type.EasterEgg;
					currentLevel.fore = null;
					ChangeSong();
				}
				#endregion

				#region Update collectables
				foreach (Cat c in currentLevel.cats)
				{
					if (angel.hitBox.Intersects(c.hitBox) && !c.hit)
					{
						c.hit = true;
						catCount++;
						if (currentLevel == levels[9, 10])
							if (goTime)
							{
								endGame = true;
								break;
							}
						meow.Play(0.4F, 0.0F, 0.0F);
					}
					c.Update();
				}

				if (currentLevel.gem != null)
				{
					if (angel.hitBox.Intersects(currentLevel.gem.hitBox) && !currentLevel.gem.taken)
					{
						currentLevel.gem.taken = true;
						getGem.Play();
						foreach (ElectricFence e in finalFences)
							if (e.color == currentLevel.gem.color)
							{
								e.isDeadly = false;
								break;
							}
						gemExplosion = new Explosion(new Vector2(currentLevel.gem.hitBox.X, currentLevel.gem.hitBox.Y), currentLevel.gem.color, explosionTexture);
					}
					currentLevel.gem.Update();
				}
				#endregion

				if (deathExplosion != null)
					deathExplosion.Update();
				if (gemExplosion != null)
					gemExplosion.Update();

				#region Update obstacles
				bool check = true;
				foreach (DeathWall o in currentLevel.deathWalls)
				{
					if (o.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						continue;
					}
				}
				foreach (FallingRock o in currentLevel.rocks)
				{
					o.Update();
					if (o.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						continue;
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
							continue;
						}
					}
				}
				if (currentLevel.coordinates.X == 0 && currentLevel.coordinates.Y == 10)
					foreach (ElectricFence e in finalFences)
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
								continue;
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
						continue;
					}
				}
				foreach (RocketLauncher r in currentLevel.launchers)
				{
					if (r.explosion != null)
						r.explosion.Update();
					if (r.Update(angel, currentLevel.walls, currentLevel.links, rocketLaunch))
					{
						r.explosion = new Explosion(new Vector2(r.rocket.hitBox.X, r.rocket.hitBox.Y), Color.OrangeRed, explosionTexture);
						explode.Play(0.5f, 0.0f, 0.0f);
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
				foreach (Bubbles b in currentLevel.bubbles)
				{
					b.Update();
					if (angel.hitBox.Intersects(b.hitBox))
					{
						if (b.motion.X > 0 && angel.motion.X < b.motion.X ||
							b.motion.X < 0 && angel.motion.X > b.motion.X ||
							b.motion.Y < 0 && angel.motion.Y > b.motion.Y ||
							b.motion.Y > 0 && angel.motion.Y < b.motion.Y)
							angel.motion += Vector2.Normalize(b.motion) * b.motion.Length() / 10;
					}
				}
				foreach (Starfish s in currentLevel.starfish)
				{
					s.Update(currentLevel.walls, currentLevel.links);
					if (s.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						continue;
					}
				}
				foreach (Fish s in currentLevel.fish)
				{
					s.Update(angel, currentLevel.walls, currentLevel.fish);
					if (s.hitBox.Intersects(angel.hitBox))
					{
						KillAngel();
						check = false;
						continue;
					}
				}
				#endregion

				#region Update timers and buttons
				bool oneisticking = false;
				foreach (Timer t in currentLevel.timers)
				{
					t.Update(currentLevel);
					if (t.activated && !t.loop)
					{
						loopTick.Play();
						oneisticking = true;
					}
				}
				if (!oneisticking)
					loopTick.Stop();

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
				if (check)
				{
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
								gemExplosion = null;
								deathExplosion = null;
								foreach (RocketLauncher r in currentLevel.launchers)
								{
									r.explosion = null;
									r.rocket.position = new Vector2(-10000, -10000);
									r.pause = 0;
									r.rocket.trail.RemoveRange(0, r.rocket.trail.Count);
								}
								foreach (Fish f in currentLevel.fish)
								{
									f.position = f.startPosition;
									f.motion = Vector2.Zero;
								}
								if (previousLevel.type != currentLevel.type)
									ChangeSong();
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
				}
				#endregion

				HUDCat.hit = false;
				HUDCat.Update();
			}
			else
			{
				if (goTime)
				{
					seconds = gameTime.TotalGameTime;
					goTime = false;
				}

				if (creditPosition.Y < -800)
					endGame = false;
				else
					creditPosition.Y -= 4;
			}

			base.Update(gameTime);
		}
		private void KillAngel()
		{
			deathExplosion = new Explosion(new Vector2(angel.hitBox.X + 8, angel.hitBox.Y + 8), Color.White, explosionTexture);
			die.Play();
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
						if ((currentLevel.coordinates.X + i >= levels.GetLength(0) || currentLevel.coordinates.Y + j >= levels.GetLength(1) ||
							currentLevel.coordinates.X + i < 0 || currentLevel.coordinates.Y + j < 0) || levels[(int)currentLevel.coordinates.X + i, (int)currentLevel.coordinates.Y + j] != null)
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
				foreach (Fish f in currentLevel.fish)
					f.Draw(spriteBatch);
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
				if (currentLevel.coordinates.X == 0 && currentLevel.coordinates.Y == 10)
					foreach (ElectricFence e in finalFences)
						e.Draw(spriteBatch);
				foreach (SpinningPlasma p in currentLevel.plasmas)
					p.Draw(spriteBatch);
				foreach (Starfish s in currentLevel.starfish)
					s.Draw(spriteBatch);
				foreach (Bubbles b in currentLevel.bubbles)
					b.Draw(spriteBatch);
				foreach (Cat c in currentLevel.cats)
					c.Draw(spriteBatch);
				if (deathExplosion != null)
					deathExplosion.Draw(spriteBatch);
				if (gemExplosion != null)
					gemExplosion.Draw(spriteBatch);
				foreach (RocketLauncher r in currentLevel.launchers)
					if (r.explosion != null)
						r.explosion.Draw(spriteBatch);
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
					"Except Fish Lair's Music's tune. That was by\n" +
					"              Alan Kahn\n\n" +
					"              Tools Used\n" +
					"       XNA 4.0 for Visual Studio 2010\n" +
					"              Sibelius 7\n" + 
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
