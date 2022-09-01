# Nfw.Linux.Hid

A convenient library for outputting Keyboard and Mouse events via a Linux USB Gadget + OTG + /dev/hidX device.

## NuGet

```dotnet add package Nfw.Linux.Hid```

## Sample: Keyboard

```csharp
using Nfw.Linux.Hid.Keyboard;

SimpleKeyboard keyboard = new SimpleKeyboard();
keyboard.EmitString("test");
```

You can also use methdos to toggle keys (i.e. 'hold down SHIFT'), send specific keys etc.  ```SendKey``` or the [Events](https://github.com/nate-yocom/Nfw.Linux.Hid/blob/main/lib/Nfw.Linux.Hid/Keyboard/KeyEvents.cs) which can be sent via EmitEvent.

## Sample: Mouse

```csharp
using Nfw.Linux.Hid.Mouse;

SimpleMouse mouse = new SimpleMouse();

// Start at center
mouse.Center();

// Move diagonally from top left, to bottom right
for(int x = 0; x < SimpleMouse.MAX_X; x++) {    
    mouse.MoveTo(x,x);
    Thread.Sleep(1);    
}
```

Note additional methods for clicking, toggling (press-and-hold), etc are also available.

## Sample: Joystick

```csharp
using Nfw.Linux.Hid.Joystick;

string hidDevice = args.Count() > 0 ? args[0] : "/dev/hidg2";

SimpleJoystick joystick = new SimpleJoystick(hidDevice);

// First press them all one by one
for (byte buttonId = 0; buttonId <= SimpleJoystick.MAX_BUTTON_ID; buttonId++) {
    joystick.UpdateButton(buttonId, true);
}

// Now release them all one by one
for (byte buttonId = 0; buttonId <= SimpleJoystick.MAX_BUTTON_ID; buttonId++) {
    joystick.UpdateButton(buttonId, false);
}

// Press all the odd ones at the same time
joystick.UpdateButtons(new byte[] { 1, 3, 5, 7, 9, 11, 13, 15 }, true);
// And release them all at once
joystick.UpdateButtons(new byte[] { 1, 3, 5, 7, 9, 11, 13, 15 }, false);
```

## References
- https://www.rmedgar.com/blog/using-rpi-zero-as-keyboard-setup-and-device-definition/
- https://www.elinux.org/images/e/ef/USB_Gadget_Configfs_API_0.pdf
- https://www.usb.org/sites/default/files/documents/hid1_11.pdf
- http://www.linux-usb.org/usb.ids
- https://github.com/tiny-pilot/tinypilot/blob/master/app/main.py


## Attribution
- [Keyboard icons created by Freepik - Flaticon](https://www.flaticon.com/free-icons/keyboard)
- Much inspiration, and OTG/HID setup code, from the [TinyPilot](https://tinypilotkvm.com) project

## Creating the HID devices

Creating the HID devices via USB Gadget is a bit out of scope for this library, but for reference, the method used by the author is included in the repo [here](https://github.com/nate-yocom/Nfw.Linux.Hid/tree/main/hid-scripts).  Only tested on rasbian-lite 32bit on a R-Pi4.  Requires the dwc2 OTG driver/overlay:

```bash
echo "dtoverlay=dwc2" >> /boot/config.txt
echo "dwc2" >> /etc/modules
```

Then reboot, and run:

```bash
sudo ./init-usb-gadget.sh
sudo ./install-usb-gadget.sh
```

You should end up with /dev/hidg0, hidg1, hidg2 accordingly.

Note that the samples and code in this lib presume the use of the report descriptors from these scripts, i.e. a 8 byte keyboard report (2 status, 6 keys), a 7 byte mouse report, and an 11 byte joystick (16 buttons, 9 axis).
