using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.Helping_Classes
{
    public class DTO
    {
        public int count { get; set; }
        public int Id { get; set; }
        public string EncId { get; set; }
        public string Name { get; set; }
        public string Location{ get; set; }
        public int Role { get; set; }

        //Type DTO
        public int TypeId { get; set; }
        public string TypeEncId { get; set; }
        public string TypeName { get; set; }

        //Tag DTO

        public int TagId { get; set; }
        public string TagEncId { get; set; }
        public string TagName { get; set; }

        //history Dto

        public int HId { get; set; }
        public string HEncId { get; set; }
        public string HStatus { get; set; }
        public string HCreated { get; set; }
        public string HCode { get; set; }
        public string HCreatedAt { get; set; }

        //EquipmentCode Dto

        public int EId { get; set; }
        public string EEncId { get; set; }
        public string ECode { get; set; }
        public string EEquipmentName { get; set; }
        public string EDescription { get; set; }
        public string ECreated { get; set; }
        public string ECreatedAt { get; set; }
    }
}