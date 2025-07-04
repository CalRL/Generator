# Generator CLI Usage

## Command

```bash
Generator.exe [options]
```

| Argument                 | Type    | Required      | Description                                                                        |
| ------------------------ | ------- | ------------- | ---------------------------------------------------------------------------------- |
| `--version=`             | int     | Yes           | Generation to use (e.g., `9` for `SV`, `3` for `RSE`). Throws an error if not set. |
| `--trainername`          | string  | No            | Original Trainer name                                                              |
| `--species=`             | string  | No            | Pokémon species name.                                                              |
| `--nickname=`            | string  | No            | Custom nickname for the Pokémon.                                                   |
| `--level=`               | byte    | Conditional\* | Level of the Pokémon. Mutually exclusive with `--xp=`.                             |
| `--xp=`                  | uint    | Conditional\* | Exact experience value. Mutually exclusive with `--level=`.                        |
| `--form=`                | int     | No            | Form index (e.g., for Pikachu, Deoxys, etc.).                                      |
| `--nature=`              | string  | No            | Nature name (e.g., `Adamant`, `Timid`).                                            |
| `--gender=`              | string  | No            | Gender (`male = 0`, `female = 1`, or `genderless = 2`).                            |
| `--ball=`                | string  | No            | ID of Poké Ball used (e.g., 4 for `Poke Ball`).                                    |
| `--shiny=`               | boolean | No            | Makes the Pokémon shiny (e.g., `--shiny=true` makes it shiny)                      |
| `--move1=` to `--move4=` | string  | No            | Up to 4 moves to assign to the Pokémon.                                            |
| `--ivs`                  | string  | No            | CSV of ivs in format: `HP,ATK,DEF,SPEED,SPATK,SPDEF`                               |

\* If both `--level` and `--xp` are present, the program will throw an error.

## Examples

Generate a shiny Pikachu at level 50:

```bash
Generator.exe --version=9 --species=Pikachu --level=50 --shiny
```

Export a Modest shiny Charizard with specific moves:

```bash
Generator.exe --version=9 --species=Charizard --shiny=true --nature=Modest --moves=Flamethrower,AirSlash,Roost,Protect

```
