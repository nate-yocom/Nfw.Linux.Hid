using Microsoft.Extensions.Logging;

using static MoreLinq.Extensions.BatchExtension;

namespace Nfw.Linux.Hid.Keyboard {
    public class SimpleKeyboard {        
        private readonly string _devicePath;

        private const int KEYBOARD_BUFFER_SIZE = 8;
        private const int MODIFIER_KEY_INDEX = 0;
        private const int FIRST_KEY_INDEX = 2;
        private const string DEFAULT_DEVICE = "/dev/hidg0";

        private byte[] _buffer = new byte[KEYBOARD_BUFFER_SIZE];
        private static List<Keys> _toggledKeys = new List<Keys>();
        
        private ILogger? _logger;
        
        public SimpleKeyboard(string devicePath, ILogger? logger) {
            _devicePath = devicePath;
            _logger = logger;
        }

        public SimpleKeyboard(string devicePath) : this(devicePath, null) {            
        }

        public SimpleKeyboard() : this(DEFAULT_DEVICE, null) { 
        }
        
        public void SendKey(Keys key, Modifiers modifiers) {
            WriteKeyboardMessage(Enumerable.Repeat(key, 1), modifiers);            
        }

        public void SendKeys(IEnumerable<Keys> keys, Modifiers modifiers) {
            WriteKeyboardMessage(keys, modifiers);            
        }
        
        public void EmitEvent(KeyEvent emittedEvent) {             
            for(int repeat = 0; repeat < emittedEvent.RepeatCount; repeat++) {
                Keys[] sendValues = _toggledKeys.Union(emittedEvent.Values).ToArray();
                switch(emittedEvent.Type) {
                    case KeyEvent.EventType.KeyPress:                    
                        SendKeys(sendValues, emittedEvent.Modifiers);
                        break;
                    case KeyEvent.EventType.KeyRelease:
                        SendKeys(_toggledKeys.ToArray(), Modifiers.NONE);
                        break;
                    case KeyEvent.EventType.KeyClick:                                        
                        SendKeys(sendValues, emittedEvent.Modifiers);
                        if (emittedEvent.HoldMilliseconds > 0) Thread.Sleep(emittedEvent.HoldMilliseconds);
                        SendKeys(_toggledKeys.ToArray(), Modifiers.NONE);
                        break;
                    case KeyEvent.EventType.KeyToggle:                    
                        // Press those which are not already toggled
                        IEnumerable<Keys> pressKeys = emittedEvent.Values.Except(_toggledKeys).ToList();      

                        // Subtract those which are already toggled
                        IEnumerable<Keys> releaseKeys = emittedEvent.Values.Intersect(_toggledKeys).ToList();

                        _toggledKeys.RemoveAll(x => releaseKeys.Contains(x));
                        _toggledKeys.AddRange(pressKeys);
                        SendKeys(_toggledKeys.ToArray(), Modifiers.NONE);
                        break;
                }
            }
        }

        public void EmitString(string emit) {
            List<KeyEvent> events = emit.ToCharArray().Select(c => { 
                KeyAndModifier km = KeyFromChar(c, ' ');
                return new KeyEvent() { 
                    Type = KeyEvent.EventType.KeyClick, 
                    RepeatCount = 1, 
                    HoldMilliseconds = 0, 
                    Values = new Keys[] { km.KeyScanCode }, 
                    Modifiers = km.ModifierScanCode 
                };
            }).ToList();

            foreach(KeyEvent ev in events) {
                EmitEvent(ev);
            }
        }

        private struct KeyAndModifier {
            public Keys KeyScanCode { get; set; }
            public Modifiers ModifierScanCode { get; set; }        
        }

        private static Dictionary<char, KeyAndModifier> s_charToScanCode = new Dictionary<char, KeyAndModifier>() {
            { 'a', new KeyAndModifier() { KeyScanCode = Keys.KEY_A, ModifierScanCode = Modifiers.NONE } },
            { 'b', new KeyAndModifier() { KeyScanCode = Keys.KEY_B, ModifierScanCode = Modifiers.NONE } },
            { 'c', new KeyAndModifier() { KeyScanCode = Keys.KEY_C, ModifierScanCode = Modifiers.NONE } },
            { 'd', new KeyAndModifier() { KeyScanCode = Keys.KEY_D, ModifierScanCode = Modifiers.NONE } },
            { 'e', new KeyAndModifier() { KeyScanCode = Keys.KEY_E, ModifierScanCode = Modifiers.NONE } },
            { 'f', new KeyAndModifier() { KeyScanCode = Keys.KEY_F, ModifierScanCode = Modifiers.NONE } },
            { 'g', new KeyAndModifier() { KeyScanCode = Keys.KEY_G, ModifierScanCode = Modifiers.NONE } },
            { 'h', new KeyAndModifier() { KeyScanCode = Keys.KEY_H, ModifierScanCode = Modifiers.NONE } },
            { 'i', new KeyAndModifier() { KeyScanCode = Keys.KEY_I, ModifierScanCode = Modifiers.NONE } },
            { 'j', new KeyAndModifier() { KeyScanCode = Keys.KEY_J, ModifierScanCode = Modifiers.NONE } },
            { 'k', new KeyAndModifier() { KeyScanCode = Keys.KEY_K, ModifierScanCode = Modifiers.NONE } },
            { 'l', new KeyAndModifier() { KeyScanCode = Keys.KEY_L, ModifierScanCode = Modifiers.NONE } },
            { 'm', new KeyAndModifier() { KeyScanCode = Keys.KEY_M, ModifierScanCode = Modifiers.NONE } },
            { 'n', new KeyAndModifier() { KeyScanCode = Keys.KEY_N, ModifierScanCode = Modifiers.NONE } },
            { 'o', new KeyAndModifier() { KeyScanCode = Keys.KEY_O, ModifierScanCode = Modifiers.NONE } },
            { 'p', new KeyAndModifier() { KeyScanCode = Keys.KEY_P, ModifierScanCode = Modifiers.NONE } },
            { 'q', new KeyAndModifier() { KeyScanCode = Keys.KEY_Q, ModifierScanCode = Modifiers.NONE } },
            { 'r', new KeyAndModifier() { KeyScanCode = Keys.KEY_R, ModifierScanCode = Modifiers.NONE } },
            { 's', new KeyAndModifier() { KeyScanCode = Keys.KEY_S, ModifierScanCode = Modifiers.NONE } },
            { 't', new KeyAndModifier() { KeyScanCode = Keys.KEY_T, ModifierScanCode = Modifiers.NONE } },
            { 'u', new KeyAndModifier() { KeyScanCode = Keys.KEY_U, ModifierScanCode = Modifiers.NONE } },
            { 'v', new KeyAndModifier() { KeyScanCode = Keys.KEY_V, ModifierScanCode = Modifiers.NONE } },
            { 'w', new KeyAndModifier() { KeyScanCode = Keys.KEY_W, ModifierScanCode = Modifiers.NONE } },
            { 'x', new KeyAndModifier() { KeyScanCode = Keys.KEY_X, ModifierScanCode = Modifiers.NONE } },
            { 'y', new KeyAndModifier() { KeyScanCode = Keys.KEY_Y, ModifierScanCode = Modifiers.NONE } },
            { 'z', new KeyAndModifier() { KeyScanCode = Keys.KEY_Z, ModifierScanCode = Modifiers.NONE } },
            { 'A', new KeyAndModifier() { KeyScanCode = Keys.KEY_A, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'B', new KeyAndModifier() { KeyScanCode = Keys.KEY_B, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'C', new KeyAndModifier() { KeyScanCode = Keys.KEY_C, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'D', new KeyAndModifier() { KeyScanCode = Keys.KEY_D, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'E', new KeyAndModifier() { KeyScanCode = Keys.KEY_E, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'F', new KeyAndModifier() { KeyScanCode = Keys.KEY_F, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'G', new KeyAndModifier() { KeyScanCode = Keys.KEY_G, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'H', new KeyAndModifier() { KeyScanCode = Keys.KEY_H, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'I', new KeyAndModifier() { KeyScanCode = Keys.KEY_I, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'J', new KeyAndModifier() { KeyScanCode = Keys.KEY_J, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'K', new KeyAndModifier() { KeyScanCode = Keys.KEY_K, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'L', new KeyAndModifier() { KeyScanCode = Keys.KEY_L, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'M', new KeyAndModifier() { KeyScanCode = Keys.KEY_M, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'N', new KeyAndModifier() { KeyScanCode = Keys.KEY_N, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'O', new KeyAndModifier() { KeyScanCode = Keys.KEY_O, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'P', new KeyAndModifier() { KeyScanCode = Keys.KEY_P, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'Q', new KeyAndModifier() { KeyScanCode = Keys.KEY_Q, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'R', new KeyAndModifier() { KeyScanCode = Keys.KEY_R, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'S', new KeyAndModifier() { KeyScanCode = Keys.KEY_S, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'T', new KeyAndModifier() { KeyScanCode = Keys.KEY_T, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'U', new KeyAndModifier() { KeyScanCode = Keys.KEY_U, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'V', new KeyAndModifier() { KeyScanCode = Keys.KEY_V, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'W', new KeyAndModifier() { KeyScanCode = Keys.KEY_W, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'X', new KeyAndModifier() { KeyScanCode = Keys.KEY_X, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'Y', new KeyAndModifier() { KeyScanCode = Keys.KEY_Y, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'Z', new KeyAndModifier() { KeyScanCode = Keys.KEY_Z, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { ' ', new KeyAndModifier() { KeyScanCode = Keys.KEY_SPACE, ModifierScanCode = Modifiers.NONE } },        
            { '1', new KeyAndModifier() { KeyScanCode = Keys.KEY_1, ModifierScanCode = Modifiers.NONE } },
            { '2', new KeyAndModifier() { KeyScanCode = Keys.KEY_2, ModifierScanCode = Modifiers.NONE } },
            { '3', new KeyAndModifier() { KeyScanCode = Keys.KEY_3, ModifierScanCode = Modifiers.NONE } },
            { '4', new KeyAndModifier() { KeyScanCode = Keys.KEY_4, ModifierScanCode = Modifiers.NONE } },
            { '5', new KeyAndModifier() { KeyScanCode = Keys.KEY_5, ModifierScanCode = Modifiers.NONE } },
            { '6', new KeyAndModifier() { KeyScanCode = Keys.KEY_6, ModifierScanCode = Modifiers.NONE } },
            { '7', new KeyAndModifier() { KeyScanCode = Keys.KEY_7, ModifierScanCode = Modifiers.NONE } },
            { '8', new KeyAndModifier() { KeyScanCode = Keys.KEY_8, ModifierScanCode = Modifiers.NONE } },
            { '9', new KeyAndModifier() { KeyScanCode = Keys.KEY_9, ModifierScanCode = Modifiers.NONE } },
            { '0', new KeyAndModifier() { KeyScanCode = Keys.KEY_0, ModifierScanCode = Modifiers.NONE } },
            { '!', new KeyAndModifier() { KeyScanCode = Keys.KEY_1, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '@', new KeyAndModifier() { KeyScanCode = Keys.KEY_2, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '#', new KeyAndModifier() { KeyScanCode = Keys.KEY_3, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '$', new KeyAndModifier() { KeyScanCode = Keys.KEY_4, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '%', new KeyAndModifier() { KeyScanCode = Keys.KEY_5, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '^', new KeyAndModifier() { KeyScanCode = Keys.KEY_6, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '&', new KeyAndModifier() { KeyScanCode = Keys.KEY_7, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '*', new KeyAndModifier() { KeyScanCode = Keys.KEY_8, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '(', new KeyAndModifier() { KeyScanCode = Keys.KEY_9, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { ')', new KeyAndModifier() { KeyScanCode = Keys.KEY_0, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },        
        };

        private static KeyAndModifier s_defaultUnknownChar = new KeyAndModifier() { KeyScanCode = Keys.KEY_SPACE, ModifierScanCode = Modifiers.NONE };

        private static KeyAndModifier KeyFromChar(char input) {
            return KeyFromChar(input, ' ');
        }
        private static KeyAndModifier KeyFromChar(char input, char def) {
            return s_charToScanCode.ContainsKey(input) ? s_charToScanCode[input] : s_charToScanCode.ContainsKey(def) ? s_charToScanCode[def] : s_defaultUnknownChar;
        }
        
        private void WriteKeyboardMessage(IEnumerable<Keys> key, Modifiers modifiers) {            
            try {
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(_devicePath))) {
                    foreach(IEnumerable<Keys> batch in key.Batch(KEYBOARD_BUFFER_SIZE - FIRST_KEY_INDEX)) {
                        Array.Clear(_buffer, 0, _buffer.Length);
                        _buffer[MODIFIER_KEY_INDEX] = (byte) modifiers;
                        for(int x = 0; x < batch.Count() && (FIRST_KEY_INDEX + x < _buffer.Length); x++)
                            _buffer[FIRST_KEY_INDEX + x] = (byte) batch.ElementAt(x);
                        writer.Write(_buffer, 0, _buffer.Length);
                        writer.Flush();        
                    }                    
                }
            } catch (Exception ex) {
                _logger?.LogWarning($"Unable to output to {_devicePath} => {ex.Message}");
            }
        }     
    }
}