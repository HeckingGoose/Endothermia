using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    /// <summary>
    /// Contains information representing a singular ground tile.
    /// </summary>
    internal class GroundTile
    {
        // Variables
        private bool _filled;
        private byte _type;
        private Rectangle _rect;

        // Constructors
        /// <summary>
        /// Create a new ground tile using the minimum required values.
        /// </summary>
        /// <param name="type">Whether the tile is rock, coal, oil or gas.</param>
        public GroundTile( // Default constructor
            byte type
            )
        {
            _filled = true;
            _type = type;
        }
        /// <summary>
        /// Create a new ground tile with all values specified.
        /// </summary>
        /// <param name="filled">Whether the tile is filled or not.</param>
        /// <param name="type">Whether the tile is rock, coal, oil or gas.</param>
        public GroundTile( // Complete constructor
            Rectangle rect,
            byte type
            )
        {
            _filled = true;
            _rect = rect;
            _type = type;
        }
        public GroundTile() // Empty constructor
        {
            _filled = true;
            _type = 50;
        }

        // Methods
        public bool Filled
        {
            get { return _filled; }
            set { _filled = value; }
        }
        public byte Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public Rectangle Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
    }
}
