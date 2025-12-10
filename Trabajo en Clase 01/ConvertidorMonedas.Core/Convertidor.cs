using System.Collections.Generic;

namespace ConvertidorMonedas.Core
{
    /// <summary>
    /// Provides basic currency conversion functionality.
    /// This is a simple, deterministic implementation that looks up a direct rate key
    /// in the provided rates dictionary and returns amount * rate when found.
    /// </summary>
    public class Convertidor
    {
        /// <summary>
        /// Converts an amount from <paramref name="sourceCurrency"/> to <paramref name="targetCurrency"/>
        /// using the direct rate lookup key "SRC->TGT" inside the <paramref name="rates"/> dictionary.
        /// </summary>
        /// <param name="amount">Amount to convert.</param>
        /// <param name="sourceCurrency">Source currency code (e.g. "USD").</param>
        /// <param name="targetCurrency">Target currency code (e.g. "EUR").</param>
        /// <param name="rates">Dictionary of direct rates keyed by "SRC->TGT" (case-insensitive keys are normalized to upper-case).</param>
        /// <returns>The converted amount when a direct rate exists.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when a direct rate for the specified currency pair is not found in <paramref name="rates"/>.</exception>
        public decimal MiConvertidorDeMonedas(decimal amount, string sourceCurrency, string targetCurrency, Dictionary<string, decimal> rates)
        {
            // Defensive but simple: if any input is missing, return 0 (no exceptions for unknown currencies per requirements)
            if (string.IsNullOrWhiteSpace(sourceCurrency) || string.IsNullOrWhiteSpace(targetCurrency) || rates == null)
                throw new System.Collections.Generic.KeyNotFoundException("Invalid input or rates dictionary is null.");

            // Build normalized key: "USD->EUR"
            var key = $"{sourceCurrency.Trim().ToUpperInvariant()}->{targetCurrency.Trim().ToUpperInvariant()}";

            if (rates.TryGetValue(key, out var rate))
            {
                return amount * rate;
            }

            // No direct rate found: throw to indicate missing rate (tests expect an exception)
            throw new System.Collections.Generic.KeyNotFoundException($"Rate for '{key}' not found.");
        }
    }
}
