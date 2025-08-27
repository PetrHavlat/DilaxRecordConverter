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
	/// Reprezentuje FHDR blok, který je prvním blokem DLX3 souboru a následuje ihned po signatuře souboru.
	/// </summary>
	public class FhdrBlock : Dlx3Block
	{
		/// <summary>
		/// Očekávaná hodnota revize formátu souboru ('D' = 68).
		/// </summary>
		public const byte ExpectedFileRevision = 68; // 'D'

		/// <summary>
		/// Očekávaná hodnota geodetického systému (1 = WGS84).
		/// </summary>
		public const byte ExpectedGeodeticSystem = 1; // WGS84

		/// <summary>
		/// Získá revizi formátu souboru. Měla by být 'D' (decimal 68).
		/// </summary>
		public byte FileRevision { get; private set; }

		/// <summary>
		/// Získá časové razítko vytvoření souboru.
		/// </summary>
		public uint CreationTime { get; private set; }

		/// <summary>
		/// Získá časové razítko předchozího souboru.
		/// </summary>
		public uint PreviousFileTime { get; private set; }

		/// <summary>
		/// Získá číslo geodetického systému. Mělo by být 1 (WGS84).
		/// </summary>
		public byte GeodeticSystem { get; private set; }

		/// <summary>
		/// Získá informace o časovém pásmu.
		/// </summary>
		public string TimeZone { get; private set; }

		/// <summary>
		/// Získá typ modelu zařízení.
		/// </summary>
		public string DeviceModel { get; private set; }

		/// <summary>
		/// Získá sériové číslo zařízení.
		/// </summary>
		public string DeviceSerial { get; private set; }

		/// <summary>
		/// Získá název operátora.
		/// </summary>
		public string Operator { get; private set; }

		/// <summary>
		/// Získá ID vozidla.
		/// </summary>
		public string VehicleId { get; private set; }

		/// <summary>
		/// Získá, zda revize formátu souboru odpovídá očekávané hodnotě.
		/// </summary>
		public bool IsValidFileRevision => FileRevision == ExpectedFileRevision;

		/// <summary>
		/// Získá, zda geodetický systém odpovídá očekávané hodnotě.
		/// </summary>
		public bool IsValidGeodeticSystem => GeodeticSystem == ExpectedGeodeticSystem;

		/// <summary>
		/// Získá datum a čas vytvoření souboru jako DateTime.
		/// </summary>
		public DateTime CreationDateTime => DateTimeOffset.FromUnixTimeSeconds(CreationTime).DateTime;

		/// <summary>
		/// Získá datum a čas předchozího souboru jako DateTime.
		/// </summary>
		public DateTime PreviousFileDateTime => DateTimeOffset.FromUnixTimeSeconds(PreviousFileTime).DateTime;

		/// <summary>
		/// Získá, zda existuje předchozí soubor.
		/// </summary>
		public bool HasPreviousFile => PreviousFileTime != 0;

		/// <summary>
		/// Parsuje binární data FHDR bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 10) // Minimálně potřebujeme 10 bajtů (1+4+4+1)
			{
				Console.WriteLine("Varování: FHDR blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					FileRevision		= reader.ReadByte();
					if (FileRevision != ExpectedFileRevision)
					{
						Console.WriteLine($"Varování: Neočekávaná revize formátu souboru: {FileRevision}, očekáváno: {ExpectedFileRevision}");
					}
					CreationTime		= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					PreviousFileTime	= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					GeodeticSystem		= BinaryHelper.ReadByteValue(reader);
					if (GeodeticSystem != ExpectedGeodeticSystem)
					{
						Console.WriteLine($"Varování: Neočekávaný geodetický systém: {GeodeticSystem}, očekáváno: {ExpectedGeodeticSystem}");
					}
					TimeZone			= BinaryHelper.ReadStringValue(reader);
					DeviceModel			= BinaryHelper.ReadStringValue(reader);
					DeviceSerial		= BinaryHelper.ReadStringValue(reader);
					Operator			= BinaryHelper.ReadStringValue(reader);
					VehicleId			= BinaryHelper.ReadStringValue(reader);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování FHDR bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci FHDR bloku.
		/// </summary>
		public override string ToString()
		{
			return $"FHDR blok: Revize={FileRevision}, Vytvořeno={CreationDateTime}, Předchozí={PreviousFileDateTime}, Zařízení={DeviceModel}, Sériové číslo={DeviceSerial}, Operátor={Operator}, ID vozidla={VehicleId}";
		}
	}
}


