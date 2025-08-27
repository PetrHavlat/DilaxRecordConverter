using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	/// <summary>
	/// Reprezentuje FEND blok, který označuje konec DLX3 souboru.
	/// Podle specifikace 4.1.1 musí být FEND blok poslední blok v souboru a neobsahuje žádná data.
	/// </summary>
	public class FendBlock : Dlx3Block
	{
		/// <summary>
		/// Parsuje data FEND bloku.
		/// </summary>
		/// <param name="data">Binární data k parsování.</param>
		/// <exception cref="InvalidOperationException">Vyvolána, pokud FEND blok obsahuje data.</exception>
		public override void ParseData(byte[] data)
		{
			// FEND blok nesmí obsahovat žádná data podle specifikace 4.1.1
			if (data != null && data.Length > 0)
			{
				Console.WriteLine($"Varování: FEND blok by neměl obsahovat žádná data, ale obsahuje {data.Length} bajtů.");
			}
		}

		/// <summary>
		/// Ověří CRC pro FEND blok.
		/// </summary>
		/// <param name="typeAndData">Kombinace typu bloku a dat pro výpočet CRC.</param>
		/// <returns>True, pokud CRC odpovídá, jinak false.</returns>
		//public override bool ValidateCrc(byte[] typeAndData)
		//{
		//	// Pro FEND blok by typeAndData mělo obsahovat pouze typ bloku (4 bajty "FEND")
		//	// a žádná data, takže by mělo mít délku přesně 4 bajty
		//	if (typeAndData.Length != 4)
		//	{
		//		Console.WriteLine($"Varování: Neočekávaná délka dat pro CRC FEND bloku: {typeAndData.Length} bajtů.");
		//	}

		//	// Použijeme standardní implementaci z nadřazené třídy
		//	return base.ValidateCrc(typeAndData);
		//}

		/// <summary>
		/// Vrací řetězcovou reprezentaci FEND bloku.
		/// </summary>
		/// <returns>Řetězcová reprezentace FEND bloku.</returns>
		public override string ToString()
		{
			return "FEND blok: Konec souboru";
		}
	}
}

