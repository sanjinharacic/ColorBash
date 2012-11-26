using System.Collections.Generic;
using ColorBash.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColorBash
{
    /// <summary>
    /// Represents the screen management type
    /// </summary>
    public class ScreenManager
    {
        // private vars
        private Screen _currentScreen;
        private Screen _nextScreen;

        private int textColor = 0;
        private bool textColorFlag = true;
        private int _screenOffset = 1600;
        private bool _swappingScreens = false;
        private int _swapVelocity = 50;
        private bool _swappingOut = false;

        // private instances
        private MainScreen _mainScreen;
        private GameScreen _gameScreen;
        private SettingsScreen _settingsScreen;
        private TopScoresScreen _topScoresScreen;
        private ColorMap _colorMap;
        private GraphicsDevice _graphicsDevice;
        private List<SpriteFont> _fonts;
        private Texture2D _bgT;

        /// <summary>
        /// Handles the screen changing flags
        /// </summary>
        /// <param name="currentScreen"></param>
        /// <param name="nextScreen"></param>
        private void ScreenChanged( Screen currentScreen, Screen nextScreen )
        {
            if( _swappingScreens )
                return;

            _swappingScreens = true;
            _currentScreen = currentScreen;
            _nextScreen = nextScreen;
            _mainScreen.Swapping = true;
            _gameScreen.Swapping = true;
            _settingsScreen.Swapping = true;
            _topScoresScreen.Swapping = true;
        }

        /// <summary>
        /// Creates the Screen manager and inits the base data
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="colorMap"></param>
        /// <param name="fonts"></param>
        public ScreenManager( GraphicsDevice graphicsDevice, ColorMap colorMap, List<SpriteFont> fonts )
        {
            // assign local refs
            _graphicsDevice = graphicsDevice;
            _colorMap = colorMap;
            _fonts = fonts;

            // set the default screen
            _currentScreen = Screen.Main;
            _nextScreen = Screen.None;

            // create all screens
            _mainScreen = new MainScreen( _graphicsDevice, _colorMap, _fonts, ScreenChanged );
            _gameScreen = new GameScreen( _graphicsDevice, _colorMap, _fonts, ScreenChanged );
            _settingsScreen = new SettingsScreen( _graphicsDevice, _colorMap, _fonts, ScreenChanged );
            _topScoresScreen = new TopScoresScreen( _graphicsDevice, _colorMap, _fonts, ScreenChanged );

            // set up the swapping texture
            _bgT = ItemFactory.CreateFilledTexture( _graphicsDevice, 1, 480, Color.Black );
        }

        /// <summary>
        /// Update the screen manager and the currently displayed screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {
            // Color changing text logic
            if( textColor == _colorMap.BlueLine.Count - 1 )
                textColorFlag = false;

            if( textColor == 0 )
                textColorFlag = true;

            if( textColorFlag )
                textColor++;
            else
                textColor--;

            // handle screen swapping state
            if( _swappingScreens )
            {
                // move the swap screen out
                if( _swappingOut )
                    _screenOffset += _swapVelocity;
                else
                    // move the swap screen in
                    _screenOffset -= _swapVelocity;

                // swap the screens if the screen is covered with the swapper layer
                if( _screenOffset == 0 )
                {
                    _swappingOut = true;
                    _currentScreen = _nextScreen;
                }

                // reset the swapper flags and position if the swapping completed
                if( _screenOffset == 1600 && _swappingOut )
                {
                    _swappingScreens = false;
                    _swappingOut = false;
                    _screenOffset = 1600;

                    _mainScreen.Swapping = false;
                    _gameScreen.Swapping = false;
                    _settingsScreen.Swapping = false;
                    _topScoresScreen.Swapping = false;
                }
            }
            else
            {
                // update the current screen
                switch( _currentScreen )
                {
                    case Screen.Main:
                        _mainScreen.Update( gameTime, textColor );
                        break;

                    case Screen.Game:
                        _gameScreen.Update( gameTime );
                        break;

                    case Screen.Settings:
                        _settingsScreen.Update( gameTime );
                        break;

                    case Screen.TopScores:
                        _topScoresScreen.Update( gameTime, textColor );
                        break;
                }
            }
        }

        /// <summary>
        /// draw the selected screen and/or the swapping
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw( GameTime gameTime, SpriteBatch spriteBatch )
        {
            // regular screen draw invocation
            switch( _currentScreen )
            {
                case Screen.Main:
                    _mainScreen.Draw( gameTime, spriteBatch );
                    break;
                case Screen.Game:
                    _gameScreen.Draw( gameTime, spriteBatch );
                    break;
                case Screen.Settings:
                    _settingsScreen.Draw( gameTime, spriteBatch );
                    break;
                case Screen.TopScores:
                    _topScoresScreen.Draw( gameTime, spriteBatch );
                    break;
            }

            // handle the swapping drawing
            if( _swappingScreens )
            {
                spriteBatch.Begin( SpriteSortMode.FrontToBack, BlendState.AlphaBlend );

                // swapper texture visibility
                float alpha = 0;

                // create a gradient swap screen and draw it on the related position
                for( int i = 0; i < 1651; i++ )
                {
                    spriteBatch.Draw( _bgT, new Rectangle( 800 + i - ( 1600 - _screenOffset ), 0, 1, 480 ), new Color( new Vector4( 1f, 1f, 1f, alpha ) ) );
                    alpha += 0.0015f;
                }

                spriteBatch.End();
            }
        }
    }
}