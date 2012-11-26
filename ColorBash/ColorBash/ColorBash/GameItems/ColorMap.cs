using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColorBash
{
    /// <summary>
    /// Represents the Color map type
    /// </summary>
    public class ColorMap
    {
        // private instances
        private Texture2D _colorMapRed;
        private Texture2D _colorMapGreen;
        private Texture2D _colorMapBlue;
        private Texture2D _colorMapSelectionMark;
        private GraphicsDevice _graphicsDevice;

        // public instances
        public List<Color> RedLine;
        public List<Color> GreenLine;
        public List<Color> BlueLine;

        // public vars
        public Color SelectedColor;
        public int[] SelectionMarkPosition;
        public int ApsoluteX = 0;

        /// <summary>
        /// Creates the color map and assignes the color lines accordingly
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public ColorMap( GraphicsDevice graphicsDevice )
        {
            // assign the device ref to the local instance
            this._graphicsDevice = graphicsDevice;

            // prepare color line item holders
            RedLine = new List<Color>();
            GreenLine = new List<Color>();
            BlueLine = new List<Color>();

            // create color maps
            _colorMapRed = CreateColorMapXNA( 770, 60, 1 );
            _colorMapGreen = CreateColorMapXNA( 770, 60, 2 );
            _colorMapBlue = CreateColorMapXNA( 770, 60, 3 );
            _colorMapSelectionMark = CreateColorMapXNA( 5, 70, 1 );

            // Set the color marker to the begining
            SelectionMarkPosition = new int[ 2 ] { 0, 0 };
        }

        /// <summary>
        /// Handle the color map input
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void HandleInput( int x, int y )
        {
            if( y > 0 && y < 60 )//Red
            {
                SelectedColor = RedLine[ x ];
                SelectionMarkPosition[ 0 ] = x - 3;
                SelectionMarkPosition[ 1 ] = -5;
            }
            else if( y > 70 && y < 130 )//Green
            {
                SelectedColor = GreenLine[ x ];
                SelectionMarkPosition[ 0 ] = x - 3;
                SelectionMarkPosition[ 1 ] = 65;
            }
            else if( y > 140 && y < 200 )//Blue
            {
                SelectedColor = BlueLine[ x ];
                SelectionMarkPosition[ 0 ] = x - 3;
                SelectionMarkPosition[ 1 ] = 135;
            }
        }
        
        /// <summary>
        /// Draw the color map lines and selector
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw( SpriteBatch spriteBatch )
        {
            spriteBatch.Begin();
            // draw the lines
            spriteBatch.Draw( _colorMapRed, new Microsoft.Xna.Framework.Rectangle( ApsoluteX + 0, 0, 770, 60 ), Color.White );
            spriteBatch.Draw( _colorMapGreen, new Microsoft.Xna.Framework.Rectangle( ApsoluteX + 0, 70, 770, 60 ), Color.White );
            spriteBatch.Draw( _colorMapBlue, new Microsoft.Xna.Framework.Rectangle( ApsoluteX + 0, 140, 770, 60 ), Color.White );

            // draw the color marker
            spriteBatch.Draw( _colorMapSelectionMark, new Rectangle( ApsoluteX + SelectionMarkPosition[ 0 ], SelectionMarkPosition[ 1 ], 5, 70 ), Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Create a color map with a defined scheme, width and height 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="scheme">1 - red, 2 - green, 3 - blue</param>
        /// <returns></returns>
        private Texture2D CreateColorMapXNA( int width, int height, short scheme )
        {
            // create the texture
            Texture2D colorMap = new Texture2D( _graphicsDevice, width, height );

            // define color max values and line width
            int r = 255, g = 255, b = 255, line = width;
            
            // create an array of colors
            Color[] bgcolorArray = new Color[ width ];
            
            // create a list for all content color
            List<Color> bgcolor = new List<Color>();

            // iterate and create a color for each loop that is darker for one r,g or b value than the previous one
            for( int i = 0; i < bgcolorArray.Length; i++ )
            {
                // resolve the scheme
                switch( scheme )
                {
                    case 1:
                        // create the red line palette color based on the r,g and b values
                        bgcolorArray[ i ] = new Color( r, g, b, 100 );
                        RedLine.Add( bgcolorArray[ i ] );
                        if( g != 0 )
                            g--;
                        else if( b != 0 )
                            b--;
                        else if( r != 0 )
                            r--;
                        break;
                    case 2:
                        // create the green line palette color based on the r,g and b values
                        bgcolorArray[ i ] = new Color( r, g, b, 100 );
                        GreenLine.Add( bgcolorArray[ i ] );
                        if( b != 0 )
                            b--;
                        else if( r != 0 )
                            r--;
                        else if( g != 0 )
                            g--;
                        break;
                    case 3:
                        // create the blue line palette color based on the r,g and b values
                        bgcolorArray[ i ] = new Color( r, g, b, 100 );
                        BlueLine.Add( bgcolorArray[ i ] );
                        if( r != 0 )
                            r--;
                        else if( g != 0 )
                            g--;
                        else if( b != 0 )
                            b--;
                        break;
                }
            }

            // add the colors created to all colors
            for( int j = 0; j < height; j++ )
            {
                bgcolor.AddRange( bgcolorArray );
            }

            // set the color map texture data
            colorMap.SetData( bgcolor.ToArray() );

            return colorMap;
        }
    }
}
