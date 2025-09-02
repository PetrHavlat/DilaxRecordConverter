using DilaxRecordConverter.Core;

namespace DLX3Converter.Dlx3Conversion.Dlx3Bloky
{
    /// <summary>
	/// Reprezentuje jednu diagnostickou zprávu v DIAG bloku.
	/// </summary>
	public class DiagnosticMessage
	{
		/// <summary>
		/// Získá časové razítko, kdy byla tato zpráva zapsána.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		/// Získá ID modulu.
		/// </summary>
		public byte ModuleId { get; set; }

		/// <summary>
		/// Získá ID podmodulu.
		/// </summary>
		public byte SubModuleId { get; set; }

		/// <summary>
		/// Získá ID zprávy.
		/// </summary>
		public byte MessageId { get; set; }

		/// <summary>
		/// Získá kategorii zprávy.
		/// </summary>
		public byte Category { get; set; }

		/// <summary>
		/// Získá text zprávy.
		/// </summary>
		public string? Message { get; set; }

		/// <summary>
		/// Získá, zda zpráva obsahuje informace o zařízení (Module ID = 20).
		/// </summary>
		public bool HasDeviceInfo => ModuleId == 20;

		// Parsované hodnoty pro Module ID 20
		public uint? DeviceId { get; set; }
		public byte? DoorInstance { get; set; }
		public string? AdditionalInfo { get; set; }

		/// <summary>
		/// Získá popis diagnostické zprávy.
		/// </summary>
		public string Description => DiagnosticCodes.GetMessageDescription(ModuleId, SubModuleId, MessageId);

		/// <summary>
		/// Získá popis kategorie.
		/// </summary>
		public string CategoryDescription => DiagnosticCodes.GetCategoryDescription(Category);

		/// <summary>
		/// Parsuje informace o zařízení z textu zprávy pro Module ID 20.
		/// </summary>
		public void ParseDeviceInfo()
		{
			if (!HasDeviceInfo || string.IsNullOrEmpty(Message))
				return;

			var parts = Message.Split(',');
			foreach (var part in parts)
			{
				var keyValue = part.Split(':');
				if (keyValue.Length != 2)
					continue;

				var key = keyValue[0].Trim();
				var value = keyValue[1].Trim();

				switch (key)
				{
					case "addr":
						if (uint.TryParse(value, out uint deviceId))
							DeviceId = deviceId;
						break;
					case "inst":
						if (byte.TryParse(value, out byte instance))
							DoorInstance = instance;
						break;
					case "info":
					case "time":
						AdditionalInfo = value;
						break;
				}
			}
		}

		/// <summary>
		/// Vrací řetězcovou reprezentaci diagnostické zprávy.
		/// </summary>
		public override string ToString()
		{
			if (HasDeviceInfo)
			{
				return $"[{Timestamp}] {Description} - Device: {DeviceId}, Instance: {DoorInstance}, Info: {AdditionalInfo} ({CategoryDescription})";
			}
			else
			{
				return $"[{Timestamp}] {Description} - Module: {ModuleId}, SubModule: {SubModuleId}, MsgID: {MessageId} ({CategoryDescription})";
			}
		}
	}
}

