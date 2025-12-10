using System;
using ConvertidorMonedas.App.Models;
using ConvertidorMonedas.App.Views;
using ConvertidorMonedas.Core;
using System.Collections.Generic;

namespace ConvertidorMonedas.App.Controllers
{
    public class ConversionController
    {
        private readonly Convertidor _convertidor = new Convertidor();

        // ExecuteConversion reads a model (demo) and calls the converter.
        public void ExecuteConversion()
        {
            MenuView.ShowMenu();
            var key = Console.ReadLine();
            if (key == "1")
            {
                var model = new ConversionModel
                {
                    Amount = 100m,
                    SourceCurrency = "USD",
                    TargetCurrency = "EUR",
                    Result = 0m
                };

                // Prepare a simple rates dictionary with example direct rates
                var rates = new Dictionary<string, decimal>
                {
                    ["USD->EUR"] = 0.9m,
                    ["EUR->USD"] = 1.1m
                };

                // Call the converter with required parameters and set the model result
                model.Result = _convertidor.MiConvertidorDeMonedas(model.Amount, model.SourceCurrency!, model.TargetCurrency!, rates);

                // Show result
                Console.WriteLine($"Converted {model.Amount} {model.SourceCurrency} -> {model.Result} {model.TargetCurrency}");
            }
            else
            {
                Console.WriteLine("Exiting");
            }
        }
    }
}
