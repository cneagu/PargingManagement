using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingManagement.Infrastructure
{
    public static class SqlValueConverter
    {
        public static string EscapedString(this string value)
        {
            return value.Replace("'", "''");
        }

        public static string ToSqlValue(this string value)
        {
            return value == null ? "null" : string.Concat("N'", value.EscapedString(), "'");
        }

        public static string ToSqlValue(this bool? value)
        {
            return value.HasValue ? value.Value.ToSqlValue() : "null";
        }

        public static string ToSqlValue(this bool value)
        {
            return value ? "1" : "0";
        }

        public static string ToSqlValue(this DateTime? value)
        {
            return value.HasValue ? value.Value.ToSqlValue() : "null";
        }

        public static string ToSqlValue(this DateTime value)
        {
            return value == DateTime.MinValue ? "null" : string.Concat("'", value.ToString("yyyy-MM-dd hh:MM:ss"), "'");
        }

        public static string ToSqlValue(this Guid? value)
        {
            return value.HasValue ? value.Value.ToSqlValue() : "null";
        }

        public static string ToSqlValue(this Guid value)
        {
            return string.Concat("N'", value.ToString(), "'");
        }

        public static string ToSqlValue(this int? value)
        {
            return value.HasValue ? value.Value.ToString() : "null";
        }
        public static string ToSqlValue(this int value)
        {
            return value.ToString();
        }

        public static string ToSqlValue(this decimal? value)
        {
            return value.HasValue ? value.Value.ToString() : "null";
        }
        public static string ToSqlValue(this decimal value)
        {
            return  value.ToString();
        }

        public static string ToSqlValue(this byte[] value)
        {
            return value == null ? "null" : string.Concat("0x", BitConverter.ToString(value).Replace("-", string.Empty));
        }

        public static string ToSqlValue(this object value, Type type)
        {
            if (type == typeof(string))
            {
                return ToSqlValue((string)value);
            }
            if (type.IsEnum)
            {
                return ToSqlValue((int)value);
            }
            if (type == typeof(bool?))
            {
                return ToSqlValue((bool?)value);
            }
            if (type == typeof(bool))
            {
                return ToSqlValue((bool)value);
            }
            if (type == typeof(DateTime?))
            {
                return ToSqlValue((DateTime?)value);
            }
            if (type == typeof(DateTime))
            {
                return ToSqlValue((DateTime)value);
            }
            if (type == typeof(Guid?))
            {
                return ToSqlValue((Guid?)value);
            }
            if (type == typeof(Guid))
            {
                return ToSqlValue((Guid)value);
            }
            if (type == typeof(int))
            {
                return ToSqlValue((int)value);
            }
            if (type == typeof(int?))
            {
                return ToSqlValue((int?)value);
            }
            if (type == typeof(decimal))
            {
                return ToSqlValue((decimal)value);
            }
            if (type == typeof(decimal?))
            {
                return ToSqlValue((decimal?)value);
            }
            if (type == typeof(byte[]))
            {
                return ToSqlValue((byte[])value);
            }
            return "null";
        }

        public static string ToSqlInClause(this IEnumerable<Guid> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items", "cannot generate a valid SQL IN clause with a null item list");
            }

            if (!items.Any())
            {
                throw new ArgumentOutOfRangeException("items", "cannot generate a valid SQL IN clause with an empty item list");
            }

            return string.Join(", ", items.Select(x => x.ToSqlValue()));
        }
    }
}
