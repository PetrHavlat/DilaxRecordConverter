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
	/// Reprezentuje FSTP blok, který obsahuje mezivýsledky počítání nastupujících a vystupujících cestujících.
	/// </summary>
	public class FstpBlock : Dlx3Block
	{
		/// <summary>
		/// Získá časové razítko, kdy byl tento blok zapsán.
		/// </summary>
		public uint Timestamp { get; private set; }

		/// <summary>
		/// Získá seznam informací o dveřích.
		/// </summary>
		public List<DoorData> DoorCounts { get; } = new List<DoorData>();

		/// <summary>
		/// Získá datum a čas vytvoření bloku jako DateTime.
		/// </summary>
		public DateTime TimestampDateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;



		/// <summary>
		/// Parsuje binární data FSTP bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 4) // Minimálně potřebujeme 4 bajty pro časové razítko
			{
				Console.WriteLine("Varování: FSTP blok je příliš krátký nebo null.");
				return;
			}

			// Kontrola délky bloku
			if ((data.Length - 4) % 11 != 0)
			{
				Console.WriteLine($"Varování: Neplatná délka FSTP bloku: {data.Length} bajtů. Délka mínus 4 by měla být dělitelná 9.");
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Načtení časového razítka
					Timestamp = BinaryHelper.ReadUIntValue(reader);
					
					// Načítání informací o dveřích
					while (ms.Position + 11 <= ms.Length) // Potřebujeme 11 bajtů pro každé dveře (4+1+2+2+2)
					{
						var doorCount = new DoorData
						{
							DeviceId = BinaryHelper.ReadUIntValue(reader),
							Instance = BinaryHelper.ReadByteValue(reader),
							BoardingPassengers = BinaryHelper.ReadShortValue(reader),
							AlightingPassengers = BinaryHelper.ReadShortValue(reader),
							UncertainPassengers	= BinaryHelper.ReadShortValue(reader)
						};

						DoorCounts.Add(doorCount);
					}

					// Kontrola, zda jsme přečetli všechna data
					if (ms.Position < ms.Length)
					{
						Console.WriteLine($"Varování: Nepřečtená data v FSTP bloku: {ms.Length - ms.Position} bajtů.");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování FSTP bloku: {ex.Message}");
			}

			
		}



		/// <summary>
		/// Vrací řetězcovou reprezentaci FSTP bloku.
		/// </summary>
		public override string ToString()
		{
			return $"FSTP blok: Čas={TimestampDateTime}, Počet dveří={DoorCounts.Count}";
		}
	}
}

