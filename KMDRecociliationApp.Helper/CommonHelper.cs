using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using System.Data;

namespace KMDRecociliationApp.API.Common
{
    public class CommonHelper
    {
        public const string RetireeRoleName = "Retiree";
        public const string LogFolderPath = "C:\\logs\\Api";
        public const string KMDSECRETAPIKEY = "SecretAPIKey";
        public static DataTable ConvertListToDataTable<T>(List<T> dataList)
        {
            DataTable dataTable = new DataTable();

            // Get the properties of the object type
            var properties = typeof(T).GetProperties();

            // Create columns in DataTable based on the properties of the object
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            // Populate the DataTable with data from the list
            foreach (var item in dataList)
            {
                DataRow row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
       
    }
}
