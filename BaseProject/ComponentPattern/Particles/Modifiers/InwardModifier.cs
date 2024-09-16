﻿using BaseProject.ComponentPattern.Particles.BirthModifiers;
using Microsoft.Xna.Framework;
using SharpDX.X3DAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.ComponentPattern.Particles.Modifiers
{
    public class InwardModifier : Modifier
    {
        private float _minDistanceBeforeDelete;

        public InwardModifier(int minDistanceBeforeDelete = 2)
        {
            _minDistanceBeforeDelete = minDistanceBeforeDelete;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            float v = p.VelocityZ.Length();
            Vector2 targetPos;
            if (e.FollowPoint != Vector2.Zero) targetPos = e.FollowPoint;
            else targetPos = e.Position;
            
            Vector2 temp = targetPos - p.Position;
            Vector2 tempAbs = new Vector2(MathF.Abs(temp.X), MathF.Abs(temp.Y));

            if (tempAbs.X < _minDistanceBeforeDelete && tempAbs.Y < _minDistanceBeforeDelete) 
            {
                p.Age = p.MaxAge;
            }

            if (temp != Vector2.Zero)
            {
                temp.Normalize();

                float z = 0;
                if (p.VelocityZ != Vector3.Zero)
                {
                    Vector3 normalizedVel = Vector3.Normalize(p.VelocityZ);
                    z = normalizedVel.Z;
                }

                p.VelocityZ = new Vector3(temp, z) * v;
            }

            // Growing it in size (this was for a emitter that made 10.000 per sec)
            //_minDistanceBeforeDelete += 0.00001f;
        }
    }
}
