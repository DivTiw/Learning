using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.Utility
{
    public static class ComLibCommon
    {
        static ComLibCommon()
        {
            //Mapper.Initialize(cfg => cfg.CreateMap<ProjectMaster, ProjectDM>());           
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProjectMaster, ProjectDM>();
                cfg.CreateMap<WorkflowMaster, WorkflowDM>().ForMember(x => x.module_count, opt => opt.Ignore());
                cfg.CreateMap<GroupMaster, GroupDM>();
            });


        }

        public static int ProjectSyscode { get; private set; } = Convert.ToInt32(ConfigurationManager.AppSettings["TokenProjectSysCode"]);
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        /// <summary>
        /// Casts the boolean value of the variables to 'Yes' or 'No' strings to be utilised in the views.
        /// </summary>
        /// <param name="_boolValue">value to be casted</param>
        /// <returns>string value as 'Yes' or 'No'</returns>
        public static string CastToYesNo(this bool _boolValue)
        {
            var strYesNo = string.Empty;
            if (_boolValue)
            {
                strYesNo = "Yes";
            }
            else
            {
                strYesNo = "No";
            }
            return strYesNo;
        }

        public static string ChangeToLower(this bool? _boolValue)
        {
            return _boolValue == null ? null : _boolValue.Value ? "true" : "false";
        }      

        public static string GetDurationFromHours(TimeSpan timeDiff)
        {
            string sDuration = string.Empty;
            if (timeDiff != null && (timeDiff.TotalHours > 0 || timeDiff.Minutes > 0))
            {
                int totHours, totAbsDays=0, remainingAbsHrs=0;
                if (timeDiff.TotalHours > 0)
                {
                    int ofcHrsInDay = 8;
                    totHours = (int)Math.Floor(timeDiff.TotalHours);
                    totAbsDays = totHours / ofcHrsInDay;
                    remainingAbsHrs = totHours % ofcHrsInDay;
                }
                int totAbsMins = timeDiff.Minutes;

                sDuration = $"{totAbsDays}Days {remainingAbsHrs}Hrs {totAbsMins}Mins";
            }            

            return sDuration;
        }

        public static TDestination Map<TSource, TDestination>(this object source) where TSource : class
        {
            var destDTO = Mapper.Map<TDestination>(source);
            return destDTO;
        }

        public static List<SelectItemDTO> ExtractDDLDataForKey(this IDictionary<DBTableNameEnums, List<SelectItemDTO>> dicData, DBTableNameEnums ddlKey)
        {
            if (dicData == null || dicData.Count <= 0) return new List<SelectItemDTO>();//throw new Exception("DDL Data dictionary is null or empty. Please make sure should get data flag was set to true.");
            if (!dicData.ContainsKey(ddlKey)) return new List<SelectItemDTO>();//throw new Exception("Given entity not found in the DDL Data dictionary. Please check if the request was made for this entity in View Model or Domain Model.");
            return dicData[ddlKey];
        }

        public static SelectList ExtractDDLDataAsSelectList(this IDictionary<DBTableNameEnums, List<SelectItemDTO>> dicData, DBTableNameEnums ddlKey, object selectedValue = null)
        {
            SelectList sl;
            List<SelectItemDTO> lstItem = ExtractDDLDataForKey(dicData, ddlKey);
            if (lstItem == null)
            {
                lstItem = new List<SelectItemDTO>();
            }
            if (selectedValue != null)
            {
                sl = new SelectList(lstItem, "Value", "Text", selectedValue);
            }
            else
            {
                sl = new SelectList(lstItem, "Value", "Text");
            }
            return sl;
        }

        public static Exception ReturnActualException(this Exception ex)
        {
            while (ex!= null && ex.InnerException != null) ex = ex.InnerException ;
            
            return ex;
        }

        public static bool CheckDuplicateUser(int[] arrRead, int[] arrWrite)
        {
            bool isDuplicate = false;
            if (arrRead != null && arrWrite != null)
            {
                foreach (var arrItem in arrRead)
                {
                    isDuplicate = Array.Exists(arrWrite, element => element == arrItem);
                    if (isDuplicate)
                    { break; }
                }
            }
            return isDuplicate;
        }

        public static DataTable IEnumerableToDataTable<TSource>(IEnumerable<TSource> source)
        {
            var props = typeof(TSource).GetProperties();

            var dt = new DataTable();
            dt.Columns.AddRange(
              props.Select(p => new DataColumn(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)).ToArray()
            );


            source.ToList().ForEach(
              i => dt.Rows.Add(props.Select(p => p.GetValue(i, null)).ToArray())
            );

            return dt;
        }

        public static DataTable ListToDataTable<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        public static void SetColumnsOrder(this DataTable dtbl, params String[] columnNames)
        {
            List<string> listColNames = columnNames.ToList();

            //Remove invalid column names.
            foreach (string colName in columnNames)
            {
                if (!dtbl.Columns.Contains(colName))
                {
                    listColNames.Remove(colName);
                }
            }

            foreach (string colName in listColNames)
            {
                dtbl.Columns[colName].SetOrdinal(listColNames.IndexOf(colName));
            }
        }
        public static void RemoveColsSetOrder(this DataTable dtbl, params String[] columnNames)
        {
            string[] arrCols = dtbl.Columns.Cast<DataColumn>().Select(x=> x.ColumnName).ToArray();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            int i = 0;
            foreach (string colName in columnNames)
            {
                dtbl.Columns[colName].SetOrdinal(i);
                dtbl.Columns[colName].ColumnName = textInfo.ToTitleCase(colName.Replace('_', ' ').ToLower());
                i++;
            }

            for (int j = i; j < arrCols.Length; j++)
            {
                dtbl.Columns.RemoveAt(i);
            }                     
        }
    }
}
