using Microsoft.Extensions.Logging;

namespace Nfw.Linux.Hid.Mouse {
    public class SimpleMouse {
        public const int MAX_X = 32767;
        public const int MAX_Y = 32767;
                                       
        private ILogger? _logger;
        private readonly string _devicePath; 
        private int _currentX = 0;
        private int _currentY = 0;
        private byte[] _buffer = new byte[MOUSE_MESSAGE_BUFFER_SIZE];
        private bool _leftButtonToggle = false;
        
        private const string DEFAULT_DEVICE = "/dev/hidg1";
        private const int MOUSE_MESSAGE_BUFFER_SIZE = 7;        


        public int CurrentX { get { return _currentX; } }
        public int CurrentY { get { return _currentY; } }


        public SimpleMouse(string devicePath, ILogger? logger) {
            _devicePath = devicePath;
            _logger = logger;
        }

        public SimpleMouse(string devicePath) : this(devicePath, null) {            
        }

        public SimpleMouse() : this(DEFAULT_DEVICE, null) {
        }


        public void Center() {
            MoveRelativeToScreen(0.50f, 0.50f);
        }

        public void LeftClick() {
            // Click is a push AND release
            WriteMouseMessage(true, false, _currentX, _currentY, 0, 0);
            WriteMouseMessage(false, false, _currentX, _currentY, 0, 0);
        }

        public void DoubleClick() {
            LeftClick();
            Thread.Sleep(100);
            LeftClick();
        }

        public void RightClick() {
            // Click is a push AND release
            WriteMouseMessage(false, true, _currentX, _currentY, 0, 0);
            WriteMouseMessage(false, false, _currentX, _currentY, 0, 0);
        }

        public void ButtonClick(byte buttons) {
            WriteMouseMessageInternal(buttons, _currentX, _currentY, 0, 0);
            WriteMouseMessageInternal((byte)0x00, _currentX, _currentY, 0, 0);
        }

        public void MoveRelativeToScreen(float scaledX, float scaledY) {
            int targetX = (int)(MAX_X * scaledX);
            int targetY = (int)(MAX_Y * scaledY);

            // Later, may need to track state of mouse buttons, so we can do click->drag->release?
            WriteMouseMessage(_leftButtonToggle, false, targetX, targetY, 0, 0);
        }

        // Move relative to the current position based on scale - i.e. X = 0.50 means 'move half again to right on X'
        public void MoveScaledRelativeToCurrent(float relativeX, float relativeY) {
            int x = (int) (_currentX * relativeX);
            int y = (int) (_currentY * relativeY);
            WriteMouseMessage(_leftButtonToggle, false, x, y, 0, 0);
        }

        public void MoveRelativeToCurrent(int deltaX, int deltaY) {
            int x = _currentX + deltaX;
            int y = _currentY + deltaY;
            WriteMouseMessage(_leftButtonToggle, false, x, y, 0, 0);
        }

        public void MoveTo(int x, int y) {
            WriteMouseMessage(_leftButtonToggle, false, x, y, 0, 0);
        }

        public void ScrollVertical(byte delta) {
            WriteMouseMessage(_leftButtonToggle, false, _currentX, _currentY, delta, 0);
        }

        public void ScrollHorizontal(byte delta) {
            WriteMouseMessage(_leftButtonToggle, false, _currentX, _currentY, 0, delta);
        }

        public void ToggleLeftButton() {
            _logger?.LogDebug($"Toggling left button Previous: {_leftButtonToggle}");
            _leftButtonToggle = !_leftButtonToggle;
            WriteMouseMessage(_leftButtonToggle, false, _currentX, _currentY, 0, 0);
            _logger?.LogDebug($"Toggling left button Now: {_leftButtonToggle}");
        }
                
        private int BoxVal(int proposed, int max) {
            return Math.Max(Math.Min(proposed, max), 1);
        }

        private void WriteMouseMessage(bool button1, int actualX, int actualY, int vWheelDelta, int hWheelDelta) {
            WriteMouseMessageInternal((byte) (button1 ? 0x01 : 0x00), actualX, actualY, hWheelDelta, hWheelDelta);
        }

        private void WriteMouseMessage(bool button1, bool button2, int actualX, int actualY, int vWheelDelta, int hWheelDelta) { 
            byte buttons = 0x00;
            buttons = (byte)(0x00 | (button1 ? 0x01 : 0x00));
            buttons = (byte)(buttons | (button2 ? 0x02 : 0x00));
            WriteMouseMessageInternal(buttons, actualX, actualY, hWheelDelta, hWheelDelta);
        }

        private void WriteMouseMessage(bool button1, bool button2, bool button3, int actualX, int actualY, int vWheelDelta, int hWheelDelta) { 
            byte buttons = 0x00;
            buttons = (byte)(0x00 | (button1 ? 0x01 : 0x00));
            buttons = (byte)(buttons | (button2 ? 0x02 : 0x00));
            buttons = (byte)(buttons | (button3 ? 0x04 : 0x00));
            WriteMouseMessageInternal(buttons, actualX, actualY, hWheelDelta, hWheelDelta);
        }

        private void WriteMouseMessageInternal(byte buttons, int actualX, int actualY, int vWheelDelta, int hWheelDelta) {
            actualX = BoxVal(actualX, MAX_X);
            actualY = BoxVal(actualY, MAX_Y);

            // Caller disabled left button explicitly
            byte button1on = (byte) (buttons & (byte )0x01);
            if (button1on == 0x00 && _leftButtonToggle) {
                _logger?.LogDebug($"Auto-de-flagging left toggle");
                _leftButtonToggle = false;
            }

            _logger?.LogDebug($"From: {_currentX},{_currentY} => {actualX},{actualY}, Buttons[{buttons}] LeftToggle[{_leftButtonToggle}]");
            
            _currentX = actualX;
            _currentY = actualY;            

            Array.Clear(_buffer, 0, _buffer.Length);
            _buffer[0] = buttons;            
            _buffer[1] = (byte) (actualX & 0xff);
            _buffer[2] = (byte) ((actualX >> 8) & 0xff);        
            _buffer[3] = (byte) (actualY & 0xff);
            _buffer[4] = (byte) ((actualY >> 8) & 0xff);        
            _buffer[5] = (byte) vWheelDelta;
            _buffer[6] = (byte) hWheelDelta;

            try {
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(_devicePath))) {
                    writer.Write(_buffer, 0, _buffer.Length);
                    writer.Flush();
                }
            } catch (Exception ex) {
                _logger?.LogWarning($"Unable to write message: {ex.Message}");
            }
        }        
    }

}