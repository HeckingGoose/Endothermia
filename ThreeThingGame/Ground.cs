using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class Ground
    {
        // Methods
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
            if (coalDist + oilDist + gasDist + 3 * clumpSum< 1)
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
                        output[y, x].Type = 1;
                    }
                    // True if oil
                    else if (randomValue < coalDist + clumpSums[0] + oilDist + clumpSums[1])
                    {
                        output[y, x].Type = 2;
                    }
                    // True if gas
                    else if (randomValue < coalDist + clumpSums[0] + oilDist + clumpSums[1] + gasDist + clumpSums[2])
                    {
                        output[y, x].Type = 3;
                    }
                    // True if rock
                    else
                    {
                        output[y, x].Type = 0;
                    }
                }
            }

            // Return result
            return output;
        }
    }
}
