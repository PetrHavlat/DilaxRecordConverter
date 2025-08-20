using System;

namespace DilaxRecordConverter.Core
{
    /// <summary>
    /// Reprezentuje konfiguraci jedněch dveří v APC systému.
    /// </summary>
    public class DoorConfiguration
    {
        /// <summary>
        /// Získá nebo nastaví unikátní ID zařízení PCU nebo TSL.
        /// </summary>
        public uint DeviceId { get; set; }

        /// <summary>
        /// Získá nebo nastaví instanci dveří na zařízení PCU nebo TSL, začínající od nuly pro první dveře.
        /// </summary>
        public byte Instance { get; set; }

        /// <summary>
        /// Získá nebo nastaví identifikaci zařízení PCU nebo TSL, včetně čísla verze.
        /// </summary>
        public string DeviceModel { get; set; }

        /// <summary>
        /// Získá nebo nastaví unikátní název dveří v rámci vozidla.
        /// </summary>
        public string DoorName { get; set; }

        /// <summary>
        /// Získá nebo nastaví identifikátor vozidla.
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// Získá nebo nastaví typ vozidla.
        /// </summary>
        public string VehicleType { get; set; }

        /// <summary>
        /// Získá nebo nastaví název operátora.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Vrací řetězcovou reprezentaci konfigurace dveří.
        /// </summary>
        public override string ToString()
        {
            return $"Dveře {DoorName} (DeviceId: {DeviceId}, Instance: {Instance})";
        }
    }
}

