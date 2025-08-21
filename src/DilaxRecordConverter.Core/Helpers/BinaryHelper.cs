using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DilaxRecordConverter.Core.Helpers
{
//| Datový typ(C#) | CTS typ                | Velikost (v bajtech) | Rozsah hodnot                                         | Popis                                 |
//|-----------------|-----------------------|----------------------|------------------------------------------------------|--------------------------------------|
//| `bool`          | System.Boolean        | 1                    | `true` nebo `false`                                  | Logická hodnota                       |
//| `byte`          | System.Byte           | 1                    | 0 až 255                                             | Cele číslo bez znaménka              |
//| `sbyte`         | System.SByte          | 1                    | -128 až 127                                          | Cele číslo se znaménkem              |
//| `char`          | System.Char           | 2                    | Unicode znak (0 až 65,535)                           | Jeden Unicode znak                   |
//| `short`         | System.Int16          | 2                    | -32,768 až 32,767                                    | Krátké celé číslo se znaménkem      |
//| `ushort`        | System.UInt16         | 2                    | 0 až 65,535                                          | Krátké celé číslo bez znaménka      |
//| `int`           | System.Int32          | 4                    | -2,147,483,648 až 2,147,483,647                      | Standardní celé číslo se znaménkem  |
//| `uint`          | System.UInt32         | 4                    | 0 až 4,294,967,295                                   | Standardní celé číslo bez znaménka  |
//| `long`          | System.Int64          | 8                    | -9,223,372,036,854,775,808 až 9,223,372,036,854,775,807 | Dlouhé celé číslo se znaménkem      |
//| `ulong`         | System.UInt64         | 8                    | 0 až 18,446,744,073,709,551,615                      | Dlouhé celé číslo bez znaménka      |
//| `float`         | System.Single         | 4                    | +-1.5 × 10⁻⁴⁵ až +-3.4 × 10³⁸ (přibližně)           | Jednoduchá přesnost, číslo s plovoucí desetinnou čárkou |
//| `double`        | System.Double         | 8                    | +-5.0 × 10⁻³²⁴ až +-1.7 × 10³⁰⁸ (přibližně)          | Dvojitá přesnost, číslo s plovoucí desetinnou čárkou |
//| `decimal`       | System.Decimal        | 16                   | ±1.0 × 10⁻²⁸ až ±7.9 × 10²⁸                          | Vysoká přesnost, vhodné pro finanční výpočty |
//| `string`        | System.String         | proměnná             | Řetězec znaků                                        | Sekvence Unicode znaků               |
//| `object`        | System.Object         | proměnná             | Libovolný datový typ                                 | Základní typ všech objektů v.NET   |

	public static class BinaryHelper
	{
		private static byte[] ReadBinaryData(BinaryReader reader, int sizeInBytes, string typeName, bool isInBigEndian = false)
		{
			try
			{
				var availableBytes = reader.BaseStream.Length - reader.BaseStream.Position;

				if (availableBytes < sizeInBytes)
				{
					throw new EndOfStreamException($"Nedostatek dat pro načtení {sizeInBytes} bajtu pro {typeName} hodnotu. Dostupné: {availableBytes} bajtů.");
				}

				var bytes = reader.ReadBytes(sizeInBytes);

				if (isInBigEndian) Array.Reverse(bytes);

				return bytes;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení: {ex.Message}");
				throw; // zachovává stack trace
			}
		}

		public static bool ReadBoolValue(BinaryReader reader) 
		{
			try
			{
				var availableBytes = reader.BaseStream.Length - reader.BaseStream.Position;

				if (availableBytes < sizeof(Boolean))
				{
					throw new EndOfStreamException($"Nedostatek dat pro načtení {sizeof(Boolean)} bajtu pro {nameof(Boolean)} hodnotu. Dostupné: {availableBytes} bajtů.");
				}

				return reader.ReadBoolean();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení: {ex.Message}");
				throw; // zachovává stack trace
			}
		}

		public static byte ReadByteValue(BinaryReader reader)
		{
			try
			{
				var availableBytes = reader.BaseStream.Length - reader.BaseStream.Position;

				if (availableBytes < sizeof(Byte))
				{
					throw new EndOfStreamException($"Nedostatek dat pro načtení {sizeof(Byte)} bajtu pro {nameof(Byte)} hodnotu. Dostupné: {availableBytes} bajtů.");
				}

				return reader.ReadByte();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení: {ex.Message}");
				throw; // zachovává stack trace
			}
		}
		
		public static sbyte ReadSByteValue(BinaryReader reader)
		{
			try
			{
				var availableBytes = reader.BaseStream.Length - reader.BaseStream.Position;

				if (availableBytes < sizeof(SByte))
				{
					throw new EndOfStreamException($"Nedostatek dat pro načtení {sizeof(SByte)} bajtu pro {nameof(SByte)} hodnotu. Dostupné: {availableBytes} bajtů.");
				}

				return reader.ReadSByte();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení: {ex.Message}");
				throw; // zachovává stack trace
			}
		}

		public static char ReadCharValue(BinaryReader reader)
		{
			try
			{
				var availableBytes = reader.BaseStream.Length - reader.BaseStream.Position;

				if (availableBytes < sizeof(Char))
				{
					throw new EndOfStreamException($"Nedostatek dat pro načtení {sizeof(Char)} bajtu pro {nameof(Char)} hodnotu. Dostupné: {availableBytes} bajtů.");
				}

				return reader.ReadChar();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení: {ex.Message}");
				throw; // zachovává stack trace
			}
		}

		public static short ReadShortValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(Int16), nameof(Int16), isInBigEndian);
			return BitConverter.ToInt16(bytes, 0);
		}

		public static ushort ReadUShortValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(UInt16), nameof(UInt16), isInBigEndian);
			return BitConverter.ToUInt16(bytes, 0);
		}

		public static int ReadIntValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(Int32), nameof(Int32), isInBigEndian);
			return BitConverter.ToInt32(bytes, 0);
		}

		public static uint ReadUIntValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(UInt32), nameof(UInt32), isInBigEndian);
			return BitConverter.ToUInt32(bytes, 0);
		}

		public static long ReadLongValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(Int64), nameof(Int64), isInBigEndian);
			return BitConverter.ToInt64(bytes, 0);
		}

		public static ulong ReadULongValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(UInt64), nameof(UInt64), isInBigEndian);
			return BitConverter.ToUInt64(bytes, 0);
		}

		public static float ReadFloatValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(float), nameof(Single), isInBigEndian);
			return BitConverter.ToSingle(bytes, 0);
		}

		public static double ReadDoubleValue(BinaryReader reader, bool isInBigEndian = false)
		{
			var bytes = ReadBinaryData(reader, sizeof(double), nameof(Double), isInBigEndian);
			return BitConverter.ToDouble(bytes, 0);
		}

		public static string ReadStringValue(BinaryReader reader)
		{
			// Budu pravděpodobně modifikovat

			try
			{
				List<byte> bytes = new List<byte>();
				byte b;

				// Kontrola, zda je k dispozici alespoň jeden bajt
				while (reader.BaseStream.Position < reader.BaseStream.Length &&
					  (b = reader.ReadByte()) != 0)
				{
					bytes.Add(b);

					// Bezpečnostní pojistka proti nekonečnému cyklu
					if (bytes.Count > 1000)
					{
						Console.WriteLine("Varování: Příliš dlouhý řetězec bez ukončovacího znaku.");
						break;
					}
				}

				return Encoding.GetEncoding("ISO-8859-1").GetString(bytes.ToArray());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Varování při čtení null-terminated string: {ex.Message}");
				return string.Empty;
			}
		}
	}	
}
