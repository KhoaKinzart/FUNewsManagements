using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class NewsArticleDAO
    {
        public static List<NewsArticle> GetNewsArticles()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags)
                    .ToList();
            }
        }
        public static void AddNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.NewsArticles.Add(newsArticle);
                context.SaveChanges();
            }
        }
        public static void UpdateNewsArticle(NewsArticle newsArticle)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.NewsArticles.Update(newsArticle);
                context.SaveChanges();
            }
        }
        public static void DeleteNewsArticle(string id)
        {
            using (var context = new FuNewsManagementContext())
            {
                var newsArticle = context.NewsArticles
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleID == id);

                if (newsArticle != null)
                {
                    // Clear the many-to-many relationships first
                    newsArticle.Tags.Clear();

                    // Save changes to remove the junction table entries
                    context.SaveChanges();

                    // Now remove the actual article
                    context.NewsArticles.Remove(newsArticle);
                    context.SaveChanges();
                }
            }
        }
        public static NewsArticle GetNewsArticleById(string id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleID == id);
            }
        }
        public static bool NewsArticleExists(string id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles.Any(n => n.NewsArticleID.Equals(id));
            }
        }
        public static List<NewsArticle> GetNewsStatus()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles.Where(n => n.NewsStatus == true).ToList();
            }
        }
        public static List<NewsArticle> SearchNewsArticles(string keyword)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles
                    .Where(n => n.NewsTitle.Contains(keyword) || n.NewsTitle.Contains(keyword))
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags)
                    .ToList();
            }
        }

        public static List<NewsArticle> GetActiveNewsArticles()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles
                    .Where(n => n.NewsStatus == true)
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags)
                    .ToList();
            }
        }
        public static List<NewsArticle> GetNewsArticlesByCategory(short categoryId)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles
                    .Where(n => n.NewsArticleID.Equals(categoryId))
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags)
                    .ToList();
            }
        }

        public static List<NewsArticle> GetNewsArticlesByCreatedBy(short createdById)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.NewsArticles
                    .Where(n => n.CreatedByID == createdById)
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags)
                    .ToList();
            }
        }

        public static List<object> GetNewsStatisticsByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var context = new FuNewsManagementContext())
            {
                var articles = context.NewsArticles
                    .Where(n => n.CreatedDate.HasValue && 
                               n.CreatedDate.Value.Date >= startDate.Date && 
                               n.CreatedDate.Value.Date <= endDate.Date)
                    .ToList();

                var statistics = articles
                    .GroupBy(n => n.CreatedDate.Value.Date)
                    .Select(g => new {
                        Date = g.Key,
                        DateString = g.Key.ToString("yyyy-MM-dd"),
                        TotalArticles = g.Count(),
                        ActiveArticles = g.Count(n => n.NewsStatus == true),
                        InactiveArticles = g.Count(n => n.NewsStatus == false)
                    })
                    .OrderByDescending(s => s.Date)
                    .Cast<object>()
                    .ToList();

                return statistics;
            }
        }
    }
}
