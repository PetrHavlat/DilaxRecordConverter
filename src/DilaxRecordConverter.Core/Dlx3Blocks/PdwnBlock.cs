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
	/// Reprezentuje PDWN blok, který zaznamenává události vypnutí a zapnutí napájení.
	/// Poznámka: PDWN blok je zastaralý. Nové aplikace by měly místo toho zapisovat dva EVNT bloky s událostmi 8 a 9.
	/// </summary>
	[Obsolete("PDWN blok je zastaralý. Nové aplikace by měly místo toho zapisovat dva EVNT bloky s událostmi 8 a 9.")]
	public class PdwnBlock : Dlx3Block
	{
		/// <summary>
		/// Získá časové razítko vypnutí napájení.
		/// </summary>
		public uint PowerOffTimestamp { get; private set; }

		/// <summary>
		/// Získá časové razítko zapnutí napájení.
		/// </summary>
		public uint PowerOnTimestamp { get; private set; }

		/// <summary>
		/// Získá datum a čas vypnutí napájení jako DateTime.
		/// </summary>
		public DateTime PowerOffDateTime => DateTimeOffset.FromUnixTimeSeconds(PowerOffTimestamp).DateTime;

		/// <summary>
		/// Získá datum a čas zapnutí napájení jako DateTime.
		/// </summary>
		public DateTime PowerOnDateTime => DateTimeOffset.FromUnixTimeSeconds(PowerOnTimestamp).DateTime;

		/// <summary>
		/// Získá dobu trvání výpadku napájení.
		/// </summary>
		public TimeSpan PowerDownDuration => PowerOnDateTime - PowerOffDateTime;

		/// <summary>
		/// Parsuje binární data PDWN bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 8) // Potřebujeme 8 bajtů (4+4)
			{
				Console.WriteLine("Varování: PDWN blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					PowerOffTimestamp = BinaryHelper.ReadUIntValue(reader);
					PowerOnTimestamp = BinaryHelper.ReadUIntValue(reader);
					
					// Kontrola, zda jsme přečetli všechna data
					if (ms.Position < ms.Length)
					{
						Console.WriteLine($"Varování: Nepřečtená data v PDWN bloku: {ms.Length - ms.Position} bajtů.");

						// Pokud jsou k dispozici další data, může jít o rozšířenou verzi bloku
						// Například PowerDownReason z původní implementace
						if (ms.Position < ms.Length)
						{
							var powerDownReason = reader.ReadByte();
							Console.WriteLine($"Informace: Nalezen PowerDownReason: {powerDownReason}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování PDWN bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci PDWN bloku.
		/// </summary>
		public override string ToString()
		{
			return $"PDWN blok: Vypnutí={PowerOffDateTime}, Zapnutí={PowerOnDateTime}, Trvání={PowerDownDuration}";
		}
	}
}

