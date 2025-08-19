using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DilaxRecordConverter.Core.Helpers
{
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

		public static short ReadIntValue(BinaryReader reader)
		{
			return ReadValue(reader, r => r.ReadInt16(), sizeof(short), nameof(Int16));
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
					throw new EndOfStreamException("Nedostatek dat pro čtení 2 bajtů pro ushort.");
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
