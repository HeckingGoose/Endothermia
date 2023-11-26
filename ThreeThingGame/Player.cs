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
        private float _temperature;
        private string _ID;
        private uint _heldCoal;
        private uint _coalCapacity;
        private Vector2 _position;
        private Vector2 _velocity;
        private Vector2 _size;
        private Texture2D _texture;
        private State _state;

        public enum State
        {
            Climbing,
            Mining_Right,
            Mining_Left,
            Mining_Down,
            Mining_Up,
            Idle
        }

        // Constructors
        public Player(string ID)
        {
            _health = 100;
            _temperature = 38;
            _ID = ID;
            _heldCoal = 0;
            _coalCapacity = 0;
            _position = Vector2.Zero;
            _velocity = Vector2.Zero;
            _size = Vector2.Zero;
            _texture = null;
            _state = State.Idle;
        }

        public Player(
            string ID,
            uint coalCapacity,
            Vector2 position,
            Texture2D texture,
            Vector2 size
            )
        {
            _health = 100;
            _temperature = 38;
            _ID = ID;
            _heldCoal = 0;
            _coalCapacity = coalCapacity;
            _position = position;
            _velocity = Vector2.Zero;
            _texture = texture;
            _size = size;
            _state = State.Idle;
        }
        public Player(
            uint health,
            float temperature,
            string ID,
            uint coalCapacity,
            Vector2 position,
            Vector2 velocity,
            Vector2 size,
            Texture2D texture
            )
        {
            _health = health;
            _temperature = temperature;
            _ID = ID;
            _heldCoal = 0;
            _coalCapacity = coalCapacity;
            _position = position;
            _velocity = velocity;
            _texture = texture;
            _size = size;
            _state = State.Idle;
        }

        //Static Methods
        public static Texture2D SetTexture(
            Player player,
            Dictionary<string, Texture2D> textures
            )
        {
            switch (player.PlayerState)
            {
                case State.Climbing:
                    player.Texture = textures[$"{player.ID}_Climb"];
                    break;
                case State.Mining_Right:
                    player.Texture = textures[$"{player.ID}_Mine_Right"];
                    break;
                case State.Mining_Left:
                    player.Texture = textures[$"{player.ID}_Mine_Left"];
                    break;
                case State.Mining_Down:
                    player.Texture = textures[$"{player.ID}_Mine_Down"];
                    break;
                case State.Mining_Up:
                    player.Texture = textures[$"{player.ID}_Mine_Up"];
                    break;
                case State.Idle:
                    player.Texture = textures[$"{player.ID}_Front"];
                    break;
            }
        }

        // Methods
        public uint Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public float Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public string ID
        {
            get { return _ID; }
        }

        public uint HeldCoal
        {
            get { return _heldCoal; }
            set { _heldCoal = value; }
        }

        public uint CoalCapacity
        {
            get { return _coalCapacity; }
            set { _coalCapacity = value; }
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

        public State PlayerState
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}
