using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class TagDAO
    {
        public static List<Tag> GetTags()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.Tags.Include(t => t.NewsArticles).ToList();
            }
        }

        public static Tag GetTagById(int id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.Tags.Find(id);
            }

        }

        public static void AddTag(string tagName)
        {
            using (var context = new FuNewsManagementContext())
            {
                var tag = new Tag { TagName = tagName };
                context.Tags.Add(tag);
                context.SaveChanges();
            }
        }

        public static void AddTag(Tag tag)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.Tags.Add(tag);
                context.SaveChanges();
            }
        }

        public static void UpdateTag(Tag tag)
        {
            using (var context = new FuNewsManagementContext())
            {
                var existingTag = context.Tags
                    .Include(t => t.NewsArticles)
                    .FirstOrDefault(t => t.TagID == tag.TagID);

                if (existingTag != null)
                {
                    // Update basic properties
                    existingTag.TagName = tag.TagName;
                    existingTag.Note = tag.Note;

                    // Update relationships with articles
                    existingTag.NewsArticles.Clear();

                    // Add the articles from the updated tag
                    foreach (var article in tag.NewsArticles)
                    {
                        var dbArticle = context.NewsArticles.Find(article.NewsArticleID);
                        if (dbArticle != null)
                        {
                            existingTag.NewsArticles.Add(dbArticle);
                        }
                    }

                    context.SaveChanges();
                }
            }
        }

        public static void DeleteTag(string tagName)
        {
            using (var context = new FuNewsManagementContext())
            {
                var tag = context.Tags.FirstOrDefault(t => t.TagName == tagName);
                if (tag != null)
                {
                    context.Tags.Remove(tag);
                    context.SaveChanges();
                }
            }
        }

        public static void DeleteTag(int tagId)
        {
            using (var context = new FuNewsManagementContext())
            {
                var tag = context.Tags
                    .Include(t => t.NewsArticles)
                    .FirstOrDefault(t => t.TagID == tagId);
                    
                if (tag != null)
                {
                    // Clear relationships first
                    tag.NewsArticles.Clear();
                    
                    // Then remove the tag
                    context.Tags.Remove(tag);
                    context.SaveChanges();
                }
            }
        }
    }
}
