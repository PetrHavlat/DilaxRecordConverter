using DilaxRecordConverter.Core;
using DilaxRecordConverter.Core.Helpers;
using DLX3Converter.Pomocnici;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	/// <summary>
	/// Reprezentuje WAYP blok, který obsahuje informace o waypointech (bodech trasy) vozidla.
	/// </summary>
	public class WaypBlock : Dlx3Block
	{
		/// <summary>
		/// Typy waypointů.
		/// </summary>
		public enum WaypointType : byte
		{
			/// <summary>
			/// Vozidlo zastavilo na tomto waypointu.
			/// </summary>
			VehicleHalted = 1,

			/// <summary>
			/// Vozidlo projelo tímto waypointem.
			/// </summary>
			VehiclePassed = 2,

			/// <summary>
			/// Vozidlo zastavilo. Tento waypoint je první nebo poslední zastávka jízdy.
			/// </summary>
			FirstOrLastStop = 3
		}

		/// <summary>
		/// Získá časové razítko odjezdu z předchozího waypointu.
		/// </summary>
		public uint DepartureTimestamp { get; private set; }

		/// <summary>
		/// Získá časové razítko příjezdu na waypoint.
		/// </summary>
		public uint ArrivalTimestamp { get; private set; }

		/// <summary>
		/// Získá typ waypointu.
		/// </summary>
		public WaypointType Type { get; private set; }

		/// <summary>
		/// Získá zeměpisnou šířku v jednotkách 0.0001 minuty.
		/// Jižní polokoule je záporná. Rozsah je -90° (-54000000) až 90° (54000000).
		/// Hodnota 0 znamená, že poloha je neznámá.
		/// </summary>
		public int Latitude { get; private set; }

		/// <summary>
		/// Získá zeměpisnou délku v jednotkách 0.0001 minuty.
		/// Západní polokoule je záporná. Rozsah je -180° (-108000000) až 180° (108000000).
		/// Hodnota 0 znamená, že poloha je neznámá.
		/// </summary>
		public int Longitude { get; private set; }

		/// <summary>
		/// Získá počet satelitů (0 až 12) použitých pro určení polohy.
		/// </summary>
		public byte Satellites { get; private set; }

		/// <summary>
		/// Získá celkovou ujetou vzdálenost v metrech od předchozího waypointu.
		/// Záporná hodnota znamená, že vzdálenost nelze změřit.
		/// </summary>
		public short TravelledDistance { get; private set; }

		/// <summary>
		/// Získá aktuální rychlost v jednotkách 0.1 km/h.
		/// Záporná hodnota znamená, že rychlost nelze změřit.
		/// </summary>
		public short Speed { get; private set; }

		/// <summary>
		/// Získá identifikátor zastávky.
		/// Pokud je waypoint pravidelnou zastávkou, je to identifikátor zastávky.
		/// Pokud tato hodnota není k dispozici, může být prázdná nebo obsahovat "n.a.".
		/// </summary>
		public string StopIdentifier { get; private set; }

		/// <summary>
		/// Získá datum a čas odjezdu z předchozího waypointu jako DateTime.
		/// </summary>
		public DateTime DepartureDateTime => DateTimeOffset.FromUnixTimeSeconds(DepartureTimestamp).DateTime;

		/// <summary>
		/// Získá datum a čas příjezdu na waypoint jako DateTime.
		/// </summary>
		public DateTime ArrivalDateTime => DateTimeOffset.FromUnixTimeSeconds(ArrivalTimestamp).DateTime;

		/// <summary>
		/// Získá zeměpisnou šířku ve stupních.
		/// </summary>
		public decimal LatitudeDegrees => Latitude != 0 ? Latitude * 0.0001m / 60m : 0;

		/// <summary>
		/// Získá zeměpisnou délku ve stupních.
		/// </summary>
		public decimal LongitudeDegrees => Longitude != 0 ? Longitude * 0.0001m / 60m : 0;

		/// <summary>
		/// Získá aktuální rychlost v km/h.
		/// </summary>
		public decimal SpeedKmh => Speed >= 0 ? Speed * 0.1m : 0;

		/// <summary>
		/// Získá aktuální rychlost v m/s.
		/// </summary>
		public decimal SpeedMps => Speed >= 0 ? Speed * 0.1m / 3.6m : 0;

		// Pro zpětnou kompatibilitu
		public uint Timestamp => ArrivalTimestamp;
		public short Course { get; private set; }
		public decimal CourseDegrees => Course / 100m;



		/// <summary>
		/// Parsuje binární data WAYP bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		public override void ParseData(byte[] data)
		{
			if (data == null || data.Length < 20) // Minimálně potřebujeme 20 bajtů (4+4+1+4+4+1+2+2)
			{
				Console.WriteLine("Varování: WAYP blok je příliš krátký nebo null.");
				return;
			}

			try
			{
				using (var ms = new MemoryStream(data))
				using (var reader = new BinaryReader(ms))
				{
					// Kontrola, zda máme dostatek dat pro původní formát
					if (data.Length >= 16)
					{
						// Pokusíme se nejprve načíst data podle původní implementace
						uint timestamp = BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
						int latitude = BinaryHelper.ReadIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
						int longitude = BinaryHelper.ReadIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
						short speed = BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
						short course = BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);

						// Uložíme načtená data
						ArrivalTimestamp = timestamp;
						DepartureTimestamp = timestamp; // Nemáme k dispozici, použijeme stejnou hodnotu
						Type = WaypointType.VehiclePassed; // Nemáme k dispozici, použijeme výchozí hodnotu

						// Převod z 1e-6 stupňů na 0.0001 minuty
						Latitude = (int)(latitude / 1_000_000m * 60m * 10000m);
						Longitude = (int)(longitude / 1_000_000m * 60m * 10000m);

						// Převod z cm/s na 0.1 km/h
						Speed = (short)(speed / 100m * 3.6m * 10m);

						Satellites = 0; // Nemáme k dispozici
						TravelledDistance = 0; // Nemáme k dispozici
						StopIdentifier = string.Empty; // Nemáme k dispozici

						// Uložíme původní hodnoty pro zpětnou kompatibilitu
						Course = course;

						Console.WriteLine("Informace: Načtena původní verze WAYP bloku.");
						return;
					}

					// Načtení dat podle specifikace
					DepartureTimestamp	= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					ArrivalTimestamp	= BinaryHelper.ReadUIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					Type = (WaypointType)BinaryHelper.ReadByteValue(reader);
					Latitude			= BinaryHelper.ReadIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					Longitude			= BinaryHelper.ReadIntValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					Satellites			= BinaryHelper.ReadByteValue(reader);
					TravelledDistance	= BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);
					Speed				= BinaryHelper.ReadShortValue(reader, DefaultValues.IS_IN_BIG_ENDIAN);

					// Načtení identifikátoru zastávky
					if (ms.Position < ms.Length)
					{
						StopIdentifier = BinaryHelper.ReadStringValue(reader);
					}
					else
					{
						StopIdentifier = string.Empty;
					}

					// Kontrola, zda jsme přečetli všechna data
					if (ms.Position < ms.Length)
					{
						Console.WriteLine($"Varování: Nepřečtená data v WAYP bloku: {ms.Length - ms.Position} bajtů.");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při parsování WAYP bloku: {ex.Message}");
			}
		}



		/// <summary>
		/// Vrací řetězcovou reprezentaci WAYP bloku.
		/// </summary>
		public override string ToString()
		{
			return $"WAYP blok: Příjezd={ArrivalDateTime}, Typ={Type}, " +
				   $"Poloha=[{LatitudeDegrees}, {LongitudeDegrees}], " +
				   $"Rychlost={SpeedKmh} km/h, Zastávka={StopIdentifier}";
		}
	}
}

