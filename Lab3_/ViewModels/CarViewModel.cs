using System.ComponentModel;

namespace Lab3_.ViewModels
{
    public class CarViewModel
    {
        [DisplayName("#")] public int Id { get; set; }

        [DisplayName("Марка")] public string Mark { get; set; }

        [DisplayName("Организация")] public string Organization { get; set; }

        [DisplayName("Максимальный вес груза")]
        public int CarryingWeight { get; set; }

        [DisplayName("Максимальный объем груза")]
        public int CarryingVolume { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}