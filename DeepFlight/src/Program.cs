using System;

public static class Program {
    [STAThread]
    static void Main() {
        var game = new GameController();
        game.Run();
    }
}