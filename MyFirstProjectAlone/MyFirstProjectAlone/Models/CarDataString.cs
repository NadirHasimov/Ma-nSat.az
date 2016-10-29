using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFirstProjectAlone.Models
{
    public class CarDataString
    {
        public string Colour { get; set; }
        public int PowerOfEngine { get; set; }
        public string TypeOfCar { get; set; }
        public string TypeOfEngine { get; set; }
        public string TypeOfFuel { get; set; }
        public int ProducedTime { get; set; }
        public string Oturucu { get; set; }
        public int Price { get; set; }
        public int CapacityOfEngine { get; set; }
        public string MarkName { get; set; }
        public string ModelName { get; set; }
        public string UserName { get; set; }
        public string ABS_string { get; set; }
        public string ArxaGoruntuKamerasi_string { get; set; }
        public string DeriSalon_string { get; set; }
        public string Kondisoner_string { get; set; }
        public string KsenonLampalar_string { get; set; }
        public string Lyuk_string { get; set; }
        public string MerkeziQapanma_string { get; set; }
        public string OturacaqlarinItirilmesi_string { get; set; }
        public string OturacaqlarinVentilyasiyasi_string { get; set; }
        public string ParkRadari_string { get; set; }
        public string YagisSensoru_string { get; set; }
        public string YanPerdeler_string { get; set; }
        public string YungulLEhimliDiskler_string { get; set; }
    }
    public class FilterResponseModel
    {
        public IEnumerable<CarDataString> Cars { get; set; }
        public int CountOfCars { get; set; }
    }
    public class FilterModel
    {
        public IEnumerable<int> TypeOfCar_IDs;

        public IEnumerable<int> Mark_IDs;

        public IEnumerable<int> Model_IDs;

        public IEnumerable<int> Oturucu_IDs;

        public IEnumerable<int> TypeEngine_IDs;

        public IEnumerable<int> TypesOfFuel_IDs;

        public IEnumerable<int> CapacityOfEngine_IDs;

        public IEnumerable<int> ProducedTime_IDs;

        public IEnumerable<int> ColourIDs;

        public IEnumerable<string> SearchText;

        public int LowPirce { get; set; }
        public int HighPrice { get; set; }

        public int MinProducedTimeID { get; set; }
        public int MaxProducedTimeID { get; set; }

        public int LowCapacityOfEngineID { get; set; }
        public int HighCapacityOfEngineID { get; set; }
        public int PageNumber { get; set; }

        public int PageLength { get; set; }


        
    }

    public class PagenationModel
    {
        public int PageNumber { get; set; }

        public int PageLength { get; set; }
    }
}