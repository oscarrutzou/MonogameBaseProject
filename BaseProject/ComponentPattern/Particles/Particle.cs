using BaseProject.CompositPattern;
using BaseProject.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaseProject.ComponentPattern.Particles
{
    public class Particle : Component, IParticle
    {
        public double Age { get; set; }
        public double MaxAge { get; set; }
        //public Vector2 Velocity { get; set; }
        private Vector3 _velocityZ;
        public Vector3 VelocityZ
        {
            get
            {
                return _velocityZ;
            }
            set
            {
                _velocityZ = value;
                //Velocity = new Vector2(_velocityZ.X, _velocityZ.Y);
            }
        }
        public float RotationVelocity { get; set; }
        
        public Vector2 Position
        {
            get { return GameObject.Transform.Position; }
            set { GameObject.Transform.Position = value; }
        }

        private Vector3 _position;
        public Vector3 PositionZ
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                GameObject.Transform.Position = new Vector2(_position.X, _position.Y);
            }
        }
        public Vector2 Scale
        {
            get { return GameObject.Transform.Scale; }
            set { GameObject.Transform.Scale = value; }
        }
        public double Alpha
        {
            get { return Color.A; }
            set
            {
                Color color = Color;
                color.A = (byte)(Math.Clamp(value, 0, 1) * 255);
            }
        }

        private Color nextColor;
        public Color Color
        {
            get 
            {
                if (_spriteRenderer == null) return nextColor;
                return _spriteRenderer.Color; 
            }
            set 
            { 
                if (_spriteRenderer == null)
                {
                    nextColor = value;
                    return;
                }
                nextColor = value;
                _spriteRenderer.Color = value; 
            }
        }

        public TextOnSprite TextOnSprite { get; set; }

        private SpriteRenderer _spriteRenderer;

        public Particle(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.Color = Color;
            _spriteRenderer.TextOnSprite = TextOnSprite;
        }

    }
}
