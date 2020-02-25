using System;

public static class Program {
    [STAThread]
    static void Main() {
        var game = new Controller();
        game.Run();

        //int n1 = Mod(200, 500);
        //int n2 = Mod(-200, 500);

        //Console.WriteLine(n1);
        //Console.WriteLine(n2);
    }
}