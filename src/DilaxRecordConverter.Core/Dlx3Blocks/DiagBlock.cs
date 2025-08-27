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
	/// Reprezentuje DIAG blok, který obsahuje diagnostické zprávy.
	/// </summary>
	public partial class DiagBlock : Dlx3Block
	{

		/// <summary>
		/// Získá seznam diagnostických zpráv v tomto bloku.
		/// </summary>
		public List<DiagnosticMessage> DiagnosticMessages { get; } = new List<DiagnosticMessage>();

		// Pro zpětnou kompatibilitu
		public uint Timestamp => DiagnosticMessages.Count > 0 ? DiagnosticMessages[0].Timestamp : 0;
		public uint DeviceId => DiagnosticMessages.Count > 0 && DiagnosticMessages[0].DeviceId.HasValue ? DiagnosticMessages[0].DeviceId.Value : 0;
		public ushort DiagId => DiagnosticMessages.Count > 0 ? (ushort)((DiagnosticMessages[0].ModuleId << 8) | DiagnosticMessages[0].SubModuleId) : (ushort)0;
		public byte[] DiagValue { get; private set; } = new byte[0];

		/// <summary>
		/// Parsuje binární data DIAG bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 4)
			{
				Console.WriteLine("Varování: DIAG blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Uložíme původní data pro zpětnou kompatibilitu
					DiagValue = data;

					// Parsujeme diagnostické zprávy, dokud máme dostatek dat
					while (ms.Position + 8 <= ms.Length) // Minimálně potřebujeme 8 bajtů (4+1+1+1+1)
					{
						var message = new DiagnosticMessage
						{
							Timestamp = BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							ModuleId = BinaryHelper.ReadByteValue(reader),
							SubModuleId = BinaryHelper.ReadByteValue(reader),
							MessageId = BinaryHelper.ReadByteValue(reader),
							Category = BinaryHelper.ReadByteValue(reader)
						};
						
						// Čtení zprávy, pokud máme dostatek dat
						if (ms.Position < ms.Length)
						{
							message.Message = BinaryHelper.ReadStringValue(reader);

							// Parsování informací o zařízení pro Module ID 20
							if (message.HasDeviceInfo)
							{
								message.ParseDeviceInfo();
							}
						}

						DiagnosticMessages.Add(message);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování DIAG bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci DIAG bloku.
		/// </summary>
		public override string ToString()
		{
			if (DiagnosticMessages.Count == 0)
				return "DIAG blok: Žádné diagnostické zprávy";

			return $"DIAG blok: {DiagnosticMessages.Count} diagnostických zpráv";
		}
	}
}

