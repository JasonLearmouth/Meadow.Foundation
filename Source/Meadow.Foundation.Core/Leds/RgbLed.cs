﻿using Meadow.Hardware;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Peripherals.Leds;
using static Meadow.Peripherals.Leds.IRgbLed;

namespace Meadow.Foundation.Leds
{
    /// <summary>
    /// Represents an RGB LED
    /// </summary>
    public partial class RgbLed : IRgbLed
    {
        protected Task animationTask = null;
        protected CancellationTokenSource cancellationTokenSource = null;

        /// <summary>
        /// Get the color the LED has been set to.
        /// </summary>
        public Colors Color { get; protected set; }

        /// <summary>
        /// Get the red LED port
        /// </summary>
        public IDigitalOutputPort RedPort { get; set; }

        /// <summary>
        /// Get the green LED port
        /// </summary>
        public IDigitalOutputPort GreenPort { get; set; }

        /// <summary>
        /// Get the blue LED port
        /// </summary>
        public IDigitalOutputPort BluePort { get; set; }

        /// <summary>
        /// Is the LED using a common cathode
        /// </summary>
        public CommonType Common { get; protected set; }

        /// <summary>
        /// Turns on LED with current color or turns it off
        /// </summary>
        public bool IsOn 
        {
            get => isOn;
            set 
            {
                isOn = value;

                if (isOn)
                {
                    SetColor(Color);
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    SetColor(Colors.Black);
                }
            }
        }
        protected bool isOn;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Meadow.Foundation.Leds.RgbLed"/> class.
        /// </summary>
        /// <param name="device">IO Device</param>
        /// <param name="redPin">Red Pin</param>
        /// <param name="greenPin">Green Pin</param>
        /// <param name="bluePin">Blue Pin</param>
        /// <param name="commonType">Is Common Cathode</param>
        public RgbLed(
            IIODevice device, 
            IPin redPin, 
            IPin greenPin, 
            IPin bluePin, 
            CommonType commonType = CommonType.CommonCathode) :
            this (
                device.CreateDigitalOutputPort(redPin),
                device.CreateDigitalOutputPort(greenPin),
                device.CreateDigitalOutputPort(bluePin),
                commonType) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Meadow.Foundation.Leds.RgbLed"/> class.
        /// </summary>
        /// <param name="redPort">Red Port</param>
        /// <param name="greenPort">Green Port</param>
        /// <param name="bluePort">Blue Port</param>
        /// <param name="commonType">Is Common Cathode</param>
        public RgbLed(
            IDigitalOutputPort redPort, 
            IDigitalOutputPort greenPort,
            IDigitalOutputPort bluePort,
            CommonType commonType = CommonType.CommonCathode)
        {
            RedPort = redPort;
            GreenPort = greenPort;
            BluePort = bluePort;
            Common = commonType;

            cancellationTokenSource = new CancellationTokenSource();

            Color = Colors.White;
        }

        /// <summary>
        /// Stops any running animations.
        /// </summary>
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Sets the current color of the LED.
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Colors color)
        {
            color = Color;

            bool onState = (Common == CommonType.CommonCathode);

            switch (color)
            {
                case Colors.Red:
                    RedPort.State = onState;
                    GreenPort.State = !onState;
                    BluePort.State = !onState;
                    break;
                case Colors.Green:
                    RedPort.State = !onState;
                    GreenPort.State = onState;
                    BluePort.State = !onState;
                    break;
                case Colors.Blue:
                    RedPort.State = !onState;
                    GreenPort.State = !onState;
                    BluePort.State = onState;
                    break;
                case Colors.Yellow:
                    RedPort.State = onState;
                    GreenPort.State = onState;
                    BluePort.State = !onState;
                    break;
                case Colors.Magenta:
                    RedPort.State = onState;
                    GreenPort.State = !onState;
                    BluePort.State = onState;
                    break;
                case Colors.Cyan:
                    RedPort.State = !onState;
                    GreenPort.State = onState;
                    BluePort.State = onState;
                    break;
                case Colors.White:
                    RedPort.State = onState;
                    GreenPort.State = onState;
                    BluePort.State = onState;
                    break;
                case Colors.Black:
                    RedPort.State = !onState;
                    GreenPort.State = !onState;
                    BluePort.State = !onState;
                    break;
            }
        }

        /// <summary>
        /// Starts the blink animation.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="onDuration"></param>
        /// <param name="offDuration"></param>
        public void StartBlink(Colors color, uint onDuration = 200, uint offDuration = 200)
        {
            if (!cancellationTokenSource.Token.IsCancellationRequested)
                cancellationTokenSource.Cancel();

            SetColor(Colors.Black);

            animationTask = new Task(async () =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                await StartBlinkAsync(color, onDuration, offDuration, cancellationTokenSource.Token);
            });
            animationTask.Start();
        }
        protected async Task StartBlinkAsync(Colors color, uint onDuration, uint offDuration, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                SetColor(color);
                await Task.Delay((int)onDuration);
                SetColor(Colors.Black);
                await Task.Delay((int)offDuration);
            }
        }
    }
}