using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class TagsTaskDAL
    {
        public List<TagsTask> GetAllTagsTaskList(DatabaseEntities de)
        {
            return de.TagsTasks.ToList();
        }

        public List<TagsTask> GetActiveTagsTaskList(DatabaseEntities de)
        {
            return de.TagsTasks.Where(x => x.IsActive == 1).ToList();
        }

        public TagsTask GetTagsTaskId(int Id, DatabaseEntities de)
        {
            return de.TagsTasks.Where(x => x.Id == Id).FirstOrDefault();
        }

        public bool AddTagsTask(TagsTask tags, DatabaseEntities de)
        {
            try
            {
                de.TagsTasks.Add(tags);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddTagsTask2(TagsTask tag, DatabaseEntities de)
        {
            try
            {
                de.TagsTasks.Add(tag);
                de.SaveChanges();
                return tag.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateTagsTask(TagsTask tag, DatabaseEntities de)
        {
            try
            {
                de.Entry(tag).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTagsTask(int Id, DatabaseEntities de)
        {
            try
            {
                TagsTask tag = de.TagsTasks.Where(x => x.Id == Id).FirstOrDefault();
                tag.IsActive = 0;
                de.Entry(tag).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}