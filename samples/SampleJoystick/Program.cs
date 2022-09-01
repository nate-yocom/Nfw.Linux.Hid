using Nfw.Linux.Hid.Joystick;

string hidDevice = args.Count() > 0 ? args[0] : "/dev/hidg2";

Console.WriteLine($"Pressing buttons via {hidDevice} in 5 seconds...");
Thread.Sleep(5000);
Console.WriteLine("... now");

SimpleJoystick joystick = new SimpleJoystick(hidDevice);

// First press them all one by one
Console.WriteLine("Pressing all the buttons in order...");
for (byte buttonId = 0; buttonId <= SimpleJoystick.MAX_BUTTON_ID; buttonId++) {
    joystick.UpdateButton(buttonId, true);
}

Thread.Sleep(1000);

// Now release them all one by one
Console.WriteLine("Release all the buttons in order...");
for (byte buttonId = 0; buttonId <= SimpleJoystick.MAX_BUTTON_ID; buttonId++) {
    joystick.UpdateButton(buttonId, false);
}

Thread.Sleep(1000);

// Now press all the odd ones at the same time
Console.WriteLine("Press all odd buttons at same time...");
joystick.UpdateButtons(new byte[] { 1, 3, 5, 7, 9, 11, 13, 15 }, true);

Thread.Sleep(1000);

// And release them all at once
Console.WriteLine("Release all odd buttons at same time...");
joystick.UpdateButtons(new byte[] { 1, 3, 5, 7, 9, 11, 13, 15 }, false);


Console.WriteLine("... Done");