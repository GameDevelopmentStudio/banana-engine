Banana Engine
=============

XNA based easy prototyping and quick development game engine.
It's in a very basic state as of yet. Feel free to contribute with amazing ideas & code.

This README is also in a very basic state, so don't actually believe anything you read here.

Employment
==========

You shouldn't use this yet, but if you must here are some directions.

1.  Create a new XNA Game Project & Solution.
2.  Clone Banana Engine into the solution folder. 
    The VS2010 project for Banana Engine will be created in the banana-engine folder.
3.  Add Banana Engine to the your game solution.
4.  Add a Reference to the Banana Engine project to your game project.
5.  Add the resources from banana-engine/Resources to the Content project of your game solution.
6.  You should be ready to start coding in a fun and easy fashion (lies, all lies, I tell you!)

Quick start
===========

Asuming you have prepared the game solution as explained before, here are some quick start steps to get you started.

1.  Derive bGame and override initSettings() and Initialize() to suit your needs.
<pre>
	class Game : bGame
    {
        Level level;

        protected override void initSettings()
        {
			// TODO: Modify initial settings
            base.initSettings();
        }

        protected override void Initialize()
        {
            // Create Initial Level
            level = new Level();
            changeWorld(level);

            base.Initialize();
        }
    }
</pre>
2.  Create a new bGameState derivate to model your game levels and
<pre>
	class Level : bGameState
	{
		public Level() : base()
		{
		}
	}
</pre>
3.  Override its init() method to create your entities categories and add some entities into those:
<pre>
	public override void init()
	{
		entities.Add("solids", new List<bEntity>());
		_add(new TestEntity(0, 0), "solids");
	}
</pre>
4.  Override its update() method to update your entity categories in the correct order. Also handle any required Level scoped processing: 
<pre>
	public override void update()
	{
		base.update(dt);

		foreach (bEntity e in entities["solids"])
			e.update();
	}
</pre>
5.  Override its render() method to handle de drawing of the entities. Also add any Level related drawing routines:
<pre>
	public override void render(GameTime dt, SpriteBatch sb, Matrix matrix)
	{
		base.render(dt, sb, matrix);

		foreach (bEntity ge in entities["solids"])
			ge.render(dt, sb);
	}
</pre>
6.  Write a suitable _add() method to handle entity inclusion into the GameState. It can be as complex as required or as simple as this:
<pre>
	protected override bool _add(bEntity e, string category)
	{
		entities[category].Add(e);

		return base._add(e, category);
	}
</pre>
7. Create the new bGameEntity derivate and override its init(), update() and draw() methods:
<pre>
	class TestEntity : bEntity
    {
        bStamp graphic;

        public TestEntity(int x, int y)
            : base(x, y)
        {
        }

        public override void init()
        {
            graphic = new bStamp(game.Content.Load<Texture2D>("rect"));
            base.init();
        }

        public override void update()
        {
            base.update();

            graphic.update();
        }

        public override void render(GameTime dt, SpriteBatch sb)
        {
            base.render(dt, sb);

            graphic.render(sb, pos);
        }
    }
</pre>
8. That's it. Try running the game, fix any minor error that rises and [report the bigger ones](https://github.com/GameDevelopmentStudio/banana-engine/issues).
9. Have fun

Closing words
=============

This is so undeveloped that we have no closing words yet. Bear with it.