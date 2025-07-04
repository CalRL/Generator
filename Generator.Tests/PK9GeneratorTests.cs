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
        //basic tests
        [Fact]
        public void SetLevel_SetsCorrectEXP()
        {
            string[] args = new[]
           {
                "--version=9",
                "--species=25",
                "--nickname=Pikachu",
                "--level=50",
                "--trainerName=CAL",
            };
            byte setLevel = 50;
            PK9Generator generator = new PK9Generator(args);
            generator.SetSpecies(Species.Pikachu);
            generator.SetLevel(setLevel);

            PK9 pk = (PK9) generator.GetPokemon();
            PersonalInfo9SV entry = PersonalTable.SV.GetFormEntry((int)Species.Pikachu, 0);

            byte growth = entry.EXPGrowth;
            uint expectedEXP = Experience.GetEXP(setLevel, growth);
            // Pikachu is MediumFast
            Assert.Equal(expectedEXP, generator.GetPokemon().EXP);
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

            PK9 pk = (PK9)generator.GetPokemon();

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
            PK9 pk = (PK9)generator.GetPokemon();

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

        // Reason for comment: SetEVS not yet implemented, will not set evs
        //[Fact]
        //public void Valid_EVS()
        //{
        //    var generator = new PK9Generator(this.args);
        //    string validEvs = "252,252,6,0,0,0";
        //    generator.SetEVs(validEvs);
        //    var pk = (PK9)generator.GetPokemon();

        //    Span<int> evs = stackalloc int[6];
        //    pk.GetEVs(evs);

        //    Assert.Equal(252, evs[0]); // HP
        //    Assert.Equal(252, evs[1]); // Atk
        //    Assert.Equal(6, evs[2]); // Def
        //    Assert.Equal(0, evs[3]); // Spe
        //    Assert.Equal(0, evs[4]); // SpA
        //    Assert.Equal(0, evs[5]); // SpD
        //}

        [Fact]
        public void Invalid_EVS_Should_Throw_Error()
        {
            string[] args = new[]
            {
                "--version=9",
                "--species=25",
                "--nickname=Pikachu",
                "--level=50"
            };
            var generator = new PK9Generator(args);
            var evs = "252,252,10,10,10,10";

            var ex = Assert.Throws<ArgumentException>(() => generator.SetEVs(evs));
            Assert.Contains("EVs cannot be over 510", ex.Message);
        }

        [Fact]
        public void Level_Too_High()
        {
            var generator = new PK9Generator([]);

            byte level = 102;
            var ex = Assert.Throws<ArgumentException>(() => generator.SetLevel(level));
            Assert.Contains("Level must be between 0 and 100!", ex.Message);
        }

        [Fact]
        public void Set_Valid_Ball()
        {

        }

        [Fact]
        public void Export_Creates_And_Cleans_File()
        {
            // Arrange
            string[] args = new[]
            {
                "--version=9",
                "--species=Pikachu"
            };

            var generator = new PK9Generator(args);
            generator.Run();

            string speciesName = "Pikachu";
            string formatSuffix = "9"; // PK9
            string fileName = $"{speciesName}.PK{formatSuffix}";
            string exportDir = Path.Combine(Directory.GetCurrentDirectory(), "exported");
            string exportPath = Path.Combine(exportDir, fileName);

            if (File.Exists(exportPath))
                File.Delete(exportPath); // ensure clean slate

            // Act
            generator.Export();

            // Assert
            Assert.True(File.Exists(exportPath), "Exported file was not created.");

            // Optional: Verify file is not empty
            Assert.True(new FileInfo(exportPath).Length > 0, "Exported file is empty.");

            // Cleanup
            File.Delete(exportPath);
        }

        [Fact]
        public void ParseStatArray_ShouldThrow_WhenNotSixValues()
        {
            string[] args = new[]
            {
                "--ivs=31,31,31,31,31",
            };
            var generator = new PK9Generator(args);
            var ex = Assert.Throws<ArgumentException>(() => generator.SetIVs());
            Assert.Contains("6 comma-separated", ex.Message);
        }

        [Fact]
        public void SetIVs_ShouldThrow_WhenValueOutOfRange()
        {
            string[] args = new[]
            {
                "--ivs=31,31,99,31,31,31",
            };
            var generator = new PK9Generator(args);
            var ex = Assert.Throws<ArgumentException>(() => generator.SetIVs()); // 99 is invalid
            Assert.Contains("IV are invalid", ex.Message);
        }

        [Fact]
        public void SetExperienceFromArgs_ShouldThrow_WhenLevelIsInvalid()
        {
            string[] args = new[] { "--level=abc" }; // not a number
            var generator = new PK9Generator(args);
            var ex = Assert.Throws<ArgumentException>(() => generator.Run());
            Assert.Contains("Could not parse level to byte", ex.Message);
        }

        [Fact]
        public void SetExperienceFromArgs_ShouldThrow_WhenXPIsInvalid()
        {
            string[] args = new[] { "--xp=notanumber" };
            var generator = new PK9Generator(args);
            var ex = Assert.Throws<ArgumentException>(() => generator.Run());
            Assert.Contains("--xp must be a positive", ex.Message);
        }
    }
}
