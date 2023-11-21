using AutoMapper;
using Catalog.Controllers;
using Catalog.Dto;
using Catalog.Interfaces;
using Catalog.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Tests.Controller;

public class PokemonControllerTests
{
    private readonly IPokemonRepository _PokemonRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public PokemonControllerTests()
    {
        _PokemonRepository = A.Fake<IPokemonRepository>();
        _reviewRepository = A.Fake<IReviewRepository>();
        _mapper = A.Fake<IMapper>();
    }

    [Fact]
    public void PokemonController_GetPokemon_ReturnSuccess()
    {
        //Arrange
        var pokemons = A.Fake<ICollection<PokemonDto>>();
        var pokemonList = A.Fake<List<PokemonDto>>();
        A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);
        var controller = new PokemonController(_PokemonRepository, _reviewRepository, _mapper);
        //Act
        var result = controller.GetPokemons();
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(OkObjectResult));
    }

    [Fact]
    public void PokemonController_CreatePokemon_ReturnOk()
    {
        //Arrange
        int ownerId = 1;
        int catId = 2;
        var pokemonMap = A.Fake<Pokemon>();
        var pokemon = A.Fake<Pokemon>();
        var pokemonCreate = A.Fake<PokemonDto>();
        var pokemons = A.Fake<ICollection<PokemonDto>>();
        var pokemonList = A.Fake<List<PokemonDto>>();
        A.CallTo(() => _PokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(pokemon);
          A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemon);
          A.CallTo(() => _PokemonRepository.CreatePokemon(ownerId, catId, pokemonMap)).Returns(true);
          var controller = new PokemonController(_PokemonRepository, _reviewRepository, _mapper);
          //Act
          var result = controller.CreatePokemon(ownerId, catId, pokemonCreate);
          //Assert
          result.Should().NotBeNull();
    }

    [Fact]
    public void PokemonController_UpdatePokemonReturnOk()
    {
        //Arrange
        int ownerId = 1;
        int catId = 2;
        int pokeId = 1;
        var pokemon = A.Fake<Pokemon>();
        var updatedPokemon = A.Fake<PokemonDto>();
        var pokemons = A.Fake<ICollection<PokemonDto>>();
        var pokemonMap = A.Fake<List<PokemonDto>>();
        A.CallTo(() => _mapper.Map<Pokemon>(updatedPokemon)).Returns(pokemon);
        //Act
        var controller = new PokemonController(_PokemonRepository, _reviewRepository, _mapper);
        //Assert
        var result = controller.UpdatePokemon(pokeId, ownerId, catId, updatedPokemon);
        result.Should().NotBeNull();
    }

    [Fact]
    public void PokemonController_DeletePokemonReturnOk()
    {
        //Arrange
        int pokeId = 1;
        var pokemon = A.Fake<Pokemon>();
        var pokemonToDelete = A.Fake<PokemonDto>();
        A.CallTo(() => _mapper.Map<Pokemon>(pokemonToDelete)).Returns(pokemon);
        //Act
        var controller = new PokemonController(_PokemonRepository, _reviewRepository, _mapper);
        //Assert
        var result = controller.DeletePokemon(pokeId);
        result.Should().NotBeNull();
    }
}