using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;

namespace Generator.Core
{
    public interface IGenerator
    {
        void Run();
        void Export();

        PKM GetPokemon();
        GameVersion GetGameVersion();
        IPersonalTable GetPersonalTable();

        //string GetArg(string key);
        //string GetArgOrDefault(string key, string defaultValue);
        //string? GetArgOrNull(string key);

        //// Parsing helpers
        //Species ParseSpecies(string name);
        //Nature ParseNature(string name);
        //ushort ParseMove(string name);
        //int[] ParseStatArray(string csv);

        //// Setters
        //void SetName(string name);
        //void SetSpecies(Species species);
        //void SetOTName(string name);
        //void SetEXP(uint xp);
        //void SetLevel(byte level);
        //void SetExperienceFromArgs();
        //void SetGender(byte gender);
        //void SetBall(byte ball);
        //void SetNature(Nature nature);
        //void SetStatNature(Nature nature);
        //void SetEVs(string evs);
        //void SetForm();
        //void SetShiny(bool shiny);
        //void SetMoves();
        //void SetIVs();

        //// Utility
        //uint GetRequiredEXP(Species species, byte level);
    }
}
