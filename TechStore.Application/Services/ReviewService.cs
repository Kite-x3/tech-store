using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStore.Application.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;

        public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
        }
        /// <summary>
        /// Получает отзывы для товара
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>Список DTO отзывов</returns>
        /// <exception cref="KeyNotFoundException">Если товар не найден</exception>
        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found.");

            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
            return reviews.Select(r => new ReviewDto
            {
                Id = r.ReviewId,
                Author = r.Author,
                Rating = r.Rating,
                Comment = r.Comment,
                Date = r.Date,
                ProductId = r.ProductId
            });
        }
        /// <summary>
        /// Получает отзыв по ID
        /// </summary>
        /// <param name="reviewId">ID отзыва</param>
        /// <returns>DTO отзыва</returns>
        /// <exception cref="KeyNotFoundException">Если отзыв не найден</exception>
        public async Task<ReviewDto> GetReviewByIdAsync(int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found.");

            return new ReviewDto
            {
                Id = review.ReviewId,
                Author = review.Author,
                Rating = review.Rating,
                Comment = review.Comment,
                Date = review.Date,
                ProductId = review.ProductId
            };
        }
        /// <summary>
        /// Создает новый отзыв
        /// </summary>
        /// <param name="reviewDto">DTO с данными отзыва</param>
        /// <exception cref="ArgumentNullException">Если данные отзыва не указаны</exception>
        /// <exception cref="ArgumentException">Если товар не существует</exception>
        public async Task CreateReviewAsync(ReviewDto reviewDto, string userId)
        {
            if (reviewDto == null)
                throw new ArgumentNullException(nameof(reviewDto));

            var product = await _productRepository.GetProductByIdAsync(reviewDto.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with ID {reviewDto.ProductId} does not exist.");

            var review = new Review
            {
                Author = reviewDto.Author,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                Date = DateTime.UtcNow,
                ProductId = reviewDto.ProductId,
                AuthorID = userId
            };

            await _reviewRepository.CreateReviewAsync(review);
            reviewDto.Id = review.ReviewId;
        }
        /// <summary>
        /// Удаляет отзыв
        /// </summary>
        /// <param name="reviewId">ID отзыва</param>
        /// <exception cref="KeyNotFoundException">Если отзыв не найден</exception>
        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found.");

            await _reviewRepository.DeleteReviewAsync(reviewId);
        }
    }
}