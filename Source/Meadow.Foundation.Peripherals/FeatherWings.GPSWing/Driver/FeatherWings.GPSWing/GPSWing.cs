﻿using Meadow.Hardware;
using Sensors.Location.MediaTek;

namespace Meadow.Foundation.FeatherWings.GPSWing
{
    public class GPSWing : Mt3339
    {
        public GPSWing(ISerialMessagePort serialMessagePort, int baud = 9600)
            : base(serialMessagePort, baud)
        {
        }
    }
}