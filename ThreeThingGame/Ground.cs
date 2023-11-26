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
            float bedrockDist,
            float clumpSum,
            bool surfaceResources = false
            )
        {
            // Early out
            if (coalDist + oilDist + gasDist + bedrockDist + 4 * clumpSum > 1)
            {
                // If the probabilities ever sum to be more than 1
                return null;
            }

            // Create storage for result
            GroundTile[,] output = new GroundTile[depth, width];

            // Define new random
            Random rng = new Random();

            int offset = 0;

            // If surface resources are disabled
            if (!surfaceResources)
            {
                // Fill the first ground row with rock
                for (int x = 0; x < width; x++)
                {
                    output[0, x] = new GroundTile(0);
                }
                offset = 1;
            }

            // Fill the rest of the ground with tiles
            for (int y = offset; y < depth; y++)
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
                                case 4: // Bedrock
                                    clumpSums[3] = clumpSum;
                                    break;
                            }
                        }
                    }
                    float[] clumpSums = new float[4];

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
                    // True if bedrock
                    else if (randomValue < coalDist + clumpSums[0] + oilDist + clumpSums[1] + gasDist + clumpSums[2] + bedrockDist + clumpSums[3])
                    {
                        output[y, x] = new GroundTile(4);
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
        public static void DrawEmpties(
            SpriteBatch spriteBatch,
            GroundTile[,] ground,
            uint groundWidth,
            uint groundDepth,
            Rectangle drawSpace,
            Dictionary<string, Texture2D> textures
            )
        {
            // Calculate scale factors between screen space and ground space
            Vector2 innerScale = new Vector2(
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
                        (int)(x * innerScale.X) + drawSpace.X,
                        (int)(y * innerScale.Y) + drawSpace.Y,
                        (int)innerScale.X,
                        (int)innerScale.Y
                        );

                    // Pick what needs to be drawn
                    switch (ground[y, x].Filled)
                    {
                        case false:
                            // Draw empty texture
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
            Dictionary<string, Texture2D> textures,
            bool overlayEnabled = true
            )
        {
            // Calculate scale factors between screen space and ground space
            Vector2 innerScale = new Vector2(
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
                        (int)(x * innerScale.X) + drawSpace.X,
                        (int)(y * innerScale.Y) + drawSpace.Y,
                        (int)innerScale.X,
                        (int)innerScale.Y
                        );

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
                                // Bedrock
                                case 4:
                                    // Draw bedrock texture
                                    spriteBatch.Draw(
                                        textures["Bedrock"],
                                        destRect,
                                        Color.White
                                        );
                                    break;
                            }
                            if (y == 0 && overlayEnabled) // Draw snow overlay
                            {
                                // Draw snow overlay
                                spriteBatch.Draw(
                                    textures["Snow_Overlay"],
                                    destRect,
                                    Color.White
                                    );
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Given a ground, returns an array of all tiles with exposed edges.
        /// </summary>
        /// <param name="ground">The ground to search for exposed tiles on.</param>
        /// <param name="drawSpace">The space that the ground is drawn in.</param>
        /// <returns>Returns an array of ground tiles.</returns>
        public static GroundTile[] GetSurface(Ground ground, Rectangle drawSpace)
        {
            // Define list of ground tiles
            List<GroundTile> container = new List<GroundTile>();

            // Calculate scale factor
            Vector2 innerScale = new Vector2(
                drawSpace.Width / ground.Width,
                drawSpace.Height / ground.Depth
                );

            // Loop through each block in ground
            for (int y = 0; y < ground.Depth; y++)
            {
                // Track if we are still at the surface
                bool atSurface = false;

                for (int x = 0; x < ground.Width; x++)
                {
                    // Track if the tile is exposed on any side
                    bool exposed = false;

                    // Check if any of the current tile's sides are exposed
                    if (y > 0 && !ground.Tiles[y - 1, x].Filled)
                    {
                        exposed = true;
                    }
                    else if (y < ground.Depth - 1 && !ground.Tiles[y + 1, x].Filled)
                    {
                        exposed = true;
                    }
                    else if (x > 0 && !ground.Tiles[y, x - 1].Filled)
                    {
                        exposed = true;
                    }
                    else if (x < ground.Width - 1 && !ground.Tiles[y, x + 1].Filled)
                    {
                        exposed = true;
                    }
                    else if (y == 0) // To include tiles at top of list with no tiles ever above
                    {
                        exposed = true;
                    }
                    else if (x == 0)
                    {
                        exposed = true;
                    }
                    else if (x == ground.Width - 1)
                    {
                        exposed = true;
                    }

                    // If exposed
                    if (exposed && ground.Tiles[y, x].Filled)
                    {
                        // Generate a rectangle based on where the tile should be on screen
                        Rectangle destRect = new Rectangle(
                            (int)(x * innerScale.X) + drawSpace.X,
                            (int)(y * innerScale.Y) + drawSpace.Y,
                            (int)innerScale.X,
                            (int)innerScale.Y
                            );

                        container.Add(new GroundTile(destRect, ground.Tiles[y, x].Type));
                        atSurface = true;
                    }

                }

                if (!atSurface)
                {
                    // Ditch the loop, as the surface has been read
                    break;
                }
            }

            // Return array of surface tiles
            return container.ToArray();
        }
        public static (int y, int x) GetNearestTileToPoint(Vector2 point, Ground ground, Rectangle drawSpace, Vector2 range)
        {
            // Define nearest
            (int y, int x) nearest = (-1, -1);
            Vector2 nearestDist = new Vector2(int.MaxValue, int.MaxValue);

            // Calculate scale factor
            Vector2 innerScale = new Vector2(
                drawSpace.Width / ground.Width,
                drawSpace.Height / ground.Depth
                );

            // Loop through each block in ground
            for (int y = 0; y < ground.Depth; y++)
            {
                for (int x = 0; x < ground.Width; x++)
                {
                    // Calculate tile position
                    (int x, int y) tilePos = ((int)(x * innerScale.X) + drawSpace.X, (int)(y * innerScale.Y) + drawSpace.Y);

                    // Check range
                    Vector2 dist = new Vector2(
                        (int)Math.Abs(Math.Abs(tilePos.x + innerScale.X / 2) - Math.Abs(point.X)),
                        (int)Math.Abs(Math.Abs(tilePos.y + innerScale.Y / 2) - Math.Abs(point.Y))
                        );

                    if (ground.Tiles[y, x].Filled)
                    {
                        if (dist.X <= range.X && dist.Y <= range.Y)
                        {
                            // If tile is in range
                            if (dist.Length() < nearestDist.Length())
                            {
                                nearest = (y, x);
                                nearestDist = dist;
                            }
                        }
                    }
                }
            }
            return nearest;
        }
    }
}
