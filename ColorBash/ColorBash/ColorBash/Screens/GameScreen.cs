using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace ColorBash
{
    /// <summary>
    /// Represents the Game screen
    /// </summary>
    public class GameScreen
    {
        // private vars
        private bool _pause = true;
        private bool _started = false;
        private float _ElapsedTime, _TotalFrames, _Fps;
        private TimeSpan _lastInput;

        // private instances
        private GraphicsDevice _graphicsDevice;
        private ColorMap _colorMap;
        private List<SpriteFont> _fonts;
        private ScreenChanged _screenChanged;

        // public vars
        public int ApsoluteX = 0;
        public bool Swapping = false;

        // public instances
        public ColorPlateTrack Track;

        // public delegates        
        public delegate void ScreenChanged( Screen currentScreen, Screen nextScreen );

        /// <summary>
        /// Instantiates the game screen and sets up the base values
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="colorMap"></param>
        /// <param name="fonts"></param>
        /// <param name="screenChanged"></param>
        public GameScreen( GraphicsDevice graphicsDevice, ColorMap colorMap, List<SpriteFont> fonts, ScreenChanged screenChanged )
        {
            // subscribe to the screen change delegate
            _screenChanged += screenChanged;

            // assign the local refs
            _graphicsDevice = graphicsDevice;
            _colorMap = colorMap;
            _fonts = fonts;

            // create the track
            Track = new ColorPlateTrack( _graphicsDevice, colorMap, _fonts[ 0 ], Difficulty.Pro /*alter to populate from settings*/ );
        }

        /// <summary>
        /// Updates the game screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {
            // collect gestures fo the state if the game is not started and the screen is not swapping
            if( !_started && !Swapping )
            {
                while( TouchPanel.IsGestureAvailable )
                {
                    GestureSample gs = TouchPanel.ReadGesture();
                    switch( gs.GestureType )
                    {
                        case GestureType.Tap:
                            _started = true;
                            _pause = false;
                            _lastInput = gameTime.TotalGameTime;
                            break;
                    }
                }

                return;
            }

            // collect gestures for an in progress game
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

                            // resolve location
                            if( y > Track.PlateMetrics.Height + 200 )
                            {
                                if( x < 150 && _lastInput.TotalSeconds + 1d < gameTime.TotalGameTime.TotalSeconds )
                                {
                                    _lastInput = gameTime.TotalGameTime;
                                    _pause = !_pause;
                                }
                            }
                            else if( y < 200 && !_pause )
                            {
                                if( x < 768 )
                                {
                                    _colorMap.HandleInput( x, y );
                                }
                            }
                            else if( y < Track.PlateMetrics.Height + 200 && !_pause )
                            {
                                // select or lock a plate
                                Track.HandleInput( x, y );
                            }
                            break;
                    }
                }

                // Pause state, no need to update further
                if( _pause )
                    return;

                // update the track
                Track.Update( _graphicsDevice, gameTime );

                // update the framerate counter
                _ElapsedTime += ( float )gameTime.ElapsedGameTime.TotalSeconds;
                _TotalFrames++;

                if( _ElapsedTime >= 1.0f )
                {
                    _Fps = _TotalFrames;
                    _TotalFrames = 0;
                    _ElapsedTime = 0;
                }
            }
        }

        /// <summary>
        /// Draws the game screen related sprites
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw( GameTime gameTime, SpriteBatch spriteBatch )
        {
            // draw the color map
            _colorMap.Draw( spriteBatch );

            // draw the track
            Track.Draw( spriteBatch );

            // draw the text
            spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend );

            // if the game is not started inform the user to tap to start
            if( !_started )
            {
                spriteBatch.DrawString( _fonts[ 2 ],
                                               "Tap to Start",
                                               new Vector2( ApsoluteX + 160, 200 ),
                                               Color.AntiqueWhite,
                                               0f,
                                               Vector2.Zero,
                                               1.0f,
                                               SpriteEffects.None,
                                               0f );
            }
            else
            {
                // draw the pause text
                spriteBatch.DrawString( _fonts[ 1 ],
                                           "Pause",
                                           new Vector2( ApsoluteX + 10, 430 ),
                                           Color.Azure,
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );
                // draw the current fps
                spriteBatch.DrawString( _fonts[ 0 ],
                                               "FPS = " + _Fps.ToString(),
                                               new Vector2( ApsoluteX + 200, 450 ),
                                               Color.Red );
                // draw the current track score
                spriteBatch.DrawString( _fonts[ 1 ],
                                               Track.TrackScore.ToString(),
                                               new Vector2( ApsoluteX + 600, 430 ),
                                               Color.AntiqueWhite,
                                               0f,
                                               Vector2.Zero,
                                               1.0f,
                                               SpriteEffects.None,
                                               0f );
            }
            spriteBatch.End();
        }
    }
}
