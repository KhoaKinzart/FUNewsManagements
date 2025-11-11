using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public void AddNewsArticle(NewsArticle newsArticle)
        {
            NewsArticleDAO.AddNewsArticle(newsArticle);
        }

        public void DeleteNewsArticle(string id)
        {
            NewsArticleDAO.DeleteNewsArticle(id);
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return NewsArticleDAO.GetNewsArticles();
        }

        public NewsArticle GetNewsArticleById(string id)
        {
            return NewsArticleDAO.GetNewsArticleById(id);
        }

        public List<NewsArticle> GetNewsArticlesByCategory(short categoryId)
        {
            return NewsArticleDAO.GetNewsArticlesByCategory(categoryId);
        }

        public bool NewsArticleExists(string id)
        {
            return NewsArticleDAO.NewsArticleExists(id);
        }

        public List<NewsArticle> SearchNewsArticles(string keyword)
        {
            return NewsArticleDAO.SearchNewsArticles(keyword);
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            NewsArticleDAO.UpdateNewsArticle(newsArticle);
        }

        public List<NewsArticle> GetActiveNewsArticles()
        {
            return NewsArticleDAO.GetActiveNewsArticles();
        }

        public List<NewsArticle> GetNewsArticlesByCreatedBy(short createdById)
        {
            return NewsArticleDAO.GetNewsArticlesByCreatedBy(createdById);
        }

        public List<object> GetNewsStatisticsByDateRange(DateTime startDate, DateTime endDate)
        {
            return NewsArticleDAO.GetNewsStatisticsByDateRange(startDate, endDate);
        }
    }
}
