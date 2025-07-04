using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;

namespace Generator.Core
{
    public static class PersonalTableMap
    {
        public static readonly Dictionary<GameVersion, IPersonalTable> Map = new()
        {
            { GameVersion.SV, PersonalTable.SV },
            { GameVersion.E,  PersonalTable.E  },
            { GameVersion.RS,  PersonalTable.RS },
            { GameVersion.Pt, PersonalTable.Pt },
        };

        public static IPersonalTable Get(GameVersion version)
        {
            if (!Map.TryGetValue(version, out var table))
                throw new NotSupportedException($"No PersonalTable mapped for GameVersion: {version}");

            return table;
        }
    }
}
