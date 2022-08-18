namespace Nfw.Linux.Hid.Keyboard {
    public class KeyEvent {
        public enum EventType {
            KeyToggle,     
            KeyPress,
            KeyRelease,
            KeyClick,    // A press and release shortcut            
        }

        public int RepeatCount { get; set; } = 1;
        public EventType Type { get; set; } = EventType.KeyClick;
        public Keys[] Values { get; set; } = new Keys[0];
        public Modifiers Modifiers { get; set; } = Modifiers.NONE;
        public int HoldMilliseconds { get; set; } = 0;
    }
}