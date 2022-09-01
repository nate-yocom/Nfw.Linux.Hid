using Nfw.Linux.Hid.Mouse;

string hidDevice = args.Count() > 0 ? args[0] : "/dev/hidg1";

Console.WriteLine($"Moving the mouse via {hidDevice} corner to corner in 5 seconds...");
Thread.Sleep(5000);
Console.WriteLine("... now");

SimpleMouse mouse = new SimpleMouse(hidDevice);

mouse.Center();

for(int x = 10; x < SimpleMouse.MAX_X; x++) {    
    mouse.MoveTo(x,x);
    Thread.Sleep(1);    
}

Console.WriteLine("... Done");