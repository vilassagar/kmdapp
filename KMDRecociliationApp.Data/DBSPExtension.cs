using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data
{
    public static class DBSPExtension
    {

        #region mapped methods 
        //public static List<ContactDetails> GetContactDetailsList(this DataTable dataTable)
        //{
        //    List<ContactDetails> contactDetails = new List<ContactDetails>();
        //    foreach (DataRow item in dataTable.Rows)
        //    {
        //        ContactDetails contact = new ContactDetails();
        //        contact.ID = Convert.ToInt32(item[nameof(contact.ID)]);
        //        contact.FirstName = Convert.ToString(item[nameof(contact.FirstName)]);
        //        contact.Surname = Convert.ToString(item[nameof(contact.Surname)]);
        //        contact.Email = Convert.ToString(item[nameof(contact.Email)]);
        //        contact.ContactNumber = Convert.ToString(item[nameof(contact.ContactNumber)]);
        //        contact.Designation = Convert.ToString(item[nameof(contact.Designation)]);
        //        contact.ContactTypeName = Convert.ToString(item[nameof(contact.ContactTypeName)]);
        //        contact.PortfolioID = Convert.ToInt32(item[nameof(contact.PortfolioID)]);
        //        contact.LegalEntityID = Convert.ToInt32(item[nameof(contact.LegalEntityID)]);
        //        ContactType contactType = new ContactType();
        //        contactType.ContactTypeID = Convert.ToInt32(item[nameof(contactType.ContactTypeID)]);
        //        contactType.ContactTypeName = Convert.ToString(item[nameof(contactType.ContactTypeName)]);
        //        contact.ContactType = contactType;
        //        contactDetails.Add(contact);
        //    }
        //    return contactDetails;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Root Object </typeparam>
        /// <typeparam name="TList"> List Object Name</typeparam>
        /// <typeparam name="Page_Result"> Page result object</typeparam>
        /// <param name="dataSet"></param>
        /// <param name="compareColumnName"></param>
        /// <param name="includepageresult"></param>
        /// <param name="excludePropeties"></param>
        /// <param name="excludePropeties2"></param>
        /// <returns></returns>
        internal static Tuple<List<T>, Page_Result> MapDataSetToObjectList<T, Page_Result>(this DataSet dataSet, bool includepageresult = false, List<string> excludePropeties = null)
        {
            var objList = new List<T>();
            var props = excludePropeties != null ? typeof(T).GetProperties().Where(x => !excludePropeties.Any(p => p.ToLower() == x.Name.ToLower())) : typeof(T).GetProperties();
            var proppageresult = excludePropeties != null ? typeof(Page_Result).GetProperties().Where(x => !excludePropeties.Any(p => p.ToLower() == x.Name.ToLower())) : typeof(Page_Result).GetProperties();
            Page_Result pageResult = Activator.CreateInstance<Page_Result>();
            if (dataSet != null && dataSet.Tables.Count >= 1)
            {
                DataTable dataTable = dataSet.Tables[0];

                var colMapping = dataTable.Columns.Cast<DataColumn>()
                    .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                    .ToDictionary(key => key.ColumnName.ToLower());

                if (!dataTable.IsIsNullOrEmpty())
                {
                    foreach (DataRow item in dataTable.Rows)
                    {
                        T obj = Activator.CreateInstance<T>();
                        if (includepageresult == true)
                        {
                            foreach (var proppage in proppageresult)
                            {
                                if (item.Table.Columns.Contains(proppage.Name.ToLower()))
                                {
                                    var val = item[proppage.Name.ToLower()];
                                    proppage.SetValue(pageResult, val == DBNull.Value ? null : val);
                                }
                                else
                                {
                                    proppage.SetValue(pageResult, null);
                                }
                            }
                        }

                        foreach (var prop in props)
                        {
                            Type type = prop.PropertyType;


                            if (item.Table.Columns.Contains(prop.Name.ToLower()))
                            {
                                var val = item[prop.Name.ToLower()];
                                prop.SetValue(obj, val == DBNull.Value ? null : val);
                            }
                            else
                            {
                                prop.SetValue(obj, null);
                            }

                        }
                        objList.Add(obj);
                    }
                }
            }
            Page_Result page_Resultobj = pageResult;
            return Tuple.Create(objList, page_Resultobj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Root Object </typeparam>
        /// <typeparam name="TList"> List Object Name</typeparam>
        /// <typeparam name="Page_Result"> Page result object</typeparam>
        /// <param name="dataSet"></param>
        /// <param name="compareColumnName"></param>
        /// <param name="includepageresult"></param>
        /// <param name="excludePropeties"></param>
        /// <param name="excludePropeties2"></param>
        /// <returns></returns>
        internal static Tuple<List<T>, Page_Result> MapDataSetToObjectList<T, TList, Page_Result>(this DataSet dataSet, string compareColumnName, bool includepageresult = false, List<string> excludePropeties = null, List<string> excludePropeties2 = null)
        {
            var objList = new List<T>();
            var props = excludePropeties != null ? typeof(T).GetProperties().Where(x => !excludePropeties.Any(p => p.ToLower() == x.Name.ToLower())) : typeof(T).GetProperties();
            var props2 = excludePropeties2 != null ? typeof(TList).GetProperties().Where(x => !excludePropeties.Any(p => p.ToLower() == x.Name.ToLower())) : typeof(TList).GetProperties();
            var proppageresult = excludePropeties != null ? typeof(Page_Result).GetProperties().Where(x => !excludePropeties.Any(p => p.ToLower() == x.Name.ToLower())) : typeof(Page_Result).GetProperties();
            Page_Result pageResult = Activator.CreateInstance<Page_Result>();
            if (dataSet != null && dataSet.Tables.Count >= 1)
            {
                DataTable dataTable = dataSet.Tables[0];
                DataTable dataTable2 = dataSet.Tables[1];

                var colMapping = dataTable.Columns.Cast<DataColumn>()
                    .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                    .ToDictionary(key => key.ColumnName.ToLower());

                if (!dataTable.IsIsNullOrEmpty())
                {
                    foreach (DataRow item in dataTable.Rows)
                    {
                        T obj = Activator.CreateInstance<T>();
                        if (includepageresult == true)
                        {
                            foreach (var proppage in proppageresult)
                            {
                                if (item.Table.Columns.Contains(proppage.Name.ToLower()))
                                {
                                    var val = item[proppage.Name.ToLower()];
                                    proppage.SetValue(pageResult, val == DBNull.Value ? null : val);
                                }
                                else
                                {
                                    proppage.SetValue(pageResult, null);
                                }
                            }
                        }

                        foreach (var prop in props)
                        {
                            Type type = prop.PropertyType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                Type itemType = type.GetGenericArguments()[0];
                                List<TList> objlist2 = new List<TList>();
                                foreach (DataRow dataRow in dataTable2.AsEnumerable().Where(row => row.Field<int>(compareColumnName) == Convert.ToInt32(item[compareColumnName])))
                                {
                                    var obj2 = Activator.CreateInstance<TList>();
                                    var colMapping2 = dataTable2.Columns.Cast<DataColumn>()
                                           .Where(x => props2.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                                           .ToDictionary(key => key.ColumnName.ToLower());
                                    foreach (var prop2 in props2)
                                    {
                                        if (dataRow.Table.Columns.Contains(prop2.Name.ToLower()))
                                        {
                                            var val2 = dataRow[prop2.Name.ToLower()];
                                            prop2.SetValue(obj2, val2 == DBNull.Value ? null : val2);
                                        }
                                        else
                                        {
                                            prop2.SetValue(obj2, null);
                                        }

                                    }
                                    objlist2.Add(obj2);

                                }
                                prop.SetValue(obj, objlist2);

                            }
                            else
                            {
                                if (type.Name != "String" && type.BaseType.Name == "Object")
                                {
                                    int id = Convert.ToInt32(item[compareColumnName]);
                                    var objDetails = dataTable2.AsEnumerable().Where(row => row.Field<int>(compareColumnName) == id).FirstOrDefault();
                                    //DataRow objDetails = dataTable2.Rows.Cast<DataRow>().Where(x => x[compareColumnName] == item[compareColumnName]).FirstOrDefault();
                                    if (objDetails != null)
                                    {
                                        TList Tobj = Activator.CreateInstance<TList>();
                                        foreach (var prop2 in props2)
                                        {
                                            if (objDetails.Table.Columns.Contains(prop2.Name.ToLower()))
                                            {
                                                var val = objDetails[prop2.Name.ToLower()];
                                                prop2.SetValue(Tobj, val == DBNull.Value ? null : val);
                                            }
                                            else
                                            {
                                                prop2.SetValue(Tobj, null);
                                            }
                                        }
                                        prop.SetValue(obj, Tobj);
                                    }
                                }
                                else
                                if (item.Table.Columns.Contains(prop.Name.ToLower()))
                                {
                                    var val = item[prop.Name.ToLower()];
                                    prop.SetValue(obj, val == DBNull.Value ? null : val);
                                }
                                else
                                {
                                    prop.SetValue(obj, null);
                                }
                            }
                        }
                        objList.Add(obj);
                    }
                }
            }
            Page_Result page_Resultobj = pageResult;
            return Tuple.Create(objList, page_Resultobj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Required object mapp the data set data</typeparam>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        internal static IList<T> MapDataSetToObjectList<T>(this DataSet dataSet)
        {
            var objList = new List<T>();
            var props = typeof(T).GetProperties();
            DataTable dataTable = dataSet.Tables[0];
            var colMapping = dataTable.Columns.Cast<DataColumn>()
                    .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                    .ToDictionary(key => key.ColumnName.ToLower());
            if (!dataTable.IsIsNullOrEmpty())
            {
                foreach (DataRow item in dataTable.Rows)
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        if (item.Table.Columns.Contains(prop.Name.ToLower()))
                        {
                            var val = item[prop.Name.ToLower()];
                            prop.SetValue(obj, val == DBNull.Value ? null : val);
                        }
                        else
                        {
                            prop.SetValue(obj, null);
                        }
                    }
                    objList.Add(obj);

                }
            }


            return objList;
        }


        internal static List<T> MapDataTableToObjectList<T>(this DataTable dataTable)
        {
            var objList = new List<T>();
            var props = typeof(T).GetProperties();
            var colMapping = dataTable.Columns.Cast<DataColumn>()
                    .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                    .ToDictionary(key => key.ColumnName.ToLower());
            if (!dataTable.IsIsNullOrEmpty())
            {
                foreach (DataRow item in dataTable.Rows)
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        if (item.Table.Columns.Contains(prop.Name.ToLower()))
                        {
                            var val = item[prop.Name.ToLower()];
                            prop.SetValue(obj, val == DBNull.Value ? null : val);
                        }
                        else
                        {
                            prop.SetValue(obj, null);
                        }
                    }
                    objList.Add(obj);

                }
            }


            return objList;
        }
        internal static T MapDataTableToObject<T>(this DataTable dataTable, string filterColumn = null, string filtervalue = null)
        {
            var obj = Activator.CreateInstance<T>();
            if (dataTable.IsIsNullOrEmpty())
                return obj;

            var props = typeof(T).GetProperties();

            DataTable filteredTable = null;
            if (filterColumn != null && filtervalue != null)
            {
                filteredTable = (from n in dataTable.AsEnumerable()
                                 where !n.Field<string>(filterColumn).Contains(filtervalue)
                                 select n).CopyToDataTable();
                dataTable = filteredTable;
            }
            DataRow item = dataTable.Rows[0];

            foreach (var prop in props)
            {
                if (item.Table.Columns.Contains(prop.Name.ToLower()))
                {
                    var val = item[prop.Name.ToLower()];
                    prop.SetValue(obj, val == DBNull.Value ? null : val);
                }
                else
                    prop.SetValue(obj, null);
            }


            return obj;
        }

        internal static T MapDataSetWithChildObjectList<T, T1>(this DataSet dataSet)
        {
            var objList = new List<T1>();
            var obj = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties();
            var props1 = typeof(T1).GetProperties();



            if (dataSet != null && dataSet.Tables.Count >= 1)
            {
                DataTable dataTable = dataSet.Tables[0];
                DataTable dataTable1 = dataSet.Tables[1];




                var colMapping = dataTable.Columns.Cast<DataColumn>()
                    .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                    .ToDictionary(key => key.ColumnName.ToLower());



                if (!dataTable.IsIsNullOrEmpty())
                {
                    foreach (DataRow item in dataTable.Rows)
                    {
                        foreach (var prop in props)
                        {
                            Type type = prop.PropertyType;
                            if (type.Name != "String" && type.BaseType.Name == "Object")
                            {
                                foreach (DataRow item1 in dataTable1.Rows)
                                {
                                    var Tobj = Activator.CreateInstance<T1>();
                                    foreach (var prop1 in props1)
                                    {
                                        if (item1.Table.Columns.Contains(prop1.Name.ToLower()))
                                        {
                                            var val = item1[prop1.Name.ToLower()];
                                            prop1.SetValue(Tobj, val == DBNull.Value ? null : val);
                                        }
                                        else
                                        {
                                            prop1.SetValue(Tobj, null);
                                        }
                                    }
                                    objList.Add(Tobj);
                                }
                                prop.SetValue(obj, objList);
                            }
                            else if (item.Table.Columns.Contains(prop.Name.ToLower()))
                            {
                                var val = item[prop.Name.ToLower()];
                                prop.SetValue(obj, val == DBNull.Value ? null : val);
                            }
                            else
                            {
                                prop.SetValue(obj, null);
                            }
                        }



                    }
                }
            }



            return obj;
        }

        public static bool IsIsNullOrEmpty(this DataTable dataTable)
        {
            return (dataTable == null || dataTable.Rows.Count == 0);
        }
        public static bool IsIsNullOrEmpty(this DataSet dataSet)
        {
            return (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0);
        }
        public static bool TryParseString(this DataRow row, string fieldName, out string outValue)
        {
            outValue = "";
            if (!row.IsNull(fieldName))
            {

                if (row[fieldName] == DBNull.Value)
                {
                    return false;
                }
                outValue = Convert.ToString(row[fieldName]);
                return true;


            }

            return false;


        }

        #endregion

        public static DataTable DropBlankRows(this DataTable dt)
        {
            int cnt = 0;
            string Value = null;
            List<int> rowsToDelete = new List<int>();
            DataTable temp = dt.Copy();
            for (int j = 0; j < temp.Rows.Count; j++)
            {
                for (int i = 0; i < temp.Columns.Count; i++)
                {
                    Value = dt.Rows[j][i].ToString();
                    if ((string.IsNullOrEmpty(Value)) || (string.IsNullOrWhiteSpace(Value)))
                    {
                        cnt++;
                    }
                }
                if (cnt == dt.Columns.Count)
                {
                    //rowsToDelete.Add(j);
                    DataRow dr = temp.Rows[j];
                    temp.Rows.Remove(dr);
                    j = 0;
                }
                cnt = 0;
            }
            return temp;
        }

        public static bool IsValidAlphanumeric(this string input)
        {
            // Define the regular expression pattern
            string pattern = @"^[a-zA-Z0-9\\/_\-]+[a-zA-Z0-9\\/_\-\s]*$";

            // Create a Regex object with the pattern
            Regex regex = new Regex(pattern);

            // Use the IsMatch method to validate the input
            return regex.IsMatch(input);
        }

    }
}
