using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace ColorBash
{
    /// <summary>
    /// Represents the settings screen
    /// </summary>
    class SettingsScreen
    {
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
        /// Constucts and assigns the basic data
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="colorMap"></param>
        /// <param name="fonts"></param>
        /// <param name="screenChanged"></param>
        public SettingsScreen( GraphicsDevice graphicsDevice, ColorMap colorMap, List<SpriteFont> fonts, ScreenChanged screenChanged )
        {
            // subscribe to the delegate invocation list
            _screenChanged += screenChanged;

            // assign references to the local instance refs
            _graphicsDevice = graphicsDevice;
            _colorMap = colorMap;
            _fonts = fonts;
        }

        /// <summary>
        /// Update the settings screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {
            // handle the input if the screen is not in the swapping state
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
                            if( x > 50 && y > 400 )
                            {
                                _screenChanged.Invoke( Screen.Settings, Screen.Main );
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Draw the settings screen
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw( GameTime gameTime, SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            // Draw the back option text
            spriteBatch.DrawString( _fonts[ 1 ],
                                           "Back",
                                           new Vector2( ApsoluteX + 50, 400 ),
                                           Color.AntiqueWhite,
                                           0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0f );
            spriteBatch.End();
        }
    }
}
