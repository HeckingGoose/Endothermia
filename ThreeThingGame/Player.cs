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
        }
        public void SetPlayerState(State state, Dictionary<string, Texture2D> textures)
        {
            // Assign player state
            _state = state;

            // Update texture
            switch (PlayerState)
            {
                case State.Climbing:
                    Texture = textures[$"{ID}_Climb"];
                    break;
                case State.Mining_Right:
                    Texture = textures[$"{ID}_Mine_Right"];
                    break;
                case State.Mining_Left:
                    Texture = textures[$"{ID}_Mine_Left"];
                    break;
                case State.Mining_Down:
                    Texture = textures[$"{ID}_Mine_Down"];
                    break;
                case State.Mining_Up:
                    Texture = textures[$"{ID}_Mine_Up"];
                    break;
                case State.Idle:
                    Texture = textures[$"{ID}_Front"];
                    break;
            }
        }
    }
}
