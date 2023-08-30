using AutoMapper;
using Catalog.Dto;
using Catalog.Interfaces;
using Catalog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReviewController : Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IMapper _mapper;

    public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository,  IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _reviewerRepository = reviewerRepository;
        _pokemonRepository = pokemonRepository;
        _mapper = mapper;
    }
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    public IActionResult GetReviews()
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviews);
    }
    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();
        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(review);
    }

    [HttpGet("pokemon/{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsForAPokemon(int pokeId)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));
        if (!ModelState.IsValid)
            return BadRequest();
        return Ok(reviews); 
    }
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate) 
    {
        if (reviewCreate == null)
            return BadRequest(ModelState);
        var review = _reviewRepository.GetReviews()
            .Where(c => c.Text.Trim().ToUpper() == reviewCreate.Text.TrimEnd().ToUpper())
            .FirstOrDefault();
        if (review != null)
        {
            ModelState.AddModelError("", "Review already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var reviewMap = _mapper.Map<Review>(reviewCreate);
        reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
        reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
      
        if (!_reviewRepository.CreateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Succesfully Created");

    }
    [HttpPut("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]

    public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
    {
        if (updatedReview == null)
            return BadRequest(ModelState);
        if (reviewId != updatedReview.Id)
            return BadRequest(ModelState);
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();
        var reviewMap = _mapper.Map<Review>(updatedReview);
        if (!_reviewRepository.UpdateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong updating review");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    [HttpDelete("{reviewId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();
        var reviewToDelete = _reviewRepository.GetReview(reviewId);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (!_reviewRepository.DeleteReview(reviewToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting review");
        }

        return NoContent();

    }
}