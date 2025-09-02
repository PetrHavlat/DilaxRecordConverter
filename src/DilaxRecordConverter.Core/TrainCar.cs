namespace DilaxRecordConverter.Core
{
    /// <summary>
    /// Reprezentuje jeden vůz ve vlaku.
    /// </summary>
    public class TrainCar
    {
        /// <summary>
        /// Získá nebo nastaví identifikátor vozidla.
        /// </summary>
        public string? VehicleId { get; set; }

        /// <summary>
        /// Získá nebo nastaví typ vozidla.
        /// </summary>
        public string? VehicleType { get; set; }

        /// <summary>
        /// Získá nebo nastaví název operátora.
        /// </summary>
        public string? Operator { get; set; }

        /// <summary>
        /// Vrací řetězcovou reprezentaci vozu.
        /// </summary>
        public override string ToString()
        {
            return $"Vůz: ID={VehicleId}, Typ={VehicleType}, Operátor={Operator}";
        }
    }
}