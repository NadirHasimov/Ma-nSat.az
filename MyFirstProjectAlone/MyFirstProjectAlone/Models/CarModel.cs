using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarController.Models
{
    public class CarModel
    {
        [Required(ErrorMessage = "Avtomobilin tipini daxil edin!")]
        public int TypeOfCarID { get; set; }
        [Required(ErrorMessage = "Avtomobilin markasını daxil edin!")]
        public int MarkID { get; set; }
        [Required(ErrorMessage = "Avtomobilin modelini daxil edin!")]
        public int ModelID { get; set; }
        [Required(ErrorMessage = "Ötürücü tipini daxil edin!")]
        public int Oturucu_ID { get; set; }
        [Required(ErrorMessage = "Mühərrikin  həcmini daxil edin!")]
        public int TypeOfEngineID { get; set; }
        [Required(ErrorMessage = "Mühərrikin növünü daxil edin!")]
        public int TypesOfFuelID { get; set; }
        [Required(ErrorMessage = "Mühərrikin həcmini daxil edin!")]
        public int CapacityOfEngineID { get; set; }
        [Required(ErrorMessage = "Buraxılış ilini daxil edin!")]
        public int ProducedTimeID { get; set; }
    }
}