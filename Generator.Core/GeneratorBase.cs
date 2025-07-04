using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;

namespace Generator.Core
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

        public PKM GetPokemon()
        {
            return (PKM) pokemon;
        }

        public T GetTypedPokemon()
        {
            return pokemon;
        }

        public GameVersion GetGameVersion()
        {
            return this.GetPokemon().Version;
        }

        public IPersonalTable GetPersonalTable()
        {
            GameVersion version = this.GetGameVersion();
            IPersonalTable personalTable = PersonalTableMap.Get(version);

            return personalTable;
        }

        public void Run()
        {
            string? trainerName = GetArgOrDefault("trainername", "PLACEHOLDER");
            this.SetOTName(trainerName);

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

            this.SetShinyFromArgs();

            this.CheckMoveArgConflicts();
            this.SetMovesFromArgs();
            this.SetMoves();

            this.SetIVs();
            this.SetForm();
        }

        public void Export()
        {
            SaveFile();
        }
        protected internal string GetArg(string key)
        {
            string prefix = $"--{key}=";
            string? match = args.FirstOrDefault(arg => arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (match == null || !match.Contains("="))
                throw new ArgumentException($"Missing or invalid argument: --{key}=<value>");
            return match.Split("=", 2)[1];
        }

        protected internal string GetArgOrDefault(string key, string defaultValue)
        {
            try { return GetArg(key); }
            catch { return defaultValue; }
        }
        protected internal string? GetArgOrNull(string key)
        {
            try { return GetArg(key); }
            catch { return null; }
        }
        protected bool ArgsContain(string key) => this.args.Any(a => a.Equals($"--{key}", StringComparison.OrdinalIgnoreCase));
        protected void CheckMoveArgConflicts()
        {
            bool hasBulkMoves = ArgsContain("moves");
            bool hasIndividualMoves = ArgsContain("move1") || ArgsContain("move2") ||
                                      ArgsContain("move3") || ArgsContain("move4");

            if (hasBulkMoves && hasIndividualMoves)
                throw new ArgumentException("Cannot use --moves and --move1 to --move4 together. Choose only one method.");
        }

        protected internal void SetMovesFromArgs()
        {
            string? movesArg = GetArgOrNull("moves");
            if(string.IsNullOrWhiteSpace(movesArg))
            {
                return;
            }

            string[] moveNames = movesArg.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if(movesArg.Length > 4)
            {
                throw new ArgumentException("A maximum of 4 moves can be assigned.");
            }

            for(int i = 0; i < moveNames.Length; i++)
            {
                string moveName = moveNames[i];
                
                ushort move = ParseMove(moveName);

                this.pokemon.SetMove(i, move);
            }

        }

        protected internal void SetShinyFromArgs()
        {
            string? shinyArg = GetArgOrNull("shiny");

            if(shinyArg is null)
            {
                if (this.ArgsContain("shiny")) {
                    this.SetShiny(true);
                }
            }
            else
            {
                if (bool.TryParse(shinyArg, out bool isShiny))
                    this.SetShiny(isShiny);
                else
                    throw new ArgumentException("--shiny must be true or false");
            }
        }
             
        protected internal Species ParseSpecies(string name) => Enum.Parse<Species>(name, true);
        protected internal Nature ParseNature(string name) => Enum.Parse<Nature>(name, true);
        protected internal ushort ParseMove(string name) => (ushort)Enum.Parse<Move>(name, true);

        protected internal int[] ParseStatArray(string csv)
        {
            var parts = csv.Split(',');
            if (parts.Length != 6)
                throw new ArgumentException("IVs must be 6 comma-separated numbers.");
            return parts.Select(p => int.Parse(p.Trim())).ToArray();
        }

        protected internal virtual void SetName(string name) => pokemon.Nickname = name;
        protected internal virtual void SetSpecies(Species species) => pokemon.Species = (ushort)species;
        protected internal virtual void SetOTName(string name) => pokemon.OriginalTrainerName = name;
        protected internal virtual void SetEXP(uint xp) => pokemon.EXP = xp;
        
        protected internal virtual void SetLevel(byte level)
        {
            if(level > 100 || level < 0)
            {
                throw new ArgumentException("Level must be between 0 and 100!");
            }
            var xp = this.GetRequiredEXP((Species)pokemon.Species, level);
            pokemon.EXP = xp;
            pokemon.CurrentLevel = level;
        }
        protected internal virtual void SetExperienceFromArgs()
        {
            string? xpStr = GetArgOrNull("xp");
            string? levelStr = GetArgOrNull("level");

            if (xpStr != null && levelStr != null)
                throw new ArgumentException("You cannot specify both --xp and --level.");

            if (levelStr != null)
            {
                if (!byte.TryParse(levelStr, out var level))
                    throw new ArgumentException("Could not parse level to byte");
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
                this.SetLevel(1);
            }
        }
        protected internal virtual void SetGender(byte gender) => pokemon.Gender = gender;
        protected internal virtual void SetBall(byte ball) => pokemon.Ball = ball;
        protected internal virtual void SetNature(Nature nature) => pokemon.Nature = nature;
        protected internal virtual void SetStatNature(Nature nature) => pokemon.StatNature = nature;
        protected internal virtual void SetEVs(string evs)
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
            } else if(count < 0 )
            {
                throw new ArgumentException("EVs cannot be under 0!");
            }
           // TODO: implement

        }
        protected internal virtual void SetForm()
        {
            byte form = byte.Parse(GetArgOrDefault("form", "0"));
            pokemon.Form = form;
        }
        protected internal virtual void SetShiny(bool shiny) => pokemon.SetIsShiny(shiny);

        protected internal virtual void SetMoves()
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

        protected internal virtual void SetIVs()
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

        protected internal virtual uint GetRequiredEXP(Species species, byte level)
        {
            var table = this.GetPersonalTable();
            var growth = table[(int)species].EXPGrowth;
            return Experience.GetEXP(growth, level);
        }

        protected internal virtual void SaveFile()
        {
            string name = ((Species)pokemon.Species).ToString();
            string path = Path.Combine(Environment.CurrentDirectory, "exported", $"{name}.PK{pokemon.Format}");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, pokemon.DecryptedBoxData);
            Console.WriteLine($"Saved to: {path}");
        }

    }
}
