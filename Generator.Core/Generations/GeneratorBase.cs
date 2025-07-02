using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;

namespace Generator.Core.Generations
{
    public abstract class GeneratorBase<T> : IGenerator where T : PKM
    {
        protected readonly T pokemon;
        private readonly string[] args;
        public GeneratorBase(string[] args, T pokemon)
        {
            this.args = args;
            this.pokemon = pokemon;
        }

        public T GetPokemon()
        {
            return pokemon;
        }

        public void Run()
        {
            string? trainerName = GetArgOrDefault("trainername", "PLACEHOLDER");
            SetOTName(trainerName);

            this.SetExperienceFromArgs();

            byte gender = byte.Parse(GetArgOrDefault("gender", "0"));
            this.SetGender(gender);

            Nature nature = ParseNature(GetArgOrDefault("nature", "hardy"));
            this.SetNature(nature);
            this.SetStatNature(nature);

            byte ball = byte.Parse(GetArgOrDefault("ball", "4"));
            this.SetBall(ball);

            Species species = ParseSpecies(GetArgOrDefault("species", "Pikachu"));
            this.SetSpecies(species);

            string name = GetArgOrDefault("nickname", species.ToString());
            this.SetName(name);

            bool isShiny = bool.Parse(GetArgOrDefault("shiny", "false"));
            this.SetShiny(isShiny);

            this.SetMoves();
            this.SetIVs();
            this.SetForm();
        }

        public void Export()
        {
            this.SaveFile();
        }
        protected string GetArg(string key)
        {
            string prefix = $"--{key}=";
            string? match = args.FirstOrDefault(arg => arg.StartsWith(prefix));
            if (match == null || !match.Contains("="))
                throw new ArgumentException($"Missing or invalid argument: --{key}=<value>");
            return match.Split("=", 2)[1];
        }

        protected string GetArgOrDefault(string key, string defaultValue)
        {
            try { return GetArg(key); }
            catch { return defaultValue; }
        }
        protected string? GetArgOrNull(string key)
        {
            try { return GetArg(key); }
            catch { return null; }
        }

        protected Species ParseSpecies(string name) => Enum.Parse<Species>(name, true);
        protected Nature ParseNature(string name) => Enum.Parse<Nature>(name, true);
        protected ushort ParseMove(string name) => (ushort)Enum.Parse<Move>(name, true);

        protected int[] ParseStatArray(string csv)
        {
            var parts = csv.Split(',');
            if (parts.Length != 6)
                throw new ArgumentException("IVs must be 6 comma-separated numbers.");
            return parts.Select(p => int.Parse(p.Trim())).ToArray();
        }

        protected virtual void SetName(string name) => pokemon.Nickname = name;
        protected virtual void SetSpecies(Species species) => pokemon.Species = (ushort)species;
        protected virtual void SetOTName(string name) => pokemon.OriginalTrainerName = name;
        protected virtual void SetEXP(uint xp) => pokemon.EXP = xp;
        
        protected virtual void SetLevel(byte level)
        {
            var xp = this.GetRequiredEXP((Species)this.pokemon.Species, level);
            pokemon.EXP = xp;
        }
        protected virtual void SetExperienceFromArgs()
        {
            string? xpStr = this.GetArgOrNull("xp");
            string? levelStr = this.GetArgOrNull("level");

            if (xpStr != null && levelStr != null)
                throw new ArgumentException("You cannot specify both --xp and --level.");

            if (levelStr != null)
            {
                if (!byte.TryParse(levelStr, out var level))
                    throw new ArgumentException("--level must be a byte between 1 and 100.");
                this.SetLevel(level);
            }
            else if (xpStr != null)
            {
                if (!uint.TryParse(xpStr, out var xp))
                    throw new ArgumentException("--xp must be a positive integer.");
                this.SetEXP(xp);
            }
            else
            {
                throw new ArgumentException("Missing required argument: --xp or --level");
            }
        }
        protected virtual void SetGender(byte gender) => pokemon.Gender = gender;
        protected virtual void SetBall(byte ball) => pokemon.Ball = ball;
        protected virtual void SetNature(Nature nature) => pokemon.Nature = nature;
        protected virtual void SetStatNature(Nature nature) => pokemon.StatNature = nature;
        protected virtual void SetEVs(string evs)
        {
            string[] evsSplit = evs.Split(',');

            var count = 0;
            foreach (var ev in evsSplit)
            {
                var num = int.Parse(ev.Trim());
                count += num;
            }

            if(count > 510)
            {
                throw new ArgumentException("EVs cannot be over 510!");
            }



        }
        protected virtual void SetForm()
        {
            byte form = byte.Parse(GetArgOrDefault("form", "0"));
            pokemon.Form = form;
        }
        protected virtual void SetShiny(bool shiny) => pokemon.SetIsShiny(shiny);

        protected virtual void SetMoves()
        {
            var moves = new[]
            {
            ParseMove(GetArgOrDefault("move1", "None")),
            ParseMove(GetArgOrDefault("move2", "None")),
            ParseMove(GetArgOrDefault("move3", "None")),
            ParseMove(GetArgOrDefault("move4", "None"))
        };
            pokemon.Moves = moves;
        }

        protected abstract IPersonalTable GetPersonalTable();
        protected virtual void SetIVs()
        {
            var stats = ParseStatArray(GetArgOrDefault("ivs", "0,0,0,0,0,0"));
            for (int i = 0; i < 6; i++)
            {
                if (stats[i] > 31 || stats[i] < 0)
                {
                    throw new ArgumentException("IV are invalid!");
                }
                pokemon.SetIV(i, stats[i]);
            }              
        }

        protected virtual uint GetRequiredEXP(Species species, byte level)
        {
            var table = GetPersonalTable();
            var growth = table[(int)species].EXPGrowth;
            return Experience.GetEXP(growth, level);
        }

        protected virtual void SaveFile()
        {
            string name = ((Species)pokemon.Species).ToString();
            string path = Path.Combine(Environment.CurrentDirectory, "exported", $"{name}.PK{pokemon.Format}");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, pokemon.DecryptedBoxData);
            Console.WriteLine($"Saved to: {path}");
        }

    }
}
