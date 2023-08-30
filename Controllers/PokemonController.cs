using AutoMapper;
using Catalog.Dto;
using Catalog.Interfaces;
using Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Catalog.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PokemonController : Controller
{
    private readonly IPokemonRepository _PokemonRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public PokemonController(IPokemonRepository PokemonRepository, IReviewRepository reviewRepository, IMapper mapper)
    {
        _PokemonRepository = PokemonRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemons()
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_PokemonRepository.GetPokemons());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(pokemons);
    }

    [HttpGet("{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Pokemon))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int pokeId)
    {
        if (!_PokemonRepository.PokemonExists(pokeId))
            return NotFound();
        var pokemon = _mapper.Map<PokemonDto>(_PokemonRepository.GetPokemon(pokeId));
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(pokemon);
    }
    [HttpGet("{pokeId}/rating")]
    [ProducesResponseType(200, Type = typeof(decimal))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int pokeId)
    {
        if (!_PokemonRepository.PokemonExists(pokeId))
            return NotFound();
        var rating = _PokemonRepository.GetPokemonRating(pokeId);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(rating);
    }
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate) 
    {
        if (pokemonCreate == null)
            return BadRequest(ModelState);
        var pokemon = _PokemonRepository.GetPokemons()
            .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
            .FirstOrDefault();
        if (pokemon != null)
        {
            ModelState.AddModelError("", "Pokemon already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);
        if (!_PokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Succesfully Created");

    }
    [HttpPut("{pokeId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]

    public IActionResult UpdatePokemon(int pokeId,[FromQuery]int ownerId, [FromQuery] int catId, [FromBody] PokemonDto updatedPokemon)
    {
        if (updatedPokemon == null)
            return BadRequest(ModelState);
        if (pokeId != updatedPokemon.Id)
            return BadRequest(ModelState);
        if (!_PokemonRepository.PokemonExists(pokeId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();
        var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);
        if (!_PokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong updating pokemon");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    [HttpDelete("{pokeId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeletePokemon(int pokeId)
    {
        if (!_PokemonRepository.PokemonExists(pokeId))
        {
            return NotFound();
        }

        var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);
        var pokemonToDelete = _PokemonRepository.GetPokemon(pokeId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
        {
            ModelState.AddModelError("", "Something went wrong when deleting reviews");
        }

        if (!_PokemonRepository.DeletePokemon(pokemonToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting owner");
        }

        return NoContent();
    }
}
