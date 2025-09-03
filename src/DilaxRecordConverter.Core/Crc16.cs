using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DilaxRecordConverter.Core
{
	public static class Crc16
	{
		private const ushort polynomial = 0x1021;
		private static readonly ushort[] table = new ushort[256];

		static Crc16()
		{
			for (var i = 0; i < table.Length; ++i)
			{
				ushort value = 0;
				var temp = (ushort)(i << 8);
				for (var j = 0; j < 8; ++j)
				{
					if (((value ^ temp) & 0x8000) != 0)
						value = (ushort)((value << 1) ^ polynomial);
					else
						value <<= 1;
					temp <<= 1;
				}
				table[i] = value;
			}
		}

		public static ushort ComputeChecksum(byte[] bytes)
		{
			ushort crc = 0xFFFF;
			foreach (byte b in bytes)
				crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ (0xFF & b)]);
			return crc;
		}
	}
}
