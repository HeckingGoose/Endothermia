using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class Ground
    {
        // Variables
        private GroundTile[,] _tiles;
        private uint _width;
        private uint _depth;

        // Constructors
        public Ground() // Default constructor
        {
            _tiles = null;
            _width = 0;
            _depth = 0;
        }
        public Ground(
            GroundTile[,] tiles,
            uint width,
            uint depth
            )
        {
            _tiles = tiles;
            _width = width;
            _depth = depth;
        }


        // Methods
        public GroundTile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }
        public uint Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public uint Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        // Static methods
        /// <summary>
        /// Generates a ground of given width and depth based on given probabilities.
        /// </summary>
        /// <param name="width">Width of the ground to generate in units.</param>
        /// <param name="depth">Depth of the ground to generate in units.</param>
        /// <param name="coalDist">The fractional probability of a block being coal.</param>
        /// <param name="oilDist">The fractional probability of a block being oil.</param>
        /// <param name="gasDist">The fractional probability of a block being gas.</param>
        /// <param name="clumpSum">Added probability to tile types that match the surrounding tiles.</param>
        /// <returns>Returns a 2D array representing the ground.</returns>
        public static GroundTile[,] GenerateGround(
            uint width,
            uint depth,
            float coalDist,
            float oilDist,
            float gasDist,
            float clumpSum
            )
        {
            // Early out
            if (coalDist + oilDist + gasDist + 3 * clumpSum > 1)
            {
                // If the probabilities ever sum to be more than 1
                return null;
            }

            // Create storage for result
            GroundTile[,] output = new GroundTile[depth, width];

            // Define new random
            Random rng = new Random();

            // Fill the ground with tiles
            for (int y = 0; y < depth; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Define function for adding to sum
                    void ClumpSum(GroundTile tile, ref float[] clumpSums)
                    {
                        if (tile != null)
                        {
                            switch (tile.Type)
                            {
                                case 1: // Coal
                                    clumpSums[0] = clumpSum;
                                    break;
                                case 2: // Oil
                                    clumpSums[1] = clumpSum;
                                    break;
                                case 3: // Gas
                                    clumpSums[2] = clumpSum;
                                    break;
                            }
                        }
                    }
                    float[] clumpSums = new float[3];

                    // Generate array of which tile types surround the current tile
                    if (y > 0)
                    {
                        if (x > 0)
                        {
                            ClumpSum(output[y - 1, x - 1], ref clumpSums);
                            ClumpSum(output[y, x - 1], ref clumpSums);
                        }
                        if (x < width - 1)
                        {
                            ClumpSum(output[y - 1, x + 1], ref clumpSums);
                            ClumpSum(output[y, x + 1], ref clumpSums);
                        }
                        ClumpSum(output[y - 1, x], ref clumpSums);
                    }
                    if (y < depth - 1)
                    {
                        if (x > 0)
                        {
                            ClumpSum(output[y + 1, x - 1], ref clumpSums);
                        }
                        if (x < width - 1)
                        {
                            ClumpSum(output[y + 1, x + 1], ref clumpSums);
                        }
                        ClumpSum(output[y + 1, x], ref clumpSums);
                    }


                    // Generate random value
                    float randomValue = rng.NextSingle();

                    // True if coal
                    if (randomValue < coalDist + clumpSums[0])
                    {
                        output[y, x] = new GroundTile(1);
                    }
                    // True if oil
                    else if (randomValue < coalDist + clumpSums[0] + oilDist + clumpSums[1])
                    {
                        output[y, x] = new GroundTile(2);
                    }
                    // True if gas
                    else if (randomValue < coalDist + clumpSums[0] + oilDist + clumpSums[1] + gasDist + clumpSums[2])
                    {
                        output[y, x] = new GroundTile(3);
                    }
                    // True if rock
                    else
                    {
                        output[y, x] = new GroundTile(0);
                    }
                }
            }

            // Return result
            return output;
        }
        /// <summary>
        /// Draws the ground given a set of parameters.
        /// </summary>
        /// <param name="spriteBatch">The sprite batcher to draw using.</param>
        /// <param name="ground">The ground to draw.</param>
        /// <param name="groundWidth">Represents the width of the ground in blocks.</param>
        /// <param name="groundDepth">Represents the depth of the ground in blocks.</param>
        /// <param name="drawSpace">A rect representing the origin, width and height of the space to draw in.</param>
        /// <param name="textures">A dictionary of textures.</param>
        public static void DrawGround(
            SpriteBatch spriteBatch,
            GroundTile[,] ground,
            uint groundWidth,
            uint groundDepth,
            Rectangle drawSpace,
            Dictionary<string, Texture2D> textures
            )
        {// DONT FORGET TO APPLY DRAWSPACE XY OFFSETS
            // Calculate scale factors between screen space and ground space
            Vector2 scale = new Vector2(
                drawSpace.Width / groundWidth,
                drawSpace.Height / groundDepth
                );

            // Draw tiles
            for (int y = 0; y < groundDepth; y++)
            {
                for (int x = 0; x < groundWidth; x++)
                {
                    // Generate a rectangle to draw to
                    Rectangle destRect = new Rectangle(
                        (int)(x * scale.X) + 1 + drawSpace.X,
                        (int)(y * scale.Y) + 1 + drawSpace.Y,
                        (int)scale.X + 1,
                        (int)scale.Y + 1);

                    // Pick what needs to be drawn
                    switch (ground[y, x].Filled)
                    {
                        // If the tile is filled
                        case true:
                            switch (ground[y, x].Type)
                            {
                                // Rock
                                case 0:
                                    // Draw rock texture
                                    spriteBatch.Draw(
                                        textures["Rock"],
                                        destRect,
                                        Color.White
                                        );
                                    break;
                                // Coal
                                case 1:
                                    // Draw coal texture
                                    spriteBatch.Draw(
                                        textures["Coal"],
                                        destRect,
                                        Color.White
                                        );
                                    break;
                                // Oil
                                case 2:
                                    // Draw oil texture
                                    spriteBatch.Draw(
                                        textures["Oil"],
                                        destRect,
                                        Color.White
                                        );
                                    break;
                                // Gas
                                case 3:
                                    // Draw gas(?) texture
                                    spriteBatch.Draw(
                                        textures["Gas"],
                                        destRect,
                                        Color.White
                                        );
                                    break;
                            }
                            break;

                        // If the tile is not filled
                        case false:
                            // Draw background texture
                            spriteBatch.Draw(
                                textures["Empty"],
                                destRect,
                                Color.White
                                );
                            break;
                    }
                }
            }
        }
    }
}
