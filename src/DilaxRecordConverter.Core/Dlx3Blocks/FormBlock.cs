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
    /// Reprezentuje FORM blok, který popisuje formaci vlaku.
    /// </summary>
    public class FormBlock : Dlx3Block
	{
		/// <summary>
		/// Získá časové razítko, kdy byl tento blok zapsán.
		/// </summary>
		public uint Timestamp { get; private set; }

		/// <summary>
		/// Získá seznam vozů ve vlaku.
		/// </summary>
		public List<TrainCar> Cars { get; } = new List<TrainCar>();

		/// <summary>
		/// Získá datum a čas vytvoření bloku jako DateTime.
		/// </summary>
		public DateTime TimestampDateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;

		/// <summary>
		/// Parsuje binární data FORM bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 4) // Minimálně potřebujeme 4 bajty pro časové razítko
			{
				Console.WriteLine("Varování: FORM blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Načtení časového razítka
					Timestamp = Dlx3Pomocnik.CtiUIntBigEndian(reader);
					//if (Dlx3Pomocnik.CASY_PRIJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen příjezd");
					//if (Dlx3Pomocnik.CASY_ODJEZDU.Contains(DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime))
					//	Console.WriteLine($"{GetType().Name} - nalezen odjezd");

					// Načítání sekvence vozů
					while (ms.Position < ms.Length)
					{
						var car = new TrainCar
						{
							VehicleId = Dlx3Pomocnik.ReadNullTerminatedString(reader),
							VehicleType = Dlx3Pomocnik.ReadNullTerminatedString(reader),
							Operator = Dlx3Pomocnik.ReadNullTerminatedString(reader)
						};

						Cars.Add(car);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování FORM bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci FORM bloku.
		/// </summary>
		public override string ToString()
		{
			return $"FORM blok: Čas={TimestampDateTime}, Počet vozů={Cars.Count}";
		}
	}
}


