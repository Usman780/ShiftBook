using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.Models
{
    public class WorkOrderDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public Nullable<int> FollowupParentId { get; set; }
        public string FollowupParentName { get; set; }
        public Nullable<int> EquipmentCodeId { get; set; }
        public string Code { get; set; }
        public Nullable<int> MaintenanceTaskId { get; set; }
        public string TaskDescription { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string OrderType { get; set; }
        public string OrderFor { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> UserId { get; set; }
        public string UserName { get; set; }
        public string OrderDateTime { get; set; }
        public string OrderDescription { get; set; }
        public string OrderStatus { get; set; }
        public string WorkTime { get; set; }
        public string File1Path { get; set; }
        public string File2Path { get; set; }
        public string File3Path { get; set; }
        public string File4Path { get; set; }
        public string File5Path { get; set; }
        public int IsClosed { get; set; }
        public int IsOpen { get; set; }
        public string CreatedAt { get; set; }
        public int Count { get; set; }
        public Nullable<int> Role { get; set; }
    }
}