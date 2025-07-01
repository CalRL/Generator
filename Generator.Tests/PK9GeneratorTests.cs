using Generator.Core.Generations;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Tests
{
    public class PK9GeneratorTests
    {
        [Fact]
        public void Generator_CreatesExpectedSpecies()
        {
            string[] args = new[]
            {
            "--trainername=Ash",
            "--xp=50000",
            "--gender=1",
            "--nature=Bold",
            "--species=Pikachu",
            "--ivs=31,31,31,31,31,31",
            "--move1=Thunderbolt",
            "--move2=QuickAttack",
            "--move3=None",
            "--move4=None",
            "--shiny=true"
        };

            var gen = new PK9Generator(args);
            gen.Run(); // Will save file and mutate internal Pokémon object

            var pkm = gen.GetPokemon(); // Optional: expose GetPokemon() as `internal` and make internals visible to test project

            Assert.Equal(25, pkm.Species); // Pikachu = 25
            Assert.Equal("Ash", pkm.OriginalTrainerName);
            Assert.True(pkm.IsShiny);
        }

        [Fact]
        public void Run_WithBasicArgs_SetsAllFields()
        {
            var args = new[]
            {
        "--trainername=Ash",
        "--xp=10000",
        "--gender=1",
        "--nature=Brave",
        "--ball=4",
        "--species=Pikachu",
        "--nickname=Zap",
        "--shiny=true",
        "--ivs=31,31,31,31,31,31",
        "--move1=Tackle",
        "--move2=TailWhip",
        "--move3=ThunderShock",
        "--move4=QuickAttack"
    };

            var gen = new PK9Generator(args);
            gen.Run();

            var pkm = gen.GetPokemon();

            Assert.Equal("Ash", pkm.OriginalTrainerName);
            Assert.Equal(10000u, pkm.EXP);
            Assert.Equal(1, pkm.Gender);
            Assert.Equal(Nature.Brave, pkm.Nature);
            Assert.Equal((byte)Ball.Poke, pkm.Ball); // 4 = Poké Ball
            Assert.Equal((ushort)Species.Pikachu, pkm.Species);
            Assert.Equal("Zap", pkm.Nickname);
            Assert.True(pkm.IsShiny);
            Assert.Equal(31, pkm.IV_HP);
            Assert.Equal((ushort)Move.Tackle, pkm.Moves[0]);
        }
    }

}
