using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ColorBash.Enums;

namespace ColorBash.Helpers
{
    /// <summary>
    /// Represents the encapsulation for simple item creation
    /// </summary>
    public static class ItemFactory
    {
        /// <summary>
        /// List of colors to fill the item
        /// </summary>
        public static List<Color> Colors;

        /// <summary>
        /// Creates a texture and fills it with the given color
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D CreateFilledTexture( GraphicsDevice graphicsDevice, int width, int height, Color color )
        {
            return Rectangle( graphicsDevice, width, height, color );
        }

        private static Texture2D Rectangle( GraphicsDevice graphicsDevice, int width, int height, Color color )
        {
            Texture2D rectangle = new Texture2D( graphicsDevice, width, height );

            Color[] bgColor = new Color[ width * height ];

            for( int i = 0; i < bgColor.Length; i++ )
            {
                bgColor[ i ] = color;
            }

            rectangle.SetData( bgColor );

            return rectangle;
        }

        private static Texture2D Random( GraphicsDevice graphicsDevice, int width, int height, Color color )
        {
            Texture2D rectangle = new Texture2D( graphicsDevice, width, height );

            Color[] bgColor = new Color[ width * height ];
            byte[] b = new byte[ width * height ];
            Random r = new Random();
            r.NextBytes( b );

            for( int i = 0; i < bgColor.Length; i++ )
            {
                if(r.Next(0,255) < b[i])
                    bgColor[ i ] = color;
            }

            rectangle.SetData( bgColor );

            return rectangle;
        }
    }
}
