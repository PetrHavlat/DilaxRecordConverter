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
		private static T ReadValue<T>(BinaryReader reader, Func<BinaryReader, T> readFunc, int sizeInBytes, string typeName)
		{
			try
			{
				var availableBytes = reader.BaseStream.Length - reader.BaseStream.Position;
				
				if (availableBytes < sizeInBytes)
				{
					throw new EndOfStreamException($"Nedostatek dat pro načtení {sizeInBytes} bajtu pro {typeName} hodnotu. Dostupné: {availableBytes} bajtů.");
				}

				return readFunc(reader);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení {typeName}: {ex.Message}");
				throw; // zachovává stack trace
			}
		}

		public static bool ReadBoolValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadBoolean(), sizeof(bool), nameof(Boolean));
		}

		public static byte ReadByteValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadByte(), sizeof(byte), nameof(Byte));
		}

		public static sbyte ReadSByteValue(BinaryReader reader)
		{

			return ReadValue(reader, r => r.ReadSByte(), sizeof(sbyte), nameof(SByte));
		}

		public static short ReadShortValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadInt16(), sizeof(short), nameof(Int16));
		}

		public static ushort ReadUShortValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadUInt16(), sizeof(ushort), nameof(UInt16));
		}

		public static int ReadIntValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadInt32, sizeof(int), nameof(Int32));
		}

		public static ushort ReadUIntValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadUInt16(), sizeof(ushort), nameof(UInt16));
		}

		public static uint ReadUIntBigEndianValue(BinaryReader reader)
		{
			var byteCount = 4;
			
			try
			{
				// Kontrola, zda je k dispozici dostatek dat
				if (reader.BaseStream.Length - reader.BaseStream.Position < byteCount)
				{
					throw new EndOfStreamException($"Nedostatek dat pro čtení {byteCount} bajtů pro ushort.");
				}

				var bytes = reader.ReadBytes(byteCount);
				Array.Reverse(bytes);
				return BitConverter.ToUInt32(bytes, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení ushort: {ex.Message}");
				throw ex;
			}
		}

		public static uint ReadUIntSmallEndianValue(BinaryReader reader)
		{
			var byteCount = 4;
			
			try
			{
				// Kontrola, zda je k dispozici dostatek dat
				if (reader.BaseStream.Length - reader.BaseStream.Position < byteCount)
				{
					throw new EndOfStreamException("Nedostatek dat pro čtení 2 bajtů pro ushort.");
				}

				var bytes = reader.ReadBytes(byteCount);

				return BitConverter.ToUInt32(bytes, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení ushort: {ex.Message}");
				throw ex;
			}
		}

		public static int ReadIntBigEndianValue(BinaryReader reader)
		{
			var byteCount = 4;
			
			try
			{
				// Kontrola, zda je k dispozici dostatek dat
				if (reader.BaseStream.Length - reader.BaseStream.Position < byteCount)
				{
					throw new EndOfStreamException($"Nedostatek dat pro čtení {byteCount} bajtů.");
				}

				var bytes = reader.ReadBytes(byteCount);
				Array.Reverse(bytes);
				return BitConverter.ToInt32(bytes, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení ushort: {ex.Message}");
				throw ex;
			}
		}

		public static int ReadIntSmallEndianValue(BinaryReader reader)
		{
			var byteCount = 4;
			
			try
			{
				// Kontrola, zda je k dispozici dostatek dat
				if (reader.BaseStream.Length - reader.BaseStream.Position < byteCount)
				{
					throw new EndOfStreamException("Nedostatek dat pro čtení 2 bajtů pro ushort.");
				}

				var bytes = reader.ReadBytes(byteCount);

				return BitConverter.ToInt32(bytes, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při čtení ushort: {ex.Message}");
				throw ex;
			}
		}
	}
	
}
