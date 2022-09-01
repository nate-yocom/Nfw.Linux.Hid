using Microsoft.Extensions.Logging;

namespace Nfw.Linux.Hid.Joystick {

    /**
     * Currently presumes a 16 button, 9 axis joystick (as created by the HID scripts in repo)
     */     

    public class SimpleJoystick {        
        private const int REPORT_SIZE = 11;
        private const int AXIS_OFFSET = 2;        
        private const string DEFAULT_DEVICE = "/dev/hidg2";

        public const int MAX_BUTTON_ID = 15;
        public const int MAX_AXIS_ID = 8;
        public const sbyte MAX_AXIS_VALUE = sbyte.MaxValue;
        public const sbyte MIN_AXIS_VALUE = sbyte.MinValue;
        
        private ILogger? _logger;
        private readonly string _devicePath; 
        private byte[] _buffer = new byte[REPORT_SIZE];                
        private ushort _buttonState = 0x0000;
        
        public SimpleJoystick(string devicePath, ILogger? logger) {
            _devicePath = devicePath;
            _logger = logger;
        }

        public SimpleJoystick(string devicePath) : this(devicePath, null) {            
        }

        public SimpleJoystick() : this(DEFAULT_DEVICE, null) {
        }

        public void UpdateAxis(byte axisId, sbyte value) {
            if (axisId > MAX_AXIS_ID) throw new Exception($"Invalid Axis: {axisId}");
            _buffer[AXIS_OFFSET + axisId] = (byte) value;
            SendReport();
         }

        public void UpdateButton(byte buttonId, bool pressed) {
            SetButton(buttonId, pressed);
            SendReport();
        }

        public void UpdateButtons(IEnumerable<byte> buttonIds, bool pressed) {
            foreach(byte buttonId in buttonIds) {
                SetButton(buttonId, pressed);
            }
            SendReport();
        }

        private void SetButton(byte buttonId, bool pressed) {
            if (buttonId > MAX_BUTTON_ID) throw new Exception($"Invalid Button: {buttonId}");
            if (pressed) {
                _buttonState |= (ushort) (0x01 << buttonId);
            } else {
                _buttonState &= (ushort) (~(0x01 << buttonId));
            }
            BitConverter.TryWriteBytes(_buffer, _buttonState);
        }

        private void SendReport() {
            try {
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(_devicePath))) {
                    writer.Write(_buffer, 0, _buffer.Length);
                    writer.Flush();
                }
            } catch(Exception ex) {
                _logger?.LogWarning($"Exception while writing buffer [{Convert.ToHexString(_buffer)}]: {ex}");
            }
        }        
    }
}