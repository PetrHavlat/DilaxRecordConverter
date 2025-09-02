using System;
using System.Collections.Generic;

namespace DilaxRecordConverter.Core
{
    /// <summary>
    /// Poskytuje informace o diagnostických kódech a jejich popisech.
    /// </summary>
    public static class DiagnosticCodes
    {
        // Kategorie zpráv
        private static readonly Dictionary<byte, string> Categories = new Dictionary<byte, string>
        {
            { 2, "Warning" },
            { 3, "Error" }
        };

        // Moduly
        private static readonly Dictionary<byte, string> Modules = new Dictionary<byte, string>
        {
            { 11, "PCU or BBM-WEB device (master)" },
            { 20, "PCU or TSL device (client)" }
        };

        // Podmoduly pro modul 11
        private static readonly Dictionary<byte, string> SubModules11 = new Dictionary<byte, string>
        {
            { 0, "Device" },
            { 1, "Signals" }
        };

        // Podmoduly pro modul 20
        private static readonly Dictionary<byte, string> SubModules20 = new Dictionary<byte, string>
        {
            { 0, "Door" }
        };

        // Zprávy pro modul 11, podmodul 0 (Device)
        private static readonly Dictionary<byte, string> Messages11_0 = new Dictionary<byte, string>
        {
            { 0, "Battery is empty" },
            { 1, "No GPS signal reception" },
            { 2, "GPS receiver failure" }
        };

        // Zprávy pro modul 11, podmodul 1 (Signals)
        private static readonly Dictionary<byte, string> Messages11_1 = new Dictionary<byte, string>
        {
            { 0, "Odometer signal failure" },
            { 1, "Driving signal failure" },
            { 2, "Doors enabled while vehicle is driving" },
            { 3, "Odometer contradicts standstill signal" },
            { 4, "Standstill signal failure" },
            { 5, "'At stop' signal activated while vehicle is driving" }
        };

        // Zprávy pro modul 20, podmodul 0 (Door)
        private static readonly Dictionary<byte, string> Messages20_0 = new Dictionary<byte, string>
        {
            { 1, "The PCU or TSL device is not responding" },
            { 2, "Not enough nodes connected to the SSL bus" },
            { 3, "Flickering sensor" },
            { 4, "SSL bus is not operational" },
            { 5, "Sensor or door is not operating correctly" },
            { 6, "Configuration is erroneous" },
            { 7, "Door signal unavailable" },
            { 8, "Blocked sensor" },
            { 10, "Sensor optic has not detected anything for a long time" },
            { 11, "Door contact has not changed its state for a long time" },
            { 12, "Sensor has not detected any valid event for a long time" },
            { 13, "Counter (counting input) has not counted any valid event for a long time" },
            { 20, "Door is erroneously reported as open" }
        };

        /// <summary>
        /// Získá popis kategorie podle ID.
        /// </summary>
        /// <param name="category">ID kategorie.</param>
        /// <returns>Popis kategorie.</returns>
        public static string GetCategoryDescription(byte category)
        {
            if (Categories.TryGetValue(category, out string? description))
                return description;

            return $"Unknown Category ({category})";
        }

        /// <summary>
        /// Získá popis modulu podle ID.
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <returns>Popis modulu.</returns>
        public static string GetModuleDescription(byte moduleId)
        {
            if (Modules.TryGetValue(moduleId, out string? description))
                return description;

            return $"Unknown Module ({moduleId})";
        }

        /// <summary>
        /// Získá popis podmodulu podle ID modulu a ID podmodulu.
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <param name="subModuleId">ID podmodulu.</param>
        /// <returns>Popis podmodulu.</returns>
        public static string GetSubModuleDescription(byte moduleId, byte subModuleId)
        {
            if (moduleId == 11 && SubModules11.TryGetValue(subModuleId, out string? description11))
                return description11;

            if (moduleId == 20 && SubModules20.TryGetValue(subModuleId, out string? description20))
                return description20;

            return $"Unknown SubModule ({subModuleId})";
        }

        /// <summary>
        /// Získá popis zprávy podle ID modulu, ID podmodulu a ID zprávy.
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <param name="subModuleId">ID podmodulu.</param>
        /// <param name="messageId">ID zprávy.</param>
        /// <returns>Popis zprávy.</returns>
        public static string GetMessageDescription(byte moduleId, byte subModuleId, byte messageId)
        {
            if (moduleId == 11)
            {
                if (subModuleId == 0 && Messages11_0.TryGetValue(messageId, out string? description11_0))
                    return description11_0;

                if (subModuleId == 1 && Messages11_1.TryGetValue(messageId, out string? description11_1))
                    return description11_1;
            }
            else if (moduleId == 20)
            {
                if (subModuleId == 0 && Messages20_0.TryGetValue(messageId, out string? description20_0))
                    return description20_0;
            }

            return $"Unknown Message (Module: {moduleId}, SubModule: {subModuleId}, Message: {messageId})";
        }

        /// <summary>
        /// Získá, zda zpráva obsahuje informace o zařízení (Module ID = 20).
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <returns>True, pokud zpráva obsahuje informace o zařízení, jinak false.</returns>
        public static bool HasDeviceInfo(byte moduleId)
        {
            return moduleId == 20;
        }

        /// <summary>
        /// Získá, zda zpráva obsahuje časové razítko místo kódu chyby.
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <param name="subModuleId">ID podmodulu.</param>
        /// <param name="messageId">ID zprávy.</param>
        /// <returns>True, pokud zpráva obsahuje časové razítko, jinak false.</returns>
        public static bool HasTimeStampInfo(byte moduleId, byte subModuleId, byte messageId)
        {
            return moduleId == 20 && subModuleId == 0 && (messageId == 10 || messageId == 11 || messageId == 12 || messageId == 13);
        }

        /// <summary>
        /// Získá, zda zpráva obsahuje informace o pozici SSL.
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <param name="subModuleId">ID podmodulu.</param>
        /// <param name="messageId">ID zprávy.</param>
        /// <returns>True, pokud zpráva obsahuje informace o pozici SSL, jinak false.</returns>
        public static bool HasSslPositionInfo(byte moduleId, byte subModuleId, byte messageId)
        {
            return moduleId == 20 && subModuleId == 0 && (messageId == 2 || messageId == 3 || messageId == 5 || messageId == 8);
        }

        /// <summary>
        /// Získá, zda zpráva je varování.
        /// </summary>
        /// <param name="category">Kategorie zprávy.</param>
        /// <returns>True, pokud zpráva je varování, jinak false.</returns>
        public static bool IsWarning(byte category)
        {
            return category == 2;
        }

        /// <summary>
        /// Získá, zda zpráva je chyba.
        /// </summary>
        /// <param name="category">Kategorie zprávy.</param>
        /// <returns>True, pokud zpráva je chyba, jinak false.</returns>
        public static bool IsError(byte category)
        {
            return category == 3;
        }

        /// <summary>
        /// Získá popis diagnostické zprávy včetně všech relevantních informací.
        /// </summary>
        /// <param name="moduleId">ID modulu.</param>
        /// <param name="subModuleId">ID podmodulu.</param>
        /// <param name="messageId">ID zprávy.</param>
        /// <param name="category">Kategorie zprávy.</param>
        /// <param name="message">Text zprávy.</param>
        /// <returns>Kompletní popis diagnostické zprávy.</returns>
        public static string GetFullMessageDescription(byte moduleId, byte subModuleId, byte messageId, byte category, string message)
        {
            string baseDescription = GetMessageDescription(moduleId, subModuleId, messageId);
            string categoryDescription = GetCategoryDescription(category);
            string moduleDescription = GetModuleDescription(moduleId);
            string subModuleDescription = GetSubModuleDescription(moduleId, subModuleId);

            string result = $"{baseDescription} ({categoryDescription})";

            if (HasDeviceInfo(moduleId) && !string.IsNullOrEmpty(message))
            {
                result += $"\nMessage: {message}";
            }

            result += $"\nModule: {moduleDescription}, SubModule: {subModuleDescription}";

            return result;
        }
    }
}
