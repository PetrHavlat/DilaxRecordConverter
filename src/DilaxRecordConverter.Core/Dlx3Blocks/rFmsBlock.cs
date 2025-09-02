using DilaxRecordConverter.Core;
using DilaxRecordConverter.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	/// <summary>
	/// Reprezentuje rFMS blok, který obsahuje data z Fleet Management System.
	/// </summary>
	public class rFmsBlock : Dlx3Block
	{
		/// <summary>
		/// Typy protokolů pro rFMS blok.
		/// </summary>
		public enum ProtocolType : byte
		{
			/// <summary>
			/// Neznámý protokol (není zpracováván DILAX Data Management Software).
			/// </summary>
			Unknown = 0,

			/// <summary>
			/// CAN-FMS protokol (klíč:hodnota).
			/// </summary>
			CanFms = 1,

			/// <summary>
			/// 1-Wire-FMS protokol (klíč:hodnota).
			/// </summary>
			OneWireFms = 2,

			/// <summary>
			/// CSV seznam (specifický pro projekt).
			/// </summary>
			Csv = 3
		}

		/// <summary>
		/// Získá časové razítko, kdy se informace stala platnou.
		/// </summary>
		public uint Timestamp { get; private set; }

		/// <summary>
		/// Získá typ protokolu.
		/// </summary>
		public ProtocolType Type { get; private set; }

		/// <summary>
		/// Získá zprávu jako řetězec.
		/// </summary>
		public string? Message { get; private set; }

		/// <summary>
		/// Získá původní binární data FMS.
		/// </summary>
		public byte[]? RawFmsData { get; private set; }

		/// <summary>
		/// Získá slovník klíč-hodnota pro typy protokolů 1 (CAN-FMS) a 2 (1-Wire-FMS).
		/// </summary>
		public Dictionary<string, string> KeyValuePairs { get; } = new Dictionary<string, string>();

		/// <summary>
		/// Získá datum a čas, kdy se informace stala platnou, jako DateTime.
		/// </summary>
		public DateTime TimestampDateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;

		/// <summary>
		/// Parsuje binární data rFMS bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 5) // Minimálně potřebujeme 5 bajtů (4+1)
			{
				Console.WriteLine("Varování: rFMS blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					Timestamp = BinaryHelper.ReadUIntValue(reader);

					// Uložení původních dat pro zpětnou kompatibilitu
					var dataLen = data.Length - 4;
					if (dataLen > 0)
					{
						RawFmsData = new byte[dataLen];
						Array.Copy(data, 4, RawFmsData, 0, dataLen);
					}
					else
					{
						RawFmsData = new byte[0];
					}

					// Pokud máme dostatek dat, načteme typ protokolu a zprávu
					if (dataLen > 0)
					{
						Type = (ProtocolType)reader.ReadByte();

						// Načtení zprávy
						if (ms.Position < ms.Length)
						{
							Message = BinaryHelper.ReadStringValue(reader); 

							// Zpracování zprávy podle typu protokolu
							switch (Type)
							{
								case ProtocolType.CanFms:
								case ProtocolType.OneWireFms:
									ParseKeyValuePairs(Message);
									break;

								case ProtocolType.Unknown:
								case ProtocolType.Csv:
									// Pro ostatní typy protokolů pouze uložíme zprávu
									break;
							}
						}
					}
					else
					{
						Type = ProtocolType.Unknown;
						Message = string.Empty;
					}

					// Kontrola, zda jsme přečetli všechna data
					if (ms.Position < ms.Length)
					{
						Console.WriteLine($"Varování: Nepřečtená data v rFMS bloku: {ms.Length - ms.Position} bajtů.");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování rFMS bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Parsuje páry klíč:hodnota ze zprávy.
		/// </summary>
		/// <param name="message">Zpráva k parsování.</param>
		private void ParseKeyValuePairs(string message)
		{
			if (string.IsNullOrEmpty(message))
				return;

			// Rozdělení zprávy na jednotlivé páry klíč:hodnota
			var pairs = message.Split(',');
			foreach (string pair in pairs)
			{
				var trimmedPair = pair.Trim();
				var colonIndex = trimmedPair.IndexOf(':');

				if (colonIndex > 0)
				{
					var key = trimmedPair.Substring(0, colonIndex).Trim();
					var value = trimmedPair.Substring(colonIndex + 1).Trim();

					if (!string.IsNullOrEmpty(key))
					{
						KeyValuePairs[key] = value;
					}
				}
			}
		}

		/// <summary>
		/// Získá hodnotu pro zadaný klíč.
		/// </summary>
		/// <param name="key">Klíč k vyhledání.</param>
		/// <returns>Hodnota pro zadaný klíč nebo null, pokud klíč neexistuje nebo hodnota je neplatná ('*').</returns>
		public string? GetValue(string key)
		{
			if ((Type != ProtocolType.CanFms && Type != ProtocolType.OneWireFms) || !KeyValuePairs.ContainsKey(key))
				return null;

			var value = KeyValuePairs[key];
			return value == "*" ? null : value;
		}

		/// <summary>
		/// Získá hodnotu pro zadaný klíč jako double.
		/// </summary>
		/// <param name="key">Klíč k vyhledání.</param>
		/// <returns>Hodnota pro zadaný klíč jako double nebo null, pokud klíč neexistuje, hodnota je neplatná nebo není číslo.</returns>
		public double? GetDoubleValue(string key)
		{
			var value = GetValue(key);
			if (value == null)
				return null;

			if (double.TryParse(value, out double result))
				return result;

			return null;
		}

		/// <summary>
		/// Získá spotřebu paliva v litrech za hodinu.
		/// </summary>
		public double? FuelEconomyLitersPerHour => GetDoubleValue("FUEL_ECO_L_PER_H");

		/// <summary>
		/// Získá spotřebu paliva v kilometrech na litr.
		/// </summary>
		public double? FuelEconomyKmPerLiter => GetDoubleValue("FUEL_ECO_KM_PER_L");

		/// <summary>
		/// Získá hladinu paliva v procentech.
		/// </summary>
		public double? FuelLevel => GetDoubleValue("FUEL_LEV");

		/// <summary>
		/// Získá kumulovanou spotřebu paliva v litrech.
		/// </summary>
		public double? FuelConsumption => GetDoubleValue("FUEL_C");

		/// <summary>
		/// Získá indikátor hladiny paliva.
		/// </summary>
		public string? FuelLevelIndicator => GetValue("FMS_FUEL_LEVEL");

		/// <summary>
		/// Získá rychlost vozidla v km/h.
		/// </summary>
		public double? VehicleSpeed => GetDoubleValue("VEH_SPEED");

		/// <summary>
		/// Získá kumulovanou vzdálenost v km.
		/// </summary>
		public double? Distance => GetDoubleValue("DISTANCE");

		/// <summary>
		/// Vrací řetězcovou reprezentaci rFMS bloku.
		/// </summary>
		public override string ToString()
		{
			if (Type == ProtocolType.CanFms || Type == ProtocolType.OneWireFms)
			{
				return $"rFMS blok: Čas={TimestampDateTime}, Typ={Type}, Rychlost={VehicleSpeed} km/h, Palivo={FuelLevel}%, Spotřeba={FuelEconomyLitersPerHour} l/h";
			}
			else
			{
				return $"rFMS blok: Čas={TimestampDateTime}, Typ={Type}, Zpráva={Message}";
			}
		}
	}
}

