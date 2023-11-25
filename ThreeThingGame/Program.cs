internal class Program
{
    private static void Main(string[] args)
    {
        using var game = new ThreeThingGame.Game1();
        game.Window.IsBorderless = true;
        game.Run();
    }
}