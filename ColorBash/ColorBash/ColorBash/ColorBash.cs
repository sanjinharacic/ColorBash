using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ColorBash
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ColorBash : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // private instances
        private List<SpriteFont> _fonts;
        private ColorMap _colorMap;
        private BackgroundTrack _background;
        private ScreenManager _screenManager;

        /// <summary>
        /// Main game type constructor
        /// </summary>
        public ColorBash()
        {
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks( 333333 );

            // Support Tap
            TouchPanel.EnabledGestures =
                GestureType.Tap;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the required fonts
            _fonts = new List<SpriteFont>();
            _fonts.Add( Content.Load<SpriteFont>( "Motorwerk24" ) );
            _fonts.Add( Content.Load<SpriteFont>( "Motorwerk40" ) );
            _fonts.Add( Content.Load<SpriteFont>( "Motorwerk54" ) );

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch( GraphicsDevice );

            // Create a new color map
            _colorMap = new ColorMap( GraphicsDevice );

            // Create a new background
            _background = new BackgroundTrack( GraphicsDevice );

            // Create a new screen manager with the given color map and the loaded fonts 
            _screenManager = new ScreenManager( GraphicsDevice, _colorMap, _fonts );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            // Allows the game to exit
            if( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
                this.Exit();

            // Update the background
            _background.Update( gameTime );

            // update the screen
            _screenManager.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            // clear the screen and fill it with a black color as a base
            GraphicsDevice.Clear( Color.Black );

            // draw the current screen 
            _screenManager.Draw( gameTime, spriteBatch );            

            // draw the background
            _background.Draw( gameTime, spriteBatch );

            base.Draw( gameTime );
        }
    }
}