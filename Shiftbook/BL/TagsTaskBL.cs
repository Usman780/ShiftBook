using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class TagsTaskBL
    {
        public List<TagsTask> GetAllTagsTaskList(DatabaseEntities de)
        {
            return new TagsTaskDAL().GetAllTagsTaskList(de);
        }

        public List<TagsTask> GetActiveTagsTaskList(DatabaseEntities de)
        {
            return new TagsTaskDAL().GetActiveTagsTaskList(de);
        }

        public TagsTask GetTagsTaskById(int Id, DatabaseEntities de)
        {
            return new TagsTaskDAL().GetTagsTaskId(Id, de);
        }

        public bool AddTagsTask(TagsTask tag, DatabaseEntities de)
        {
            return new TagsTaskDAL().AddTagsTask(tag, de);
        }

        public int AddTagsTask2(TagsTask tag, DatabaseEntities de)
        {
            return new TagsTaskDAL().AddTagsTask2(tag, de);
        }

        public bool UpdateTagsTask(TagsTask tag, DatabaseEntities de)
        {
            return new TagsTaskDAL().UpdateTagsTask(tag, de);
        }

        public bool DeleteTagTask(int Id, DatabaseEntities de)
        {
            return new TagsTaskDAL().DeleteTagsTask(Id, de);
        }
    }
}