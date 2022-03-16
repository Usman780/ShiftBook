using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class TagsDAL
    {
        public List<Tag> GetAllTagsList(DatabaseEntities de)
        {
            return de.Tags.ToList();
        }

        public List<Tag> GetActiveTagsList(DatabaseEntities de)
        {
            return de.Tags.Where(x => x.IsActive == 1).ToList();
        }

        public Tag GetTagById(int id, DatabaseEntities de)
        {
            return de.Tags.Where(x => x.Id == id).FirstOrDefault();
        }

        public Tag GetActiveTagById(int id, DatabaseEntities de)
        {
            return de.Tags.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddTag(Tag Tag, DatabaseEntities de)
        {
            try
            {
                de.Tags.Add(Tag);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddTag2(Tag Tag, DatabaseEntities de)
        {
            try
            {
                de.Tags.Add(Tag);
                de.SaveChanges();

                return Tag.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateTag(Tag Tag, DatabaseEntities de)
        {
            try
            {
                de.Entry(Tag).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTag(int id, DatabaseEntities de)
        {
            try
            {
                Tag tag = de.Tags.Remove(de.Tags.Where(x => x.Id == id).FirstOrDefault());
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