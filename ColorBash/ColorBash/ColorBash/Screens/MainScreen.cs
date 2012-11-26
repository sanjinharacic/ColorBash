using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace ColorBash
{
    /// <summary>
    /// Represents the main screen
    /// </summary>
    class MainScreen
    {
        // private instances
        private ColorPlateTrack _track;
        private GraphicsDevice _graphicsDevice; 
        private List<SpriteFont> _fonts;
        private ColorMap _colorMap;
        private ScreenChanged _screenChanged;
        
        // private vars
        private int _textColor;

        // public vars
        public int ApsoluteX = 0;
        public bool Swapping = false;

        // public delegates
        public delegate void ScreenChanged( Screen currentScreen, Screen nextScreen );

        /// <summary>
        /// Creates the main screen and assigns the provided instances to local refs
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="colorMap"></param>
        /// <param name="fonts"></param>
        /// <param name="method"></param>
        public MainScreen( GraphicsDevice graphicsDevice, ColorMap colorMap, List<SpriteFont> fonts, ScreenChanged method )
        {
            // subscribe to the delegate invocation list
            _screenChanged += method;

            // assign refs to the local instances
            _graphicsDevice = graphicsDevice;
            _fonts = fonts;
            _colorMap = colorMap;

            // create a new track for the main screen with the top difficulty
            _track = new ColorPlateTrack( graphicsDevice, colorMap, _fonts[ 0 ], Difficulty.Asian );
        }

        /// <summary>
        /// Update the main screen
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="textColor"></param>
        public void Update( GameTime gameTime, int textColor )
        {
            // assign the provided screen color
            _textColor = textColor;

            // handle input if the screen is not swapping
            if( !Swapping )
            {
                while( TouchPanel.IsGestureAvailable )
                {
                    GestureSample gs = TouchPanel.ReadGesture();
                    switch( gs.GestureType )
                    {
                        case GestureType.Tap:
                            // Get touch vals
                            int y = ( int )gs.Position.Y;
                            int x = ( int )gs.Position.X;

                            if( x > 50 && y > 50 && y < 200 )
                            {
                                // swap to the game screen
                                _screenChanged.Invoke( Screen.Main, Screen.Game );
                            }
                            else if( x > 50 && y > 350 && y < 400 )
                            {
                                // swap to the settings screen
                                _screenChanged.Invoke( Screen.Main, Screen.Settings );
                            }
                            else if( x > 50 && y > 400 )
                            {
                                // swap to the achievements screen
                                _screenChanged.Invoke( Screen.Main, Screen.TopScores );
                            }
                            break;
                    }
                }
            }

            // update the track
            _track.Update( _graphicsDevice, gameTime );
        }

        /// <summary>
        /// Draw the main screen items
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw( GameTime gameTime, SpriteBatch spriteBatch )
        {
            _track.Draw( spriteBatch );

            spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend );
            // draw the game start 
            spriteBatch.DrawString( _fonts[2],
                                           "Give it a shoot!",
                                           new Vector2( ApsoluteX + 50, 50 ),
                                           _colorMap.BlueLine[ _textColor ],
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );
            // draw the settings 
            spriteBatch.DrawString( _fonts[1],
                                           "Settings",
                                           new Vector2( ApsoluteX + 50, 350 ),
                                           _colorMap.GreenLine[ _textColor ],
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );
            // draw the achievements
            spriteBatch.DrawString( _fonts[1],
                                           "Top Scores",
                                           new Vector2( ApsoluteX + 50, 400 ),
                                           _colorMap.RedLine[ _textColor ],
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );
            spriteBatch.End();
        }
    }
}