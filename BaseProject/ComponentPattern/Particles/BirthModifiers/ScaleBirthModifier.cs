using Microsoft.Xna.Framework;
using BaseProject.CompositPattern;

namespace BaseProject.ComponentPattern.Particles.BirthModifiers
{
    public class ScaleBirthModifier : BirthModifier
    {
        private readonly Interval _interval;

        public ScaleBirthModifier(Interval scales)
        {
            _interval = scales;
        }

        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float scale = (float)_interval.GetValue();
            p.Scale = new Vector2(scale, scale);

            if (p.TextOnSprite == null) return;

            p.TextOnSprite.TextScale = scale;
        }
    }
}
