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
        private readonly string[] args = new[]
        {
            "--version=9",
            "--species=25",
            "--nickname=Pikachu",
            "--level=50",
            "--trainerName=CAL",
        };

        //basic tests
        [Fact]
        public void SetLevel_SetsCorrectEXP()
        {
            byte setLevel = 50;
            PK9Generator generator = new PK9Generator(this.args);
            generator.SetSpecies(Species.Pikachu);
            generator.SetLevel(setLevel);

            PK9 pk = generator.GetPokemon();
            PersonalInfo9SV entry = PersonalTable.SV.GetFormEntry((int)Species.Pikachu, 0);

            byte growth = entry.EXPGrowth;
            uint expectedEXP = Experience.GetEXP(setLevel, growth);
            uint expectedLevel = Experience.GetLevel(growth, setLevel);
            // Pikachu is MediumFast
            Assert.Equal(expectedEXP, generator.GetPokemon().EXP);
            Assert.Equal(expectedLevel, generator.GetPokemon().CurrentLevel);
        }


        // in depth tests
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

            PK9 pk = (PK9) generator.GetPokemon();  

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

            Assert.Contains("cannot specify both", ex.Message.ToLower());
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
                "--ivs=31,31,0,31,0,31"
            };

            var generator = new PK9Generator(args);
            generator.Run();
            PK9 pk = (PK9) generator.GetPokemon();

            Assert.Equal(31, pk.IV_HP);
            Assert.Equal(31, pk.IV_ATK);
            Assert.Equal(0, pk.IV_DEF);
            Assert.Equal(31, pk.IV_SPE);
            Assert.Equal(0, pk.IV_SPA);
            Assert.Equal(31, pk.IV_SPD);
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

            Assert.Equal((int)Move.Tackle, pk.Move1);
            Assert.Equal((int)Move.Growl, pk.Move2);
            Assert.Equal((int)Move.LeechSeed, pk.Move3);
            Assert.Equal((int)Move.VineWhip, pk.Move4);
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
