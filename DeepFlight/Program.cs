using System;

public static class Program {
    [STAThread]
    static void Main() {
        var game = new Controller();
        game.Run();

        uint n1 = 43;
        uint n2 = 43 % 100;

        Console.WriteLine(n1);
        Console.WriteLine(n2);
    }

    // Testing git 3 
}