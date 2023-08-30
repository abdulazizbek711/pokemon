using Catalog.Models;

namespace Catalog.Interfaces;

public interface IReviewRepository
{
    ICollection<Review> GetReviews();
    Review GetReview(int reviewId);
    ICollection<Review> GetReviewsOfAPokemon(int pokeId);
    bool ReviewExists(int reviewerId);
    bool CreateReview(Review review);
    bool UpdateReview(Review review);
    bool DeleteReview(Review review);
    bool DeleteReviews(List<Review> reviews);
    bool Save();
}