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

        [Required(ErrorMessage = "Avtomobilin növünü daxil edin!")]
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

        [Required(ErrorMessage = "Avtomobilin qiymətini daxil edin!")]
        public int Price { get; set; }

        [Required(ErrorMessage ="Mühərrikin gücünü daxil edin!")]
        public int PowerOfEngine { get; set; }
        [Required(ErrorMessage ="Maşının rəngini daxil edin!")]
        public int ColourID { get; set; }

        public int ABS_ID { get; set; }

        public int ArxaGoruntuKamerasi_ID { get; set; }

        public int DeriSalon_ID { get; set; }

        public int Kondisoner_ID { get; set; }

        public int KsenonLampalar_ID { get; set; }

        public int Lyuk_ID { get; set; }

        public int MerkeziQapanma_ID { get; set; }

        public int OturacaqlarinItirilmesi_ID { get; set; }

        public int OturacaqlarinVentilyasiyasi_ID { get; set; }

        public int ParkRadari_ID { get; set; }

        public int YagisSensoru_ID { get; set; }

        public int YanPerdeler_ID { get; set; }

        public int YungulLEhimliDiskler_ID { get; set; }
    }
}

