using System;
using System.Collections.Generic;

namespace DilaxRecordConverter.Core
{
    /// <summary>
    /// Poskytuje předdefinované typy událostí a metody pro práci s nimi.
    /// </summary>
    public static class EventTypes
    {
        // Konstanty pro často používané typy událostí
        public const byte Reserved = 0;
        public const byte VehicleStopped = 1;
        public const byte VehicleDriving = 2;
        public const byte DoorsEnabled = 3;
        public const byte DoorsLocked = 4;
        public const byte DrivingDirection1 = 5;
        public const byte DrivingDirection0 = 6;
        public const byte VehicleEmpty = 7;
        public const byte PowerOff = 8;
        public const byte PowerOn = 9;
        public const byte ScheduledStopReached = 10;
        public const byte ScheduledStopLeft = 11;

        public const byte WheelchairOnBoard = 100;
        public const byte WheelchairUnloaded = 101;
        public const byte WheelchairRampAccessed = 102;

        public const byte BicycleOnBoard = 110;
        public const byte BicycleUnloaded = 111;
        public const byte BicycleRackAccessed = 112;

        public const byte LuggageOnBoard = 120;
        public const byte LuggageUnloaded = 121;
        public const byte LuggageCompartmentAccessed = 122;

        public const byte LavatoryOccupied = 130;
        public const byte LavatoryFree = 131;
        public const byte LavatoryAccessed = 132;

        public const byte AuxOn = 140;
        public const byte AuxOff = 141;
        public const byte AuxActivated = 142;

        public const byte TicketSold = 152;
        public const byte TicketCancelled = 162;

        public const byte EngineStarted = 170;
        public const byte EngineStopped = 171;

        // Slovník obsahující všechny předdefinované typy událostí
        private static readonly Dictionary<byte, string> EventDescriptions = new Dictionary<byte, string>
        {
            { Reserved, "Reserved (placeholder only)" },
            { VehicleStopped, "Vehicle stopped" },
            { VehicleDriving, "Vehicle driving" },
            { DoorsEnabled, "Doors enabled" },
            { DoorsLocked, "Doors locked" },
            { DrivingDirection1, "Driving direction 1" },
            { DrivingDirection0, "Driving direction 0" },
            { VehicleEmpty, "Vehicle empty" },
            { PowerOff, "Power off" },
            { PowerOn, "Power on" },
            { ScheduledStopReached, "Scheduled stop reached" },
            { ScheduledStopLeft, "Scheduled stop left" },

            { WheelchairOnBoard, "Wheelchair on board" },
            { WheelchairUnloaded, "Wheelchair unloaded" },
            { WheelchairRampAccessed, "Wheelchair ramp accessed" },

            { BicycleOnBoard, "Bicycle on board" },
            { BicycleUnloaded, "Bicycle unloaded" },
            { BicycleRackAccessed, "Bicycle rack accessed" },

            { LuggageOnBoard, "Luggage on board" },
            { LuggageUnloaded, "Luggage unloaded" },
            { LuggageCompartmentAccessed, "Luggage compartment accessed" },

            { LavatoryOccupied, "Lavatory occupied" },
            { LavatoryFree, "Lavatory free" },
            { LavatoryAccessed, "Lavatory accessed" },

            { AuxOn, "AUX ON" },
            { AuxOff, "AUX OFF" },
            { AuxActivated, "AUX activated" },

            { TicketSold, "Ticket sold" },
            { TicketCancelled, "Ticket cancelled" },

            { EngineStarted, "Engine started" },
            { EngineStopped, "Engine stopped" }
        };

        /// <summary>
        /// Získá popis události podle ID.
        /// </summary>
        /// <param name="eventId">ID události.</param>
        /// <returns>Popis události.</returns>
        public static string GetEventDescription(byte eventId)
        {
            if (EventDescriptions.TryGetValue(eventId, out string? description))
                return description;

            if (eventId >= 12 && eventId <= 99)
                return "Reserved for future use";

            return "User-defined event";
        }

        /// <summary>
        /// Získá, zda je událost povinná.
        /// </summary>
        /// <param name="eventId">ID události.</param>
        /// <returns>True, pokud je událost povinná, jinak false.</returns>
        public static bool IsRequiredEvent(byte eventId)
        {
            return eventId == ScheduledStopReached || eventId == ScheduledStopLeft;
        }

        /// <summary>
        /// Získá, zda je událost související se zastávkou.
        /// </summary>
        /// <param name="eventId">ID události.</param>
        /// <returns>True, pokud je událost související se zastávkou, jinak false.</returns>
        public static bool IsStopEvent(byte eventId)
        {
            return eventId == ScheduledStopReached || eventId == ScheduledStopLeft;
        }
    }
}


