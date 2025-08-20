using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	public abstract class Dlx3Block
	{
		public ushort Length { get; set; }
		public string BlockType { get; set; }
		public ushort Crc { get; set; }

		public byte[] Data { get; set; }

		public abstract void ParseData(byte[] data);

		////public virtual bool ValidateCrc(byte[] typeAndData)
		////{
		////	return Crc16.ComputeChecksum(typeAndData) == Crc;
		////}
	}
}
