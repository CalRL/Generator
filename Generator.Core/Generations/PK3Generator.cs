using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;

namespace Generator.Core.Generations
{
    public class PK3Generator : GeneratorBase<PK3>
    {
        public PK3Generator(string[] args) : base(args, new PK3 { Version = GameVersion.SV }) { }

    }
}
