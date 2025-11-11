using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class CategoryDAO
    {
        public static List<Category> GetCategories()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.Categories
                    .Include(c => c.ParentCategory)
                    .ToList();
            }
        }
        public static void AddCategory(Category category)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }
        public static void UpdateCategory(Category category)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.Categories.Update(category);
                context.SaveChanges();
            }
        }
        public static void DeleteCategory(short id)
        {
            using (var context = new FuNewsManagementContext())
            {
                var category = context.Categories.Find(id);
                if (category != null)
                {
                    context.Categories.Remove(category);
                    context.SaveChanges();
                }
            }
        }
        public static Category GetCategoryById(short id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.Categories
                    .Include(c => c.ParentCategory)
                    .FirstOrDefault(c => c.CategoryID == id);
            }
        }
        public static bool CategoryExists(short id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.Categories.Any(c => c.CategoryID == id);
            }
        }
        public static List<Category> GetActiveCategories()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.Categories.Where(c => c.IsActive == true).ToList();
            }
        }
    }
}
