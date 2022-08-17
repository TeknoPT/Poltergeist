﻿using System;

namespace Poltergeist.PhantasmaLegacy.Numerics
{
    public static class UnitConversion
    {
        // TODO why not just BigInteger.Pow(10, units)???
        private static BigInteger GetMultiplier(int units)
        {
            BigInteger unitMultiplier = 1;
            while (units > 0)
            {
                unitMultiplier *= 10;
                units--;
            }

            return unitMultiplier;
        }

        public static decimal ToDecimal(BigInteger value, int units)
        {
            if (value == 0)
            {
                return 0;
            }

            if (units == 0)
            {
                return (long)value;
            }

            var multiplier = (decimal)GetMultiplier(units);
            var n = decimal.Parse(value.ToString()); // TODO not very efficient, improve later...
            n /= multiplier;
            return n;
        }

        public static decimal ToDecimal(string value, int units)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            BigInteger big = BigInteger.Parse(value);

            return ToDecimal(big, units);
        }

        public static BigInteger ToBigInteger(decimal n, int units)
        {
            var multiplier = GetMultiplier(units);
            var A = new BigInteger((long)n);
            var B = new BigInteger((long)multiplier);

            var fracPart = n - Math.Truncate(n);
            BigInteger C = 0;

            if (fracPart > 0)
            {
                var l = fracPart * (long)multiplier;
                C = new BigInteger((long)l);
            }

            return A * B + C;
        }

        public static BigInteger GetUnitValue(int decimals)
        {
            return ToBigInteger(1, decimals);
        }
    }
}
