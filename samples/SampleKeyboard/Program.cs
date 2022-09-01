using Nfw.Linux.Hid.Keyboard;

string hidDevice = args.Count() > 0 ? args[0] : "/dev/hidg0";

Console.WriteLine($"Outputting \"test\" to keyboard on {hidDevice} in 5 seconds...");
Thread.Sleep(5000);
Console.WriteLine("... now");

SimpleKeyboard keyboard = new SimpleKeyboard(hidDevice);
keyboard.EmitString("test");
