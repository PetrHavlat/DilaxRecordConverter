using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DilaxRecordConverter.Core;
using DilaxRecordConverter.Core.Helpers;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
    /// <summary>
    /// Reprezentuje EVNT blok, který registruje uživatelsky definované události.
    /// </summary>
    public class EvntBlock : Dlx3Block
	{
		/// <summary>
		/// Získá časové razítko, kdy událost nastala.
		/// </summary>
		public uint Timestamp { get; private set; }

		/// <summary>
		/// Získá typ události.
		/// </summary>
		public byte EventType { get; private set; }

		/// <summary>
		/// Získá dodatečná data události.
		/// </summary>
		public byte[]? EventData { get; private set; }

		/// <summary>
		/// Získá popis typu události.
		/// </summary>
		public string EventDescription => EventTypes.GetEventDescription(EventType);

		/// <summary>
		/// Získá, zda událost označuje dosažení zastávky.
		/// </summary>
		public bool IsStopReached => EventType == EventTypes.ScheduledStopReached;

		/// <summary>
		/// Získá, zda událost označuje opuštění zastávky.
		/// </summary>
		public bool IsStopLeft => EventType == EventTypes.ScheduledStopLeft;



		/// <summary>
		/// Parsuje binární data EVNT bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 4)
			{
				Console.WriteLine("Varování: EVNT blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Čtení časového razítka
					Timestamp = BinaryHelper.ReadUIntValue(reader);
					
					// Kontrola, zda máme dostatek dat pro typ události (1 bajt)
					if (ms.Position < ms.Length)
					{
						EventType = reader.ReadByte();  // Čtení jednoho bajtu

						// Čtení zbývajících dat
						int eventDataLen = (int)(ms.Length - ms.Position);
						if (eventDataLen > 0)
							EventData = reader.ReadBytes(eventDataLen);
						else
							EventData = new byte[0];
					}
					else
					{
						EventType = 0;
						EventData = new byte[0];
						Console.WriteLine("Varování: EVNT blok neobsahuje typ události.");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování EVNT bloku: {ex.Message}");
				// Nastavení výchozích hodnot v případě chyby
				Timestamp = 0;
				EventType = 0;
				EventData = new byte[0];
			}

			
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci události.
		/// </summary>
		public override string ToString()
		{
			return $"[{DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime}] Událost: {EventDescription} (ID: {EventType})";
		}
	}
}

