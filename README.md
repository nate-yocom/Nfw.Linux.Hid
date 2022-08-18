# Nfw.Linux.Hid

A convenient library for outputting Keyboard and Mouse events via a Linux USB Gadget + OTG + /dev/hidX device.

## Keyboard

```csharp
using Nfw.Linux.Hid.Keyboard;

SimpleKeyboard keyboard = new SimpleKeyboard();
keyboard.EmitString("test");
```

You can also use methdos to toggle keys (i.e. 'hold down SHIFT'), send specific keys etc.  ```SendKey``` or the [Events](https://github.com/nate-yocom/Nfw.Linux.Hid/blob/main/lib/Nfw.Linux.Hid/Keyboard/KeyEvents.cs) which can be sent via EmitEvent.

## Mouse

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

You should end up with /dev/hidg0 and /dev/hidg1 accordingly.
