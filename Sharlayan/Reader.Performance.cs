// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.CurrentPlayer.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.CurrentPlayer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

	using Sharlayan.Core;
	using Sharlayan.Core.Enums;
	using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

	using BitConverter = Sharlayan.Utilities.BitConverter;

	public static partial class Reader {
        public static bool CanGetPerformance() {
			//var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.PerformanceLayoutKey) && Scanner.Instance.Locations.ContainsKey(Signatures.PerformanceStatusKey);
			var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.PerformanceStatusKey);
			if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public static PerformanceResult GetPerformance() {
            var result = new PerformanceResult();

            if (!CanGetPerformance() || !MemoryHandler.Instance.IsAttached) {
                return result;
            }


            try {
				var PerformanceStatusMap = (IntPtr) Scanner.Instance.Locations[Signatures.PerformanceStatusKey];

				int entrySize = 12;
				int numEntries = 99;
				byte[] performanceData = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations[Signatures.PerformanceStatusKey], entrySize * numEntries);

				for(int i = 0; i < numEntries; i++) {
					int offset = (i * entrySize);

					float animation = BitConverter.TryToSingle(performanceData, 0);
					byte unknown1 = performanceData[4];
					byte id = performanceData[5];
					byte variant = performanceData[6];
					byte type = performanceData[7];
					byte status = performanceData[8];
					byte instrument = performanceData[9];
					int unknown2 = BitConverter.TryToInt16(performanceData, 10);

					if(id >= 0 && id <= 99) {
						PerformanceItem item = new PerformanceItem();
						item.Animation = animation;
						item.Unknown1 = (byte)unknown1;
						item.Id = (byte) id;
						item.Variant = (byte) variant;
						item.Type = (byte) type;
						item.Status = (Performance.Status) status;
						item.Instrument = (Performance.Instrument) instrument;

						if(!result.Performances.ContainsKey(id)) {
							result.Performances[id] = item;
						}
					}
				}
				/*
				var PerformanceLayoutMap = (IntPtr) Scanner.Instance.Locations[Signatures.PerformanceLayoutKey];

				byte layout = MemoryHandler.Instance.GetByte(PerformanceLayoutMap);
				result.Layout = layout;
				*/
            }
            catch (Exception ex) {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}