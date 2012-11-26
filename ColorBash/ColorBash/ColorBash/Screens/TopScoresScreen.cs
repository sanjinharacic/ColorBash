using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace ColorBash
{
    /// <summary>
    /// Represents the Achievements screen
    /// </summary>
    public class TopScoresScreen
    {
        // private vars
        private int _textColor = 200;

        // private instances
        private ScreenChanged _screenChanged;
        private GraphicsDevice _graphicsDevice;
        private ColorMap _colorMap;
        private List<SpriteFont> _fonts;

        // public vars
        public int ApsoluteX;
        public bool Swapping = false;

        // public delegates
        public delegate void ScreenChanged( Screen currentScreen, Screen nextScreen );

        /// <summary>
        /// Creates the achievements screen and assigns the basic data
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="colorMap"></param>
        /// <param name="fonts"></param>
        /// <param name="screenChanged"></param>
        public TopScoresScreen( GraphicsDevice graphicsDevice, ColorMap colorMap, List<SpriteFont> fonts, ScreenChanged screenChanged )
        {
            // subscribe the delegate
            _screenChanged += screenChanged;

            // assign the refs to the local instances
            _graphicsDevice = graphicsDevice;
            _colorMap = colorMap;
            _fonts = fonts;
        }

        /// <summary>
        /// Updates the achievements screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime, int textColor )
        {
            if(textColor > 200 && textColor < 550)
                _textColor = textColor;
            // handle input if the swapping state is not active
            if( !Swapping )
            {
                while( TouchPanel.IsGestureAvailable )
                {
                    GestureSample gs = TouchPanel.ReadGesture();
                    // Get touch vals
                    int y = ( int )gs.Position.Y;
                    int x = ( int )gs.Position.X;

                    switch( gs.GestureType )
                    {
                        case GestureType.Tap:
                            // resolve location
                            if( x < 200 && y > 400 )// back button tapped
                            {
                                _screenChanged.Invoke( Screen.TopScores, Screen.Main );
                            }
                            else if( x < 400 && y > 400 )// top 100
                            {
                                // set top 100 visibility flag
                            }
                            else if( x > 400 && y > 400 )// player ranking
                            {
                                // set player ranking visibility flag
                            }
                            break;
                        case GestureType.VerticalDrag:

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Draw the achiements screen
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw( GameTime gameTime, SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            // draw the back option
            spriteBatch.DrawString( _fonts[ 1 ],
                                           "Back",
                                           new Vector2( ApsoluteX + 50, 400 ),
                                           Color.AntiqueWhite,
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );

            // draw the Top 100 option
            spriteBatch.DrawString( _fonts[ 1 ],
                                           "Top 100",
                                           new Vector2( ApsoluteX + 200, 400 ),
                                           _colorMap.BlueLine[ _textColor ],
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );

            // player ranking
            spriteBatch.DrawString( _fonts[ 1 ],
                                           "Player Ranking",
                                           new Vector2( ApsoluteX + 400, 400 ),
                                           _colorMap.BlueLine[ _textColor ],
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );
            spriteBatch.End();
        }
    }
}