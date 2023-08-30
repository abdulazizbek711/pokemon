using Catalog.Models;

namespace Catalog.Interfaces;

public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons();
    Pokemon GetPokemon(int Id);
    Pokemon GetPokemon(string name);
    decimal GetPokemonRating(int Id);
    bool PokemonExists(int pokeId); 
    bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
    bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon);
    bool DeletePokemon(Pokemon pokemon);
    bool Save();
}