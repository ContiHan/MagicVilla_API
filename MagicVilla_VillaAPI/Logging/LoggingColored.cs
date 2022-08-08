namespace MagicVilla_VillaAPI.Logging
{
    public class LoggingColored : ILogging
    {
        private readonly ConsoleColor errorColor = ConsoleColor.Red;
        private readonly ConsoleColor warningColor = ConsoleColor.Yellow;
        private readonly ConsoleColor infoColor = ConsoleColor.Blue;
        private readonly ConsoleColor originColor = ConsoleColor.Gray;

        public void Log(string message, string type)
        {

            switch (type)
            {
                case "error":
                    Console.ForegroundColor = errorColor;
                    Console.Write("ERROR:\t");
                    Console.ForegroundColor = originColor;
                    break;
                case "warning":
                    Console.ForegroundColor = warningColor;
                    Console.Write("WARNING:\t");
                    Console.ForegroundColor = originColor;
                    break;
                default:
                    Console.ForegroundColor = infoColor;
                    Console.Write("INFO:\t");
                    Console.ForegroundColor = originColor;
                    break;
            }
            Console.WriteLine(message);
        }
    }
}
