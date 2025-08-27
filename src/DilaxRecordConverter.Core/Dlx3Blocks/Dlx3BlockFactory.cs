using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	public static class Dlx3BlockFactory
	{
		public static Dictionary<string, Dlx3Block> RegistrovaneBloky { get; private set; } = new Dictionary<string, Dlx3Block>()
		{
			{ "FHDR", new FhdrBlock() },
			{ "FEND", new FendBlock() },
			{ "CDAT", new CdatBlock() },
			{ "CONF", new ConfBlock() },
			{ "DIAG", new DiagBlock() },
			{ "FSTP", new FstpBlock() },
			{ "FORM", new FormBlock() },
			{ "EVNT", new EvntBlock() },
			{ "PDWN", new PdwnBlock() },
			{ "PISM", new PismBlock() },
			{ "rFMS", new rFmsBlock() },
			{ "rPET", new rPetBlock() },
			{ "WAYP", new WaypBlock() }
		};
		private static Dlx3Block VytvorBlok(string blockType)
		{
			if (String.IsNullOrEmpty(blockType))
				throw new ArgumentNullException($"Parametr {nameof(blockType)} nesmí mít hodnotu null nebo prazdný řetězec");

			if (!RegistrovaneBloky.ContainsKey(blockType))
				throw new ArgumentException($"Pro hodnotu {blockType} není definován žádný blok");
			
			return RegistrovaneBloky[blockType];
		}

		public static Dlx3Block VytvorBlok(string blockType, ushort length, byte[] data, ushort crc)
		{
			Dlx3Block blok = VytvorBlok(blockType);

			blok.BlockType	= blockType;
			blok.Length		= length;
			blok.Crc		= crc;
			blok.Data		= data;
			blok.ParseData(data);

			return blok;
		}

		public static void PridejBlok(string nazev, Dlx3Block dlx3Blok)
		{
			RegistrovaneBloky.Add(nazev, dlx3Blok);
		}

		public static void OdeberBlok(string nazev)
		{
			RegistrovaneBloky.Remove(nazev);
		}
	}
}
