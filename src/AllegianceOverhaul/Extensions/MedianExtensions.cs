using System;
using System.Collections.Generic;
using System.Linq;

namespace AllegianceOverhaul.Extensions
{
#pragma warning disable CS8629 // Nullable value type may be null.
    public static class MedianExtensions
    {
        public static double Median(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int[] data = source.OrderBy(n => n).ToArray();
            if (data.Length == 0)
            {
                throw new InvalidOperationException();
            }
            return data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0 : data[data.Length / 2];
        }

        public static double? Median(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int[] data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
            return data.Length == 0
                ? null
                : (double?) (data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0 : data[data.Length / 2]);
        }

        public static double Median(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            long[] data = source.OrderBy(n => n).ToArray();
            if (data.Length == 0)
            {
                throw new InvalidOperationException();
            }
            return data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0 : data[data.Length / 2];
        }

        public static double? Median(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            long[] data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
            return data.Length == 0
                ? null
                : (double?) (data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0 : data[data.Length / 2]);
        }

        public static float Median(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            float[] data = source.OrderBy(n => n).ToArray();
            if (data.Length == 0)
            {
                throw new InvalidOperationException();
            }
            return data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0f : data[data.Length / 2];
        }

        public static float? Median(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            float[] data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
            return data.Length == 0
                ? null
                : (float?) (data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0f : data[data.Length / 2]);
        }

        public static double Median(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            double[] data = source.OrderBy(n => n).ToArray();
            if (data.Length == 0)
            {
                throw new InvalidOperationException();
            }
            return data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0 : data[data.Length / 2];
        }

        public static double? Median(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            double[] data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
            return data.Length == 0
                ? null
                : (double?) (data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0 : data[data.Length / 2]);
        }

        public static decimal Median(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            decimal[] data = source.OrderBy(n => n).ToArray();
            if (data.Length == 0)
            {
                throw new InvalidOperationException();
            }
            return data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0m : data[data.Length / 2];
        }

        public static decimal? Median(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            decimal[] data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
            return data.Length == 0
                ? null
                : (decimal?) (data.Length % 2 == 0 ? (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0m : data[data.Length / 2]);
        }

        public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.Select(selector).Median();
        }

        public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            return source.Select(selector).Median();
        }

        public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            return source.Select(selector).Median();
        }

        public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            return source.Select(selector).Median();
        }

        public static float Median<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            return source.Select(selector).Median();
        }

        public static float? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            return source.Select(selector).Median();
        }

        public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return source.Select(selector).Median();
        }

        public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            return source.Select(selector).Median();
        }

        public static decimal Median<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return source.Select(selector).Median();
        }

        public static decimal? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Median();
        }
    }
#pragma warning restore CS8629 // Nullable value type may be null.
}