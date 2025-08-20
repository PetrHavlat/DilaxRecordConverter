using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DilaxRecordConverter.Core;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
    /// <summary>
    /// Reprezentuje CONF blok, který obsahuje konfigurační informace o dveřích v APC systému.
    /// </summary>
    public class ConfBlock : Dlx3Block
	{
		/// <summary>
		/// Získá časové razítko, kdy byl tento blok zapsán.
		/// </summary>
		public uint CurrentTime { get; private set; }

		/// <summary>
		/// Získá seznam konfigurací dveří.
		/// </summary>
		public List<DoorConfiguration> Doors { get; } = new List<DoorConfiguration>();

		/// <summary>
		/// Parsuje binární data CONF bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 4)
			{
				Console.WriteLine("Varování: CONF blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Načtení časového razítka
					CurrentTime = Dlx3Pomocnik.CtiUIntBigEndian(reader);
					//if (Dlx3Pomocnik.CASY_PRIJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(CurrentTime).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen příjezd");
					//if (Dlx3Pomocnik.CASY_ODJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(CurrentTime).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen odjezd");

					// Načítání opakujících se záznamů dveří
					while (ms.Position < ms.Length)
					{
						try
						{
							// Kontrola, zda máme dostatek dat pro DeviceId (4 bajty) a Instance (1 bajt)
							if (ms.Length - ms.Position < 5)
							{
								Console.WriteLine("Varování: Nedostatek dat pro načtení další konfigurace dveří.");
								break;
							}

							var door = new DoorConfiguration
							{
								DeviceId = Dlx3Pomocnik.CtiUIntBigEndian(reader),
								Instance = reader.ReadByte()
							};

							// Čtení řetězců s kontrolou dostupnosti dat
							if (ms.Position < ms.Length)
								door.DeviceModel = Dlx3Pomocnik.ReadNullTerminatedString(reader);
							else
								break;

							if (ms.Position < ms.Length)
								door.DoorName = Dlx3Pomocnik.ReadNullTerminatedString(reader);
							else
								break;

							if (ms.Position < ms.Length)
								door.VehicleId = Dlx3Pomocnik.ReadNullTerminatedString(reader);
							else
								break;

							if (ms.Position < ms.Length)
								door.VehicleType = Dlx3Pomocnik.ReadNullTerminatedString(reader);
							else
								break;

							if (ms.Position < ms.Length)
								door.Operator = Dlx3Pomocnik.ReadNullTerminatedString(reader);
							else
								break;

							// Validace dat
							if (ValidateDoorConfiguration(door))
							{
								Doors.Add(door);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Chyba při parsování konfigurace dveří: {ex.Message}");
							// Pokračujeme s další konfigurací dveří, neshazujeme celý proces
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování CONF bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Validuje konfiguraci dveří.
		/// </summary>
		/// <param name="door">Konfigurace dveří k validaci.</param>
		/// <returns>True, pokud je konfigurace validní, jinak false.</returns>
		private bool ValidateDoorConfiguration(DoorConfiguration door)
		{
			// Základní validace - kontrola, zda nejsou null nebo prázdné řetězce
			if (string.IsNullOrEmpty(door.DeviceModel) ||
				string.IsNullOrEmpty(door.DoorName) ||
				string.IsNullOrEmpty(door.VehicleId) ||
				string.IsNullOrEmpty(door.VehicleType) ||
				string.IsNullOrEmpty(door.Operator))
			{
				Console.WriteLine("Varování: Konfigurace dveří obsahuje prázdné nebo null řetězce.");
				return false;
			}

			return true;
		}
	}
}

