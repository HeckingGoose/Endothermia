using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class Player
    {
        // Variables
        private uint _health;
        private uint _temperature;
        private byte _ID;
        private float _heldCoal;
        private Vector2 _position;
        private Vector2 _velocity;
        private Vector2 _size;
        private Texture2D _texture;

        // Constructors
        public Player(byte ID)
        {
            _health = 100;
            _temperature = 38;
            _ID = ID;
            _heldCoal = 0;
            _position = Vector2.Zero;
            _velocity = Vector2.Zero;
            _size = Vector2.Zero;
            _texture = null;
        }

        public Player(
            byte ID,
            Vector2 position,
            Texture2D texture,
            Vector2 size
            )
        {
            _health = 100;
            _temperature = 38;
            _ID = ID;
            _heldCoal = 0;
            _position = position;
            _velocity = Vector2.Zero;
            _texture = texture;
            _size = size;
        }
        public Player(
            uint health,
            uint temperature,
            byte ID,
            Vector2 position,
            Vector2 velocity,
            Vector2 size,
            Texture2D texture
            )
        {
            _health = health;
            _temperature = temperature;
            _ID = ID;
            _position = position;
            _velocity = velocity;
            _texture = texture;
            _size = size;
        }

        // Methods
        public uint Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public uint Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public byte ID
        {
            get { return _ID; }
        }

        public float HeldCoal
        {
            get { return _heldCoal; }
            set { _heldCoal = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
    }
}
