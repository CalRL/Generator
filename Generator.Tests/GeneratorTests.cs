using Generator.Core;
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

        private static IGenerator CreateGenerator(int version, string[] args) => version switch
        {
            9 => new PK9Generator(args),
            3 => new PK3Generator(args),
            _ => throw new NotSupportedException($"Unsupported version: {version}")
        };

        [Theory]
        [InlineData(9)]
        [InlineData(3)]
        public void Generates_Pikachu_Level_50(int version)
        {
            string[] args = new[]
            {
                $"--version={version}",
                "--species=25",
                "--nickname=Pikachu",
                "--level=50"
            };

            var generator = CreateGenerator(version, args);
            generator.Run();

            var pk = generator.GetPokemon();

            Assert.Equal((int)Species.Pikachu, pk.Species);
            Assert.Equal("Pikachu", pk.Nickname);
            Assert.Equal(50, pk.CurrentLevel);
        }

    }

}
