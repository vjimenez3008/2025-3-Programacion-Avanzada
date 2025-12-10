using System;

namespace ConvertidorMonedas.App.Views
{
    public static class MenuView
    {
        public static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Currency Converter ===");
            Console.WriteLine("1) Convert currency (sample)");
            Console.WriteLine("0) Exit");
            Console.Write("Choose an option: ");
        }
    }
}
