using System;
using System.Collections.Generic;
using ConvertidorMonedas.Core;
using Xunit;

namespace ConvertidorMonedas.Tests
{
    public class BasicConversionTests
    {
        [Fact]
        public void Convert_100_USD_To_EUR_Returns_Expected()
        {
            // Arrange
            var rates = new Dictionary<string, decimal>
            {
                ["USD->EUR"] = 0.93m
            };
            var converter = new Convertidor();

            // Act
            var result = converter.MiConvertidorDeMonedas(100m, "USD", "EUR", rates);

            // Assert
            Assert.Equal(93.0m, result);
        }

        [Fact]
        public void MissingRate_Throws_KeyNotFoundException()
        {
            // Arrange
            var rates = new Dictionary<string, decimal>();
            var converter = new Convertidor();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                converter.MiConvertidorDeMonedas(50m, "USD", "JPY", rates)
            );
        }
    }
}
