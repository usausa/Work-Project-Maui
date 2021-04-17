namespace ViewSample.FormsApp.Services
{
    using System;
    using System.Text;

    using Smart.Data.Mapper;

    public static class BuildHelper
    {
        public static void AndNameCondition(
            StringBuilder condition,
            DynamicParameter parameters,
            string column,
            string name,
            object value,
            string op = "=")
        {
            condition.Append(" AND ");
            condition.Append(column);
            condition.Append(' ');
            condition.Append(op);
            condition.Append(" @");
            condition.Append(name);
            parameters.Add(name, value);
        }

        public static void AndStringCondition(
            StringBuilder condition,
            DynamicParameter parameters,
            string column,
            string value,
            string op = "=")
        {
            if (!String.IsNullOrEmpty(value))
            {
                condition.Append(" AND ");
                condition.Append(column);
                condition.Append(' ');
                condition.Append(op);
                condition.Append(" @");
                condition.Append(column);
                parameters.Add(column, value);
            }
        }

        public static void AndStringNameCondition(
            StringBuilder condition,
            DynamicParameter parameters,
            string column,
            string name,
            string value,
            string op = "=")
        {
            if (!String.IsNullOrEmpty(value))
            {
                condition.Append(" AND ");
                condition.Append(column);
                condition.Append(' ');
                condition.Append(op);
                condition.Append(" @");
                condition.Append(name);
                parameters.Add(name, value);
            }
        }

        public static void AndNullableCondition<T>(
            StringBuilder condition,
            DynamicParameter parameters,
            string column,
            T? value,
            string op = "=")
            where T : struct
        {
            if (value.HasValue)
            {
                condition.Append(" AND ");
                condition.Append(column);
                condition.Append(' ');
                condition.Append(op);
                condition.Append(" @");
                condition.Append(column);
                parameters.Add(column, value);
            }
        }

        public static void AndNullableNameCondition<T>(
            StringBuilder condition,
            DynamicParameter parameters,
            string column,
            string name,
            T? value,
            string op = "=")
            where T : struct
        {
            if (value.HasValue)
            {
                condition.Append(" AND ");
                condition.Append(column);
                condition.Append(' ');
                condition.Append(op);
                condition.Append(" @");
                condition.Append(name);
                parameters.Add(name, value);
            }
        }

        public static void AndInStringsCondition(
            StringBuilder condition,
            DynamicParameter parameters,
            string name,
            string valueName,
            params string[] values)
        {
            condition.Append($" AND {name} IN (");

            var index = 1;
            foreach (var value in values)
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var parameterName = $"{valueName}{index}";

                    condition.Append($"@{parameterName}, ");
                    parameters.Add(parameterName, value);

                    index++;
                }
            }

            if (index == 1)
            {
                condition.Length -= 10 + name.Length;
            }
            else
            {
                condition.Length -= 2;
                condition.Append(')');
            }
        }
    }
}
