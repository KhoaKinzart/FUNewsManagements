using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        public void AddTag(Tag tag)
        {
            TagDAO.AddTag(tag);
        }

        public void DeleteTag(int id)
        {
            TagDAO.DeleteTag(id);
        }

        public List<Tag> GetTags()
        {
            return TagDAO.GetTags();
        }

        public List<Tag> Search(string keyword)
        {
            return TagDAO.GetTags().Where(t => t.TagName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void UpdateTag(Tag tag)
        {
            TagDAO.UpdateTag(tag);
        }

        public Tag GetTagById(int id)
        {
            return TagDAO.GetTagById(id);
        }
    }
}
