using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColorBash
{
    /// <summary>
    /// Represents the color plate track
    /// </summary>
    public class ColorPlateTrack
    {
        // private constants
        private const int COLOR_COUNT = 2309;
        private const int SCREEN_WIDTH = 800;

        // private instance vars
        private Random random;
        private SpriteFont font;
        private GameTime gameTime;

        // public instances
        public List<ColorPlate> ColorPlates;
        public ColorMap TrackColorMap;

        // public vars
        public Rectangle PlateMetrics = new Rectangle()
        {
            X = 800,
            Y = 210,
            Width = 200,
            Height = 200
        };
        public int Velocity;
        public int PlateGap = 10;
        public int TrackScore = 0;
        public Difficulty TrackDifficulty = Difficulty.Beginner;

        /// <summary>
        /// Creates the color plate track and assigned the provided data
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="colorMap"></param>
        /// <param name="velocity"></param>
        /// <param name="font"></param>
        /// <param name="difficulty"></param>
        public ColorPlateTrack( GraphicsDevice graphicsDevice, ColorMap colorMap, SpriteFont font, Difficulty difficulty )
        {
            // assign res to local refs
            this.TrackDifficulty = difficulty;
            this.font = font;
            this.Velocity = (int)difficulty;
            this.TrackColorMap = colorMap;

            // add the first plate to the track
            ColorPlates = new List<ColorPlate>();
            ColorPlates.Add( CreateColorPlate( graphicsDevice ) );
        }

        /// <summary>
        /// Update the color plate track and add a new color plate if necessary
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="gametime"></param>
        public void Update( GraphicsDevice graphicsDevice, GameTime gametime )
        {
            // update the track gametime
            gameTime = gametime;

            // check for available space on the right side and incorporate the gap, if matched add a new plate
            if( SCREEN_WIDTH > ( ColorPlates.Last<ColorPlate>().Metrics.X + ( PlateMetrics.Width + PlateGap ) ) )
            {
                ColorPlates.Add( CreateColorPlate( graphicsDevice ) );
            }

            // Update each plate
            for( int i = ColorPlates.Count - 1; i >= 0; i-- )
            {
                // Each difficulty has its own calculations related to velocity
                switch( TrackDifficulty )
                {
                    case Difficulty.Beginner:// keep the velocity
                        break;
                    case Difficulty.Asian:// increase the speed as seconds pass
                        if( gametime.TotalGameTime.Seconds > 0 && gametime.TotalGameTime.Seconds > ColorPlates[ i ].Velocity )
                            ColorPlates[ i ].Velocity++;
                        break;
                }

                // Update the current plate position and display
                ColorPlates[ i ].Update( gametime );

                if( ColorPlates[ i ].Selected && ColorPlates[ i ].SelectionTime.Milliseconds + 100 < gameTime.TotalGameTime.Milliseconds )
                {
                    ColorPlates[ i ].Selected = false;
                    ColorPlates[ i ].SelectionTime = new TimeSpan();
                }

                // Remove the invisible plates and update the track score with the removed track score
                if( !ColorPlates[ i ].Visible )
                {
                    TrackScore += ColorPlates[ i ].Score;
                    ColorPlates.RemoveAt( i );
                    i--;
                }
            }
        }

        /// <summary>
        /// Draws the track by drawing each plate
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw( SpriteBatch spriteBatch )
        {
            foreach( var plate in ColorPlates )
            {
                plate.Draw( spriteBatch );
            }
        }

        /// <summary>
        /// Updates the plate status according to the input
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void HandleInput( int x, int y )
        {
            // find the plate at x,y and mark is as selected
            foreach( var plate in ColorPlates )
            {
                if( plate.Metrics.X < x && plate.Metrics.X + plate.Metrics.Width > x &&
                    plate.Metrics.Y < y && plate.Metrics.Y + plate.Metrics.Height > y )
                {
                    plate.Selected = true;
                    plate.SelectionTime = gameTime.TotalGameTime;
                    plate.InnerColor = TrackColorMap.SelectedColor;
                }
                else
                {
                    plate.Selected = false;
                }                
            }
        }

        /// <summary>
        /// Create a new color plate based on the track settings
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        private ColorPlate CreateColorPlate( GraphicsDevice graphicsDevice )
        {
            // Generate random number for color in all color
            random = new Random();
            int colorPalette = random.Next( 2 );
            int colorPosition = random.Next( 768 );

            // Generate a random number for the plate size
            int newWidth = random.Next( 50, 200 );
            int newHeight = random.Next( 50, 200 );
            Rectangle newPlateMetrics = new Rectangle( PlateMetrics.X, PlateMetrics.Y, newWidth, newHeight );

            // init the color palette
            Color color = Color.White;
            List<Color> selectedPalette = new List<Color>();

            // assign the palette selection
            switch( colorPalette )
            {
                case 0:
                    selectedPalette = TrackColorMap.RedLine;
                    break;
                case 1:
                    selectedPalette = TrackColorMap.GreenLine;
                    break;
                case 2:
                    selectedPalette = TrackColorMap.BlueLine;
                    break;
            }

            // resolve drawmode depending on difficulty
            DrawMode drawMode = DrawMode.Regular;
            switch( TrackDifficulty )
            {
                case Difficulty.Beginner:// keep the default
                    break;
                case Difficulty.Intermediate:
                    drawMode = ( DrawMode )random.Next( 1, 2 );
                    break;
                case Difficulty.Advanced:
                    drawMode = ( DrawMode )random.Next( 1, 3 );
                    break;
                case Difficulty.Expert:
                    drawMode = ( DrawMode )random.Next( 1, 4 );
                    break;
                case Difficulty.Pro:
                    drawMode = ( DrawMode )random.Next( 1, 5 );
                    break;
                case Difficulty.Asian:
                    drawMode = ( DrawMode )random.Next( 1, 6 );
                    break;
            }

            // return the new color plate
            return new ColorPlate( graphicsDevice, newPlateMetrics, selectedPalette[ colorPosition ], font, selectedPalette, drawMode, Velocity );
        }
    }
}