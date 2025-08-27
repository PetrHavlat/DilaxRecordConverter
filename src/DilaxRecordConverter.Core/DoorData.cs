namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
	public class DoorData
	{
		public uint DeviceId { get; set; }
		public byte Instance { get; set; }
		public short BoardingPassengers { get; set; }
		public short AlightingPassengers { get; set; }
		public short UncertainPassengers { get; set; }

		// Celkový počet pasažérů (pro zpětnou kompatibilitu)
		public short TotalCount => (short)(BoardingPassengers + AlightingPassengers);
	}
	
}

