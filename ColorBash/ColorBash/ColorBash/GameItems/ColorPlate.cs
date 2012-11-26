using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ColorBash.Enums;

namespace ColorBash
{
    /// <summary>
    /// Represents the color plate type
    /// </summary>
    public class ColorPlate
    {
        // private fields
        private Texture2D placeholder;
        private Texture2D wrapper;
        private Texture2D texture;
        private SpriteFont font;

        private Rectangle placeholderMetrics;
        private Rectangle wrapperMetrics;
        private Shape shape;

        private bool scoreCalculated = false;
        private bool innerColorChanged = false;
        private int placeholderOffset = 10;
        private int scorePositionX;
        private float rotation;
        
        private DrawMode drawMode = DrawMode.Regular;

        // public fields
        public List<Color> ColorPalette;

        public Rectangle Metrics;
        public Color Color;
        public Color _innerColor = Color.Black;
        public Color InnerColor
        {
            get
            {
                return _innerColor;
            }

            set
            {
                if( value != Color.Black && !Locked )
                {
                    innerColorChanged = true;
                    _innerColor = value;
                }
            }
        }

        public TimeSpan SelectionTime;
        public bool Selected;
        public bool Locked;
        public bool Visible = true;
        public int Velocity = 1;
        public int Score = 0;

        /// <summary>
        /// Creates a color plate with wrapper, placeholder and other default prefs set
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="metrics"></param>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="palette"></param>
        /// <param name="drawMode"></param>
        public ColorPlate( GraphicsDevice graphicsDevice, Rectangle metrics, Color color, SpriteFont font, List<Color> palette, DrawMode drawMode, int velocity )
        {
            // assign provided vars to instance refs
            this.font = font;
            this.ColorPalette = palette;
            this.drawMode = drawMode;
            this.rotation = 0f;
            this.Velocity = velocity;

            // create placeholder and wrapper based on metrics
            placeholderMetrics = new Rectangle( metrics.X + placeholderOffset / 2, metrics.Y + placeholderOffset / 2, metrics.Width - placeholderOffset, metrics.Height - placeholderOffset );
            wrapperMetrics = new Rectangle( metrics.X - placeholderOffset / 2, metrics.Y - placeholderOffset / 2, metrics.Width + placeholderOffset, metrics.Height + placeholderOffset );

            // assign the textures and colors
            this.placeholder = new Texture2D( graphicsDevice, placeholderMetrics.Width, placeholderMetrics.Height );
            this.wrapper = new Texture2D( graphicsDevice, wrapperMetrics.Width, wrapperMetrics.Height );
            this.Metrics = metrics;
            this.Color = color;
            this.texture = new Texture2D( graphicsDevice, metrics.Width, metrics.Height );

            // create the texture colors for the main texture
            // fill the main texture
            this.texture.SetData<Color>( ShapeFill(graphicsDevice, metrics.Width, metrics.Height, color ) );

            // create the texture colors for the placeholder texture
            Color[] bg1 = new Color[ placeholderMetrics.Width * placeholderMetrics.Height ];
            for( int c = 0; c < bg1.Length; c++ )
                bg1[ c ] = InnerColor;

            // fill the placeholder texture
            this.placeholder.SetData<Color>( bg1 );

            // create the texture colors for the wrapper texture
            Color[] bg2 = new Color[ wrapperMetrics.Width * wrapperMetrics.Height ];
            for( int c = 0; c < bg2.Length; c++ )
                bg2[ c ] = Color.White;

            // fill the wrapper texture
            this.wrapper.SetData<Color>( bg2 );
        }

        /// <summary>
        /// Update the palette metrics, position and score
        /// </summary>
        /// <param name="gametime"></param>
        public void Update( GameTime gametime )
        {
            // The time since Update was called last.
            float elapsed = ( float )gametime.ElapsedGameTime.TotalSeconds;

            // Calculate rotation angle based on time elapsed
            float circle = MathHelper.Pi * 2;
            rotation += elapsed % circle;           

            // Update the velocity
            Metrics.X -= Velocity;
            placeholderMetrics.X -= Velocity;
            wrapperMetrics.X -= Velocity;

            // Shrink the plate if the it passes the end line, lock it and push the calculated score to the screen
            if( Metrics.X < 0 )
            {
                Metrics.Height--;
                Metrics.Width--;
                placeholderMetrics.Height--;
                placeholderMetrics.Width--;
                wrapperMetrics.Height--;
                wrapperMetrics.Width--;
                scorePositionX += 10;
                Locked = true;
            }

            // if the metrics reach zero mark the plate as invisible (means will be deleted from the plate collection)
            if( placeholderMetrics.Height == 0 || placeholderMetrics.Width == 0 )
            {
                Visible = false;
            }

            // if the inner fill color changed we need to recalculate the score and set the fill color
            if( innerColorChanged )
            {
                // create the color array
                Color[] bg1 = new Color[ placeholderMetrics.Width * placeholderMetrics.Height ];
                for( int c = 0; c < bg1.Length; c++ )
                    bg1[ c ] = _innerColor;
                
                // try filling the placeholder with the selected color
                try
                {
                    this.placeholder.SetData<Color>( bg1 );
                }
                catch
                {
                }

                // set the score to max value
                Score = 768;

                // get the color r,g and b value differences to calculate the final score
                Score -= Math.Abs( this.Color.R - _innerColor.R );
                Score -= Math.Abs( this.Color.G - _innerColor.G );
                Score -= Math.Abs( this.Color.B - _innerColor.B );

                // mark the plate as calculated and flag it as innerColor change completed (return to false)
                scoreCalculated = true;
                innerColorChanged = false;
            }
        }

        /// <summary>
        /// Draws the plate, placeholder, score and wrapper according to plate state
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw( SpriteBatch spriteBatch )
        {
            // resolve the draw mode
            switch( drawMode )
            {
                case DrawMode.Regular:
                    Regular( spriteBatch );
                    break;

                case DrawMode.CenterRotate:
                    CenterRotate( spriteBatch );
                    break;

                case DrawMode.TopLeftCenterRotate:
                    TopLeftCenterRotate( spriteBatch );
                    break;

                case DrawMode.TopLeftRotate:
                    TopLeftRotate( spriteBatch );
                    break;

                case DrawMode.BotLeft:
                    BotLeftRotate( spriteBatch );
                    break;

                case DrawMode.BotRight:
                    BotRightRotate( spriteBatch );
                    break;
            }

            // If the plate is locked and the calculated score is above 0 we draw the score in a fancy way by scaling it in case of grater values
            if( Locked && scoreCalculated && Score != 0 )
            {
                spriteBatch.Begin();
                spriteBatch.DrawString( font,
                                               Score > 256 ? Score > 512 ? Score.ToString() + "!!!" : Score.ToString() + "!" : Score.ToString(),
                                               new Vector2( scorePositionX - 30, 400 ),
                                               this.ColorPalette[ 767 - Math.Abs( placeholderMetrics.Width ) ],
                                               0f,
                                               Vector2.Zero,
                                               Score > 256 ? Score > 512 ? 2.0f : 1.5f : 1.0f,
                                               SpriteEffects.None,
                                               0f );
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw a plate with a static border
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void Regular( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            if( Selected )
                spriteBatch.Draw( wrapper, wrapperMetrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( texture, Metrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( placeholder, placeholderMetrics, Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Create a plate with rotating border around the center origin
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void CenterRotate( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            if( Selected )
                spriteBatch.Draw( wrapper, wrapperMetrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( texture, new Rectangle( Metrics.X + Metrics.Width / 2, Metrics.Y + Metrics.Height / 2, Metrics.Width, Metrics.Height ), null, Color.White, rotation, new Vector2( Metrics.Width / 2, Metrics.Height / 2 ), SpriteEffects.None, 0f );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( placeholder, placeholderMetrics, Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Create a plate with rotating border at the top left corner rotating around it
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void TopLeftCenterRotate( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            if( Selected )
                spriteBatch.Draw( wrapper, wrapperMetrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( texture, new Rectangle( Metrics.X + Metrics.Width / 2, Metrics.Y + Metrics.Height / 2, Metrics.Width, Metrics.Height ), null, Color.White, rotation, new Vector2( Metrics.Width, Metrics.Height ), SpriteEffects.None, 0f );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( placeholder, placeholderMetrics, Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Create a plate with rotating border at the top left corner rotating around it
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void TopLeftRotate( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            if( Selected )
                spriteBatch.Draw( wrapper, wrapperMetrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( texture, Metrics, null, Color.White, rotation, new Vector2( Metrics.Width / 2, Metrics.Height / 2 ), SpriteEffects.None, 0f );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( placeholder, placeholderMetrics, Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Create a plate with rotating border at the top left corner rotating around it
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void BotLeftRotate( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            if( Selected )
                spriteBatch.Draw( wrapper, wrapperMetrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( texture, new Rectangle( Metrics.X + Metrics.Width, Metrics.Y + Metrics.Height, Metrics.Width, Metrics.Height ), null, Color.White, rotation, new Vector2( Metrics.Width / 2, Metrics.Height / 2 ), SpriteEffects.None, 0f );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( placeholder, placeholderMetrics, Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Create a plate with rotating border at the top left corner rotating around it
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void BotRightRotate( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            if( Selected )
                spriteBatch.Draw( wrapper, wrapperMetrics, Color.White );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( texture, new Rectangle( Metrics.X, Metrics.Y + Metrics.Height, Metrics.Width, Metrics.Height ), null, Color.White, rotation, new Vector2( Metrics.Width / 2, Metrics.Height / 2 ), SpriteEffects.None, 0f );
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw( placeholder, placeholderMetrics, Color.White );
            spriteBatch.End();
        }

        private Color[] ShapeFill( GraphicsDevice graphicsDevice, int width, int height, Color color )
        {
            Random r = new Random();

            switch( r.Next( 1, 3 ) )
            {
                case ( int )Shape.Random:
                    return Random( graphicsDevice, width, height, color );
                default:
                    return Rectangle( graphicsDevice, width, height, color );
            }
        }

        private Color[] Rectangle( GraphicsDevice graphicsDevice, int width, int height, Color color )
        {
            Color[] bgColor = new Color[ width * height ];

            for( int i = 0; i < bgColor.Length; i++ )
            {
                bgColor[ i ] = color;
            }

            return bgColor;
        }

        private Color[] Random( GraphicsDevice graphicsDevice, int width, int height, Color color )
        {
            Color[] bgColor = new Color[ width * height ];
            byte[] b = new byte[ width * height ];
            Random r = new Random();
            r.NextBytes( b );

            for( int i = 0; i < bgColor.Length; i++ )
            {
                if( b[ i ] > 248 )
                    bgColor[ i ] = color;
            }

            return bgColor;
        }
    }
}
