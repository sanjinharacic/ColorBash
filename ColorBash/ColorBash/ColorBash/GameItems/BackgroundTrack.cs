using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColorBash
{
    /// <summary>
    /// Represents the outerspace-like moving background
    /// </summary>
    public class BackgroundTrack
    {
        private Texture2D mainBackground;
        private Texture2D mainBackground2;
        private GraphicsDevice graphicsDevice;

        // background textures starting positions
        private int bgX = 0;
        private int bgX2 = 850;

        /// <summary>
        /// Gets or Sets the background velocity
        /// </summary>
        public int Velocity = 2;

        /// <summary>
        /// Initializes the background with random stars
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public BackgroundTrack( GraphicsDevice graphicsDevice )
        {
            this.graphicsDevice = graphicsDevice;

            // create bg textures
            mainBackground = new Texture2D( graphicsDevice, 850, 480 );
            mainBackground2 = new Texture2D( graphicsDevice, 850, 480 );

            // create arrays to fill the textures
            Color[] bgColor = new Color[ 850 * 480 ];
            byte[] br = new byte[ 850 * 480 ];
            Random r = new Random();
            r.NextBytes( br );

            // the algorithm fills only the pixels where the random-ed value of a byte is larger than 252
            for( int i = 0; i < bgColor.Length; i++ )
            {
                if( br[ i ] > 252 )
                    bgColor[ i ] = Color.WhiteSmoke;
            }

            // fill the textures
            mainBackground.SetData( bgColor );
            mainBackground2.SetData( bgColor );
        }

        /// <summary>
        /// Moves the background according to it's velocity
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {
            // The screen textures start out at different positions and move independantly to the left 
            // until they pass the complete screen and then are moved to the right side of the screen
            // to repeat the complete cycle over and over

            // move the first texture until it passes the whole screen, then position it to the back
            if( bgX < -850 + Velocity )
                bgX = 849;
            else
                bgX -= Velocity;

            // move the second texture until it passes the whole screen, then position it to the back
            if( bgX2 < -850 + Velocity )
                bgX2 = 849;
            else
                bgX2 -= Velocity;
        }

        /// <summary>
        /// Draws the background on the calculated location
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw( GameTime gameTime, SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            spriteBatch.Draw( mainBackground, new Rectangle( bgX, 0, 850, 480 ), Color.White );
            spriteBatch.Draw( mainBackground2, new Rectangle( bgX2, 0, 850, 480 ), Color.White );
            spriteBatch.End();
        }
    }
}
