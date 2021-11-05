using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace ParkingManagement.Infrastructure
{
    public static class SqlDataAccess
    {
        public static object ExecuteScalar(string sqlQuery, string connectionString)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sqlQuery;
                connection.Open();

                return command.ExecuteScalar();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("There was a SQL error", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        public static List<T> Read<T>(string sqlQuery, string connectionString)
        {
            List<T> result = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sqlQuery;

                    connection.Open();
                    using SqlDataReader reader = command.ExecuteReader();
                    var properties = typeof(T).GetProperties();

                    while (reader.Read())
                    {
                        var element = Activator.CreateInstance<T>();

                        foreach (PropertyInfo property in properties)
                        {
                            try
                            {
                                var value = reader[property.Name];
                                if (value.GetType() != typeof(DBNull)) property.SetValue(element, value, null);
                            }
                            catch (Exception ex)
                            {
                                ThrowQueryException(ex, sqlQuery);
                            }
                        }
                        result.Add(element);
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception("There was a SQL error", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error", ex);
                }
                finally
                {
                    connection.Dispose();
                }
            }
            return result;
        }

        public static void ExecuteNonQuery(string sqlQuery, string connectionString)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sqlQuery;
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("There was a SQL error", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        public static void Insert<T>(T obj, string tableName, string connectionString)
        {
            string sqlQuery = CreateInsertQuery(obj, tableName);

            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sqlQuery;
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("There was a SQL error", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        public static void Update<T>(T obj, string tableName, string where, string connectionString)
        {
            string sqlQuery = CreateUpdateQuery(obj, where, tableName);

            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sqlQuery;
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("There was a SQL error", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        public static List<T> ReadWithProcedure<T>(string storedProcedureName, string connectionString, SqlParameter[] parameters = default(SqlParameter[]))
        {
            List<T> result = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    connection.Open();
                    using SqlDataReader reader = command.ExecuteReader();
                    var properties = typeof(T).GetProperties();

                    while (reader.Read())
                    {
                        var element = Activator.CreateInstance<T>();

                        foreach (PropertyInfo property in properties)
                        {
                            try
                            {
                                var value = reader[property.Name];
                                if (value.GetType() != typeof(DBNull)) property.SetValue(element, value, null);
                            }
                            catch (Exception ex)
                            {
                                ThrowProcedureException(ex, storedProcedureName);
                            }
                        }
                        result.Add(element);
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception("There was a SQL error: {0}", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error: {0}", ex);
                }
                finally
                {
                    connection.Dispose();
                }
            }
            return result;
        }

        public static void ExecuteNonQueryWithProcedure(string storedProcedureName, string connectionString, SqlParameter[] parameters = default(SqlParameter[]))
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = storedProcedureName;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("There was a SQL error", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        public static SqlParameter CreateSQLParameter(string name, object value)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name ?? throw new ArgumentNullException("name");
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        public static List<SqlParameter> CreateSQLParameterList(object obj)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = prop.Name,
                    Value = prop.GetValue(obj) ?? DBNull.Value
                };
                parameterList.Add(parameter);
            }
            return parameterList;
        }

        public static string CreateWhereClause(string name, string value)
        {
            return string.Format("WHERE [{0}] = {1}", name, value);
        }

        private static string CreateInsertQuery(object obj, string tableName = null)
        {
            StringBuilder query = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder propertie = new StringBuilder();
            Type type = obj.GetType();
            query.AppendFormat("INSERT INTO [{0}]", tableName ?? type.Name);
            values.Append("VALUES(");
            propertie.Append('(');
            PropertyInfo[] properties = type.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo prop = properties[i];

                propertie.AppendFormat("[{0}]", prop.Name);
                values.AppendFormat("{0}", SqlValueConverter.ToSqlValue(prop.GetValue(obj), prop.PropertyType));

                if (i < properties.Length - 1)
                {
                    propertie.Append(", ");
                    values.Append(", ");
                }
            }
            propertie.Append(')');
            values.Append(')');

            return query.AppendFormat("{0} {1}", propertie.ToString(), values.ToString()).ToString();
        }

        private static string CreateUpdateQuery(object obj, string where, string tableName = null)
        {
            StringBuilder query = new StringBuilder();
            StringBuilder values = new StringBuilder();

            Type type = obj.GetType();
            query.AppendFormat("UPDATE [{0}]", tableName ?? type.Name);
            values.Append("SET");
            PropertyInfo[] properties = type.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo prop = properties[i];

                values.AppendFormat(" [{0}] = {1}", prop.Name, SqlValueConverter.ToSqlValue(prop.GetValue(obj), prop.PropertyType));

                if (i < properties.Length - 1)
                {
                    values.Append(',');
                }
            }
            return query.AppendFormat(" {0} {1}", values.ToString(), where).ToString();
        }

        private static Exception ThrowProcedureException(Exception ex, string storedProcedureName)
        {
            return new Exception("Error while executing Procedure : " + storedProcedureName, ex);
        }

        private static Exception ThrowQueryException(Exception ex, string sql)
        {
            return new Exception("Error while executing query : " + sql, ex);
        }
    }
}
