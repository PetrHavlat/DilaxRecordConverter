using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	/// <summary>
	/// Reprezentuje PISM blok, který obsahuje informace z informačního systému pro cestující.
	/// </summary>
	public class PismBlock : Dlx3Block
	{
		/// <summary>
		/// Typy protokolů pro PISM blok.
		/// </summary>
		public enum ProtocolType : byte
		{
			/// <summary>
			/// Neznámý protokol (není zpracováván DILAX Data Management Software).
			/// </summary>
			Unknown = 0,

			/// <summary>
			/// IBIS protokol.
			/// </summary>
			IBIS = 1,

			/// <summary>
			/// SAE J1587/J1708 protokol.
			/// </summary>
			J1587 = 2,

			/// <summary>
			/// SAE J1939 protokol.
			/// </summary>
			J1939 = 3,

			/// <summary>
			/// CSV seznam (specifický pro projekt).
			/// </summary>
			CSV = 4,

			/// <summary>
			/// Data o jízdě (klíč:hodnota).
			/// </summary>
			TripData = 5
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
		public string Message { get; private set; }

		/// <summary>
		/// Získá slovník klíč-hodnota pro typ protokolu 5 (TripData).
		/// </summary>
		public Dictionary<string, string> TripData { get; } = new Dictionary<string, string>();

		/// <summary>
		/// Získá datum a čas, kdy se informace stala platnou, jako DateTime.
		/// </summary>
		public DateTime TimestampDateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;



		/// <summary>
		/// Parsuje binární data PISM bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 5) // Minimálně potřebujeme 5 bajtů (4+1)
			{
				Console.WriteLine("Varování: PISM blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					Timestamp = Dlx3Pomocnik.CtiUIntBigEndian(reader);
					//if (Dlx3Pomocnik.CASY_PRIJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen příjezd");
					//if (Dlx3Pomocnik.CASY_ODJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen odjezd");

					Type = (ProtocolType)reader.ReadByte();

					// Načtení zprávy
					if (ms.Position < ms.Length)
					{
						Message = Dlx3Pomocnik.ReadNullTerminatedString(reader);

						// Zpracování zprávy podle typu protokolu
						switch (Type)
						{
							case ProtocolType.TripData:
								ParseTripData(Message);
								break;

							case ProtocolType.Unknown:
							case ProtocolType.IBIS:
							case ProtocolType.J1587:
							case ProtocolType.J1939:
							case ProtocolType.CSV:
								// Pro ostatní typy protokolů pouze uložíme zprávu
								break;
						}
					}

					// Kontrola, zda jsme přečetli všechna data
					if (ms.Position < ms.Length)
					{
						Console.WriteLine($"Varování: Nepřečtená data v PISM bloku: {ms.Length - ms.Position} bajtů.");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování PISM bloku: {ex.Message}");
			}
		}



		/// <summary>
		/// Parsuje data o jízdě (typ protokolu 5) ze zprávy.
		/// </summary>
		/// <param name="message">Zpráva k parsování.</param>
		private void ParseTripData(string message)
		{
			if (string.IsNullOrEmpty(message))
				return;

			// Rozdělení zprávy na jednotlivé páry klíč:hodnota
			string[] pairs = message.Split(',');
			foreach (string pair in pairs)
			{
				string trimmedPair = pair.Trim();
				int colonIndex = trimmedPair.IndexOf(':');

				if (colonIndex > 0)
				{
					string key = trimmedPair.Substring(0, colonIndex).Trim();
					string value = trimmedPair.Substring(colonIndex + 1).Trim();

					if (!string.IsNullOrEmpty(key))
					{
						TripData[key] = value;
					}
				}
			}
		}



		/// <summary>
		/// Získá hodnotu pro zadaný klíč z dat o jízdě.
		/// </summary>
		/// <param name="key">Klíč k vyhledání.</param>
		/// <returns>Hodnota pro zadaný klíč nebo null, pokud klíč neexistuje.</returns>
		public string GetTripDataValue(string key)
		{
			if (Type != ProtocolType.TripData || !TripData.ContainsKey(key))
				return null;

			return TripData[key];
		}



		/// <summary>
		/// Získá ID linky z dat o jízdě.
		/// </summary>
		public string Line => GetTripDataValue("line");

		/// <summary>
		/// Získá ID trasy z dat o jízdě.
		/// </summary>
		public string Route => GetTripDataValue("route");

		/// <summary>
		/// Získá ID jízdy z dat o jízdě.
		/// </summary>
		public string Trip => GetTripDataValue("trip");

		/// <summary>
		/// Získá název zastávky z dat o jízdě.
		/// </summary>
		public string Stop => GetTripDataValue("stop");

		/// <summary>
		/// Získá ID zastávky z dat o jízdě.
		/// </summary>
		public string StopId => GetTripDataValue("stopid");

		/// <summary>
		/// Získá název další zastávky z dat o jízdě.
		/// </summary>
		public string NextStop => GetTripDataValue("nextstop");

		/// <summary>
		/// Získá ID další zastávky z dat o jízdě.
		/// </summary>
		public string NextStopId => GetTripDataValue("nextstopid");

		/// <summary>
		/// Získá počet zbývajících zastávek do konce jízdy z dat o jízdě.
		/// </summary>
		public int? StopsLeft
		{
			get
			{
				string value = GetTripDataValue("stopsleft");
				if (int.TryParse(value, out int result))
					return result;
				return null;
			}
		}

		/// <summary>
		/// Získá cíl jízdy z dat o jízdě.
		/// </summary>
		public string Destination => GetTripDataValue("destination");

		/// <summary>
		/// Získá ID kurzu z dat o jízdě.
		/// </summary>
		public string Course => GetTripDataValue("course");

		/// <summary>
		/// Vrací řetězcovou reprezentaci PISM bloku.
		/// </summary>
		public override string ToString()
		{
			if (Type == ProtocolType.TripData)
			{
				return $"PISM blok: Čas={TimestampDateTime}, Typ={Type}, Linka={Line}, Trasa={Route}, Jízda={Trip}, Zastávka={Stop}, Další zastávka={NextStop}";
			}
			else
			{
				return $"PISM blok: Čas={TimestampDateTime}, Typ={Type}, Zpráva={Message}";
			}
		}
	}
}

