using Nfw.Linux.Hid.Keyboard;

Console.WriteLine("Outputting \"test\" to keyboard on /dev/hidg0 in 5 seconds...");
Thread.Sleep(5000);
Console.WriteLine("... now");

SimpleKeyboard keyboard = new SimpleKeyboard();
keyboard.EmitString("test");
