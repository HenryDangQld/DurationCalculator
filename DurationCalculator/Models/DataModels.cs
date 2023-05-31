using System;
namespace DurationCalculator.Models
{
    public class jobtiming
    {
        public string jobno { get; set; }
        public string operatorID { get; set; }
        public string jobday { get; set; }
        public string jobtime { get; set; }
        public int? id { get; set; }
        public int? stationNo { get; set; }
        public string duration { get; set; }
        public string filename { get; set; }
        public string handle { get; set; }
        public string itemno { get; set; }
        public string storageInfo { get; set; }
        public string packingID { get; set; }
        public string resetDay { get; set; }
        public string resetTime { get; set; }
    }

    public class stationManagement
    {
        public string stationName { get; set; }
        public int? deviceID { get; set; }
        public int? stationGroup { get; set; }
        public string stationStatus { get; set; }
        public int? stationNo { get; set; }
        public string dispatchStatus { get; set; }
        public int? updateByJobNo { get; set; }
        public int? updateByItemNo { get; set; }
    }
}
