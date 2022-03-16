using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.Models
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public int EquipmentCodeId { get; set; }
        public string EquipmentCode { get; set; }
        public string AssetDescription { get; set; }
        public string Pid { get; set; }
        public string SystemDescription { get; set; }
        public string MaintenanceType { get; set; }
        public string TaskDescription { get; set; }
        public string Comment { get; set; }
        public string Interval { get; set; }
        public string Unit { get; set; }
        public string PlantShutDownJob { get; set; }
        public string AtexZone { get; set; }
        public string AuxiliaryMaterial { get; set; }
        public string GreaseOilManufacturer { get; set; }
        public string GreaseOilType { get; set; }
        public string TopupAmount { get; set; }
        public string RoutineType { get; set; }
    }
}