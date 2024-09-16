using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.ComponentPattern.Particles.Origins
{
    public abstract class Origin
    {
        public abstract OriginData GetPosition(Emitter e);

    }
}
