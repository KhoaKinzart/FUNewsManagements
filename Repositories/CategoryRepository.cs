using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public void AddCategory(Category category)
        {
            CategoryDAO.AddCategory(category);
        }

        public bool CategoryExists(short id)
        {
            return CategoryDAO.CategoryExists(id);
        }

        public void DeleteCategory(short id)
        {
            CategoryDAO.DeleteCategory(id);
        }

        public List<Category> GetActiveCategories()
        {
            return CategoryDAO.GetActiveCategories();
        }

        public List<Category> GetCategories()
        {
            return CategoryDAO.GetCategories();
        }

        public Category GetCategoryById(short id)
        {
            return CategoryDAO.GetCategoryById(id);
        }

        public void UpdateCategory(Category category)
        {
            CategoryDAO.UpdateCategory(category);
        }
    }
}
