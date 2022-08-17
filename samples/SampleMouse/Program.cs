using Nfw.Linux.Hid.Mouse;

Console.WriteLine("Moving the mouse corner to corner in 5 seconds...");
Thread.Sleep(5000);
Console.WriteLine("... now");

SimpleMouse mouse = new SimpleMouse();

mouse.Center();

for(int x = 10; x < SimpleMouse.MAX_X; x++) {    
    mouse.MoveTo(x,x);
    Thread.Sleep(1);    
}

Console.WriteLine("... Done");