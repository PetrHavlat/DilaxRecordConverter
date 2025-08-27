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
	/// Reprezentuje rPET blok, který obsahuje časová razítka pohybu cestujících a otevírání/zavírání dveří.
	/// </summary>
	public class rPetBlock : Dlx3Block
	{
		/// <summary>
		/// Reprezentuje informace o jedněch dveřích v rPET bloku.
		/// </summary>
		public class DoorExchangeTime
		{
			/// <summary>
			/// Získá nebo nastaví ID zařízení.
			/// </summary>
			public uint DeviceId { get; set; }

			/// <summary>
			/// Získá nebo nastaví instanci dveří.
			/// </summary>
			public byte Instance { get; set; }

			/// <summary>
			/// Získá nebo nastaví časové razítko prvního pohybu cestujícího u dveří.
			/// </summary>
			public uint FirstPassengerMovement { get; set; }

			/// <summary>
			/// Získá nebo nastaví časové razítko posledního pohybu cestujícího u dveří.
			/// </summary>
			public uint LastPassengerMovement { get; set; }

			/// <summary>
			/// Získá nebo nastaví časové razítko prvního otevření dveří.
			/// </summary>
			public uint FirstOpening { get; set; }

			/// <summary>
			/// Získá nebo nastaví časové razítko posledního zavření dveří.
			/// </summary>
			public uint LastClosing { get; set; }

			/// <summary>
			/// Získá datum a čas prvního pohybu cestujícího u dveří jako DateTime.
			/// </summary>
			public DateTime? FirstPassengerMovementDateTime =>
				FirstPassengerMovement > 0 ? DateTimeOffset.FromUnixTimeSeconds(FirstPassengerMovement).DateTime : (DateTime?)null;

			/// <summary>
			/// Získá datum a čas posledního pohybu cestujícího u dveří jako DateTime.
			/// </summary>
			public DateTime? LastPassengerMovementDateTime =>
				LastPassengerMovement > 0 ? DateTimeOffset.FromUnixTimeSeconds(LastPassengerMovement).DateTime : (DateTime?)null;

			/// <summary>
			/// Získá datum a čas prvního otevření dveří jako DateTime.
			/// </summary>
			public DateTime? FirstOpeningDateTime =>
				FirstOpening > 0 ? DateTimeOffset.FromUnixTimeSeconds(FirstOpening).DateTime : (DateTime?)null;

			/// <summary>
			/// Získá datum a čas posledního zavření dveří jako DateTime.
			/// </summary>
			public DateTime? LastClosingDateTime =>
				LastClosing > 0 ? DateTimeOffset.FromUnixTimeSeconds(LastClosing).DateTime : (DateTime?)null;

			/// <summary>
			/// Získá dobu výměny cestujících (od prvního pohybu do posledního pohybu).
			/// </summary>
			public TimeSpan? PassengerExchangeTime =>
				(FirstPassengerMovement > 0 && LastPassengerMovement > 0) ?
					TimeSpan.FromSeconds(LastPassengerMovement - FirstPassengerMovement) : (TimeSpan?)null;

			/// <summary>
			/// Získá dobu otevření dveří (od prvního otevření do posledního zavření).
			/// </summary>
			public TimeSpan? DoorOpenTime =>
				(FirstOpening > 0 && LastClosing > 0) ?
					TimeSpan.FromSeconds(LastClosing - FirstOpening) : (TimeSpan?)null;

			/// <summary>
			/// Vrací řetězcovou reprezentaci informací o dveřích.
			/// </summary>
			public override string ToString()
			{
				return $"Dveře: ID={DeviceId}, Instance={Instance}, " +
					   $"První pohyb={FirstPassengerMovementDateTime}, Poslední pohyb={LastPassengerMovementDateTime}, " +
					   $"První otevření={FirstOpeningDateTime}, Poslední zavření={LastClosingDateTime}";
			}
		}

		/// <summary>
		/// Získá časové razítko, kdy byl tento blok zapsán.
		/// </summary>
		public uint Timestamp { get; private set; }

		/// <summary>
		/// Získá seznam informací o dveřích.
		/// </summary>
		public List<DoorExchangeTime> DoorExchangeTimes { get; } = new List<DoorExchangeTime>();

		/// <summary>
		/// Získá datum a čas vytvoření bloku jako DateTime.
		/// </summary>
		public DateTime TimestampDateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;

		// Pro zpětnou kompatibilitu
		public byte DoorId => DoorExchangeTimes.Count > 0 ? DoorExchangeTimes[0].Instance : (byte)0;
		public ushort BoardingTime => DoorExchangeTimes.Count > 0 ? (ushort)DoorExchangeTimes[0].FirstPassengerMovement : (ushort)0;
		public ushort AlightingTime => DoorExchangeTimes.Count > 0 ? (ushort)DoorExchangeTimes[0].LastPassengerMovement : (ushort)0;

		/// <summary>
		/// Parsuje binární data rPET bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 4) // Minimálně potřebujeme 4 bajty pro časové razítko
			{
				Console.WriteLine("Varování: rPET blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Načtení časového razítka
					Timestamp = BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN); 
					
					// Načítání informací o dveřích
					while (ms.Position + 17 <= ms.Length) // Potřebujeme 17 bajtů pro každé dveře (4+1+4+4+4)
					{
						var doorExchangeTime = new DoorExchangeTime
						{
							DeviceId				= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							Instance				= BinaryHelper.ReadByteValue(reader),
							FirstPassengerMovement	= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							LastPassengerMovement	= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							FirstOpening			= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN),
							LastClosing				= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN)
						};
						
						DoorExchangeTimes.Add(doorExchangeTime);
					}

					// Kontrola, zda jsme přečetli všechna data
					if (ms.Position < ms.Length)
					{
						Console.WriteLine($"Varování: Nepřečtená data v rPET bloku: {ms.Length - ms.Position} bajtů.");

						// Pokud jsou k dispozici další data, může jít o starší verzi bloku
						// Pokusíme se načíst data podle původní implementace
						if (ms.Position == 4 && ms.Length >= 9)
						{
							ms.Position = 4; // Vrátíme se na pozici po časovém razítku

							byte doorId				= BinaryHelper.ReadByteValue(reader);
							ushort boardingTime		= BinaryHelper.ReadUShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
							ushort alightingTime	= BinaryHelper.ReadUShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);

							Console.WriteLine($"Informace: Načtena starší verze rPET bloku: DoorId={doorId}, BoardingTime={boardingTime}, AlightingTime={alightingTime}");

							// Vytvoříme záznam o dveřích z načtených dat
							var doorExchangeTime = new DoorExchangeTime
							{
								DeviceId				= 0, // Neznáme Device ID
								Instance				= doorId,
								FirstPassengerMovement	= boardingTime,
								LastPassengerMovement	= alightingTime,
								FirstOpening			= 0, // Neznáme časy otevření/zavření
								LastClosing				= 0
							};

							DoorExchangeTimes.Add(doorExchangeTime);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování rPET bloku: {ex.Message}");
			}
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci rPET bloku.
		/// </summary>
		public override string ToString()
		{
			return $"rPET blok: Čas={TimestampDateTime}, Počet dveří={DoorExchangeTimes.Count}";
		}
	}
}

