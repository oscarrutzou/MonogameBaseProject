﻿using Microsoft.Xna.Framework;
using System;

namespace BaseProject.ComponentPattern.Particles.Origins
{
    public class RectangleOrigin : Origin
    {
        private readonly Interval x;
        private readonly Interval y;
        private readonly bool _edge;
        private readonly int _width;
        private readonly int _height;
        private static Random _rand = new Random();

        public RectangleOrigin(int width, int height, bool edge = false)
        {
            _edge = edge;
            _width = width;
            _height = height;
            x = new Interval(-width / 2, width / 2);
            y = new Interval(-height / 2, height / 2);
        }

        public void OffCenter(Emitter e)
        {
            e.Position += new Vector2(_width / 2, _height / 2);
        }

        public override OriginData GetPosition(Emitter e)
        {
            if (_edge)
            {
                int n = _rand.Next(_width + _height);

                if (n < _width)
                {

                    if (_rand.Next(2) == 1)
                        return new OriginData(new Vector2((int)x.GetValue(), -_height / 2));
                    else
                        return new OriginData(new Vector2((int)x.GetValue(), _height / 2));
                }
                else
                {
                    if (_rand.Next(2) == 1)
                        return new OriginData(new Vector2(-_width / 2, (int)y.GetValue()));
                    else
                        return new OriginData(new Vector2(_width / 2, (int)y.GetValue()));
                }
            }
            else
            {
                return new OriginData(new Vector2((int)x.GetValue(), (int)y.GetValue()));
            }
        }
    }
}
