using DilaxRecordConverter.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	public partial class CdatBlock : Dlx3Block
	{
		public uint Timestamp { get; private set; }
		public ushort PassengerExchangeTime { get; private set; }

		// Kolekce dat pro jednotlivé dveře
		public List<DoorData> Doors { get; } = new List<DoorData>();


		//private Dictionary<string, short> _counts = new Dictionary<string, short>();

		//// Změna: Vrací kopii slovníku, aby se předešlo modifikaci
		//public Dictionary<string, short> Counts => new Dictionary<string, short>(_counts);


		// Přidání metody pro získání počtu pro konkrétní dveře
		//public short GetCount(string doorName)
		//{
		//	return _counts.TryGetValue(doorName, out short count) ? count : (short)0;
		//}

		public override void ParseData(byte[] data)
		{
			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					//_counts.Clear(); // Vyčistíme slovník před novým parsováním

					// Kontrola minimální délky dat (timestamp + exchange time)
					if (ms.Length < 6)
					{
						Console.WriteLine("Varování: CDAT blok je příliš krátký.");
						return;
					}

					// Čtení základních údajů
					Timestamp = BinaryHelper.ReadUIntBigEndianValue(reader);
					//if (Dlx3Pomocnik.CASY_PRIJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen příjezd");
					//if (Dlx3Pomocnik.CASY_ODJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen odjezd");

					PassengerExchangeTime = BinaryHelper.ReadUShortBigEndianValue(reader);

					// Validace délky bloku
					int remainingBytes = (int)(ms.Length - ms.Position);
					if (remainingBytes % 11 != 0) // 4+1+2+2+2 = 11 bajtů na jedny dveře
					{
						Console.WriteLine($"Varování: Neplatná délka CDAT bloku. Zbývající bajty: {remainingBytes}");
					}

					// Čtení dat o dveřích
					while (ms.Position + 11 <= ms.Length) // Potřebujeme alespoň 11 bajtů pro kompletní záznam dveří
					{
						var doorData = new DoorData
						{
							DeviceId = Dlx3Pomocnik.CtiUIntBigEndian(reader),
							Instance = reader.ReadByte(),
							BoardingPassengers = (short)Dlx3Pomocnik.CtiUShortBigEndian(reader),
							AlightingPassengers = (short)Dlx3Pomocnik.CtiUShortBigEndian(reader),
							UncertainPassengers = (short)Dlx3Pomocnik.CtiUShortBigEndian(reader)
						};

						//// --- změnit
						//if (_counts.ContainsKey(doorData.))
						//{
						//	_counts[doorData.Instance] += doorData.BoardingPassengers;
						//}
						//else
						//{
						//	_counts[doorData.Instance] = doorData.BoardingPassengers;
						//}

						Doors.Add(doorData);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování CDAT bloku: {ex.Message}");
			}
		}
	}
}

