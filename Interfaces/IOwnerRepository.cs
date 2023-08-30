using Catalog.Models;

namespace Catalog.Interfaces;

public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();
    Owner GetOwner(int ownerId);
    ICollection<Owner> GetOwnerOfAPokemon(int pokeId);
    ICollection<Pokemon> GetPokemonByOwner(int ownerid);
    bool OwnerExists(int ownerId);
    bool CreateOwner(Owner owner);
    bool UpdateOwner(Owner owner);
    bool DeleteOwner(Owner owner);
    bool Save();
}