using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class TagsBL
    {
        public List<Tag> GetAllTagsList(DatabaseEntities de)
        {
            return new TagsDAL().GetAllTagsList(de);
        }

        public List<Tag> GetActiveTagsList(DatabaseEntities de)
        {
            return new TagsDAL().GetActiveTagsList(de);
        }

        public Tag GetTagById(int id, DatabaseEntities de)
        {
            return new TagsDAL().GetTagById(id, de);
        }

        public Tag GetActiveTagById(int id, DatabaseEntities de)
        {
            return new TagsDAL().GetActiveTagById(id, de);
        }

        public bool AddTag(Tag Tag, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Tag.TagName))
            {
                return false;
            }
            else
            {
                return new TagsDAL().AddTag(Tag, de);
            }
        }

        public int AddTag2(Tag Tag, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Tag.TagName))
            {
                return -1;
            }
            else
            {
                return new TagsDAL().AddTag2(Tag, de);
            }
        }


        public bool UpdateTag(Tag Tag, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Tag.TagName))
            {
                return false;
            }
            else
            {
                return new TagsDAL().UpdateTag(Tag, de);
            }
        }

        public bool DeleteTag(int id, DatabaseEntities de)
        {
            return new TagsDAL().DeleteTag(id, de);
        }
    }
}