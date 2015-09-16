using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iLium.Extensions
{
    public static class DataTableExtensions
    {

        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        /// <example>DataTable dtTable = GetEmployeeDataTable(); 
        /// List<Employee> employeeList = dtTable.DataTableToList<Employee>();</example>
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a DataTable to CSV
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="colSeparator">Separator Column, Default (',')</param>
        /// <param name="withHeaders">Include Headers</param>
        /// <returns>List with generic objects</returns>
        /// <example>DataTable dtTable = GetEmployeeDataTable(); 
        /// string employeeCSV = dtTable.DataTableToList<Employee>();</example>
        public static string DataTableToCsv(this DataTable table, string colSeparator, bool withHeaders, string dateFormat)
        {
            try
            {
                if (String.IsNullOrEmpty(colSeparator)) { colSeparator = ","; }
                if (String.IsNullOrEmpty(dateFormat)) { dateFormat = "dd/MM/yyyy HH:mm:ss"; }

                var sb = new StringBuilder();

                if (withHeaders)
                {
                    IEnumerable<String> headers = table.Columns
                                                            .OfType<DataColumn>()
                                                            .Select(column => String.Format("\"{0}\"", column.ColumnName));

                    sb.AppendLine(String.Join(colSeparator, headers));
                }

                foreach (DataRow row in table.AsEnumerable())
                {
                    ICollection<string> sRow = new List<string>();

                    foreach (DataColumn col in table.Columns)
                    {
                        try
                        {   if (row[col] != null)
                            {
                                switch (col.DataType.Name)
                                {
                                    case "Int32":
                                    case "Int64":
                                        sRow.Add(row[col].ToString());
                                        break;

                                    case "DateTime":
                                        sRow.Add(String.Format("\"{0}\"", ((DateTime)row[col]).ToString(dateFormat)));
                                        break;

                                    default:
                                        sRow.Add(String.Format("\"{0}\"", row[col].ToString()));
                                        break;
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    sb.AppendLine(String.Join(colSeparator, sRow));
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a DataTable to CSV
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="colSeparator">Separator Column, Default (',')</param>
        /// <param name="withHeaders">Include Headers</param>
        /// <param name="dateFormat">Include Headers</param>
        /// <returns>List with generic objects</returns>
        /// <example>DataTable dtTable = GetEmployeeDataTable(); 
        /// string employeeCSV = dtTable.DataTableToList<Employee>();</example>
        public static MemoryStream DataTableToStreamCsv(this DataTable table, string colSeparator, bool withHeaders, string dateFormat)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

                string dataString = DataTableToCsv(table, colSeparator, withHeaders, dateFormat);

                writer.Write(dataString);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
            catch
            {
                return null;
            }
        }
    }
}
