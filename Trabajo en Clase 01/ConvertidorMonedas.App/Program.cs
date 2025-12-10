using System;
using ConvertidorMonedas.App.Controllers;

namespace ConvertidorMonedas.App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var controller = new ConversionController();
            controller.ExecuteConversion();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
