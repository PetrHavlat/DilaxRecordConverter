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
	public partial class CdatBlock : Dlx3Block
	{
		public uint Timestamp { get; private set; }
		public ushort PassengerExchangeTime { get; private set; }

		// Kolekce dat pro jednotlivé dveře
		public List<DoorData> Doors { get; } = new List<DoorData>();

		public override void ParseData(byte[] data)
		{
			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Kontrola minimální délky dat (timestamp + exchange time)
					if (ms.Length < 6)
					{
						Console.WriteLine("Varování: CDAT blok je příliš krátký.");
						return;
					}

					// Čtení základních údajů
					Timestamp = BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					
					PassengerExchangeTime = BinaryHelper.ReadUShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);

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
							DeviceId = BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							Instance = BinaryHelper.ReadByteValue(reader),
							BoardingPassengers = BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							AlightingPassengers = BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							UncertainPassengers = BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN)
						};

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

