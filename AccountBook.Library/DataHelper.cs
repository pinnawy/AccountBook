using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;

namespace AccountBook.Library
{
    public static class DataHelper
    {
        /// <summary>
        /// IList泛型集合转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(this IList<T> list)
        {
            return ConvertToDataSet(list).Tables[0];
        }

        /// <summary>
        /// IList泛型集合转换成 DataView
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataView ConvertToDataView<T>(this IList<T> list)
        {
            return ConvertToDataSet(list).Tables[0].DefaultView;
        }

        /// <summary>
        /// IList泛型集合转换成 DataSet
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataSet ConvertToDataSet<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            DataSet ds = new DataSet();
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (T t in list)
            {
                if (t == null)
                {
                    continue;
                }

                row = dt.NewRow();

                int properyLength = myPropertyInfo.Length;
                for (int i = 0; i < properyLength; i++)
                {
                    PropertyInfo pi = myPropertyInfo[i];

                    string name = pi.Name;

                    if (dt.Columns[name] == null)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>   
        /// DataSet装换为泛型集合   
        /// </summary>   
        /// <param name="pDataSet">DataSet</param>   
        /// <param name="pTableIndex">待转换数据表索引</param>   
        /// <param name="ignoreCase">忽略T对象属性名和DataTable列名大小写进行匹配</param>   
        /// <returns>泛型集合</returns>   
        public static IList<T> DataSetToIList<T>(this DataSet pDataSet, int pTableIndex, bool ignoreCase)
        {
            if (pDataSet == null || pDataSet.Tables.Count < 0)
                return null;

            if (pTableIndex > pDataSet.Tables.Count - 1)
                return null;

            if (pTableIndex < 0)
                pTableIndex = 0;

            DataTable pData = pDataSet.Tables[pTableIndex];
            StringComparison sc = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;

            // 返回值初始化   
            IList<T> result = new List<T>();
            for (int j = 0; j < pData.Rows.Count; j++)
            {
                T t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < pData.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值   
                        if (pi.Name.Equals(pData.Columns[i].ColumnName, sc))
                        {
                            try
                            {
                                // 数据库NULL值单独处理   
                                pi.SetValue(t, pData.Rows[j][i] != DBNull.Value ? pData.Rows[j][i] : null, null);
                            }
                            catch (Exception)
                            {
                                //Guid赋值给string类型属性单独处理
                                if (pData.Rows[j][i].GetType() == typeof(Guid))
                                {
                                    pi.SetValue(t, pData.Rows[j][i] != DBNull.Value ? pData.Rows[j][i].ToString() : null, null);
                                }

                                //double赋值给float类型属性单独处理
                                if (pData.Rows[j][i].GetType() == typeof(double))
                                {
                                    pi.SetValue(t, pData.Rows[j][i] != DBNull.Value ? (object)float.Parse(pData.Rows[j][i].ToString()) : null, null);
                                }

                                //long赋值给int类型属性单独处理
                                if (pData.Rows[j][i].GetType() == typeof(long))
                                {
                                    pi.SetValue(t, pData.Rows[j][i] != DBNull.Value ? (object)int.Parse(pData.Rows[j][i].ToString()) : null, null);
                                }

                                //string赋值给DateTime类型属性单独处理
                                if (pi.PropertyType == typeof(DateTime) && pData.Rows[j][i].GetType() == typeof(string))
                                {
                                    pi.SetValue(t, pData.Rows[j][i] != DBNull.Value ? (object)DateTime.Parse(pData.Rows[j][i].ToString()) : null, null);
                                }
                            }

                            break;
                        }
                    }
                }
                result.Add(t);
            }

            return result;
        }
    }
}
