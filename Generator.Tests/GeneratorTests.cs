using Generator.Core.Generations;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void Generates_Pikachu_Level_50()
        {
            string[] args = new[]
            {
                "--version=9",
                "--species=25",
                "--nickname=Pikachu",
                "--level=50"
            };

            var generator = new PK9Generator(args);
            generator.Run();

            var pk = generator.GetPokemon();

            Assert.Equal((int)Species.Pikachu, pk.Species);
            Assert.Equal("Pikachu", pk.Nickname);
            Assert.Equal(50, pk.CurrentLevel);
        }

        [Fact]
        public void Throws_When_Both_XP_And_Level_Set()
        {
            string[] args = new[]
            {
                "--version=9",
                "--species=1",
                "--level=10",
                "--xp=5000"
            };

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var generator = new PK9Generator(args);
                generator.Run();
            });

            Assert.Contains("cannot set both", ex.Message.ToLower());
        }
        [Fact]
        public void Generates_With_Custom_IVs()
        {
            string[] args = new[]
            {
                "--version=9",
                "--species=25",
                "--nickname=Pikachu",
                "--level=50",
                "--iv_hp=31",
                "--iv_atk=31",
                "--iv_def=0",
                "--iv_spa=31",
                "--iv_spd=0",
                "--iv_spe=31"
            };

            var generator = new PK9Generator(args);
            generator.Run();
            var pk = generator.GetPokemon();

            Assert.Equal(31, pk.IV_HP);
            Assert.Equal(31, pk.IV_ATK);
            Assert.Equal(0, pk.IV_DEF);
            Assert.Equal(31, pk.IV_SPA);
            Assert.Equal(0, pk.IV_SPD);
            Assert.Equal(31, pk.IV_SPE);
        }

        [Fact]
        public void Generates_With_Moves()
        {
            string[] args = new[]
            {
                "--version=9",
                "--species=1",
                "--nickname=BulbaMoves",
                "--level=10",
                "--move1=Tackle",
                "--move2=Growl",
                "--move3=LeechSeed",
                "--move4=VineWhip"
            };

            var generator = new PK9Generator(args);
            generator.Run();
            var pk = generator.GetPokemon();

            Assert.Equal("TACKLE", pk.Move1.ToString().ToUpperInvariant());
            Assert.Equal("GROWL", pk.Move2.ToString().ToUpperInvariant());
            Assert.Equal("LEECHSEED", pk.Move3.ToString().Replace(" ", "").ToUpperInvariant());
            Assert.Equal("VINEWHIP", pk.Move4.ToString().Replace(" ", "").ToUpperInvariant());
        }

        [Fact]
        public void Errors_When_Wrong_Version()
        {
            string[] args = new[] {
                "--version=3",
                "--species=1"
            };

            var generator = new PK9Generator(args);
            generator.Run();
            var pk = generator.GetPokemon();
        }
    }

}
