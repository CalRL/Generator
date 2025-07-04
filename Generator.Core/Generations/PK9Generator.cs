using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Core.Generations
{
    public class PK9Generator : GeneratorBase<PK9> 
    {
        public PK9Generator(string[] args) : base(args, new PK9 { Version = GameVersion.SV }) { }

    }
}
