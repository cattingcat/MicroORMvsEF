using System;
using System.Text;

namespace MyOrm
{
    static internal class OrmMapExtension
    {
        public static string GetColumnsNamesList(this OrmMap map)
        {
            StringBuilder b = new StringBuilder();
            foreach (string col in map.Columns)
            {
                b.AppendFormat("{0}.{1}", map.TableInfo.DbTableName, col);
                b.Append(',');
            }
            b.Remove(b.Length - 1, 1);
            return b.ToString();
        }


        public static string BuildSelectAllQueryLazy(this OrmMap map)
        {
            string selectTemplate = "SELECT {0} FROM {1}";
            string argCols = GetColumnsNamesList(map);
            return String.Format(selectTemplate, argCols, map.TableInfo.DbTableName);
        }

        public static string BuildSelectAllQuery(this OrmMap map, string whereSection = "")
        {
            IMappingPool pool = map.Pool;

            // 0 - arg list
            // 1 - table
            // 2 - JOIN-s
            // 3 - WHERE
            // 4 - order by
            string selectTemplate = "SELECT {0} FROM {1} {2} {3} {4}";

            StringBuilder joins = new StringBuilder();

            StringBuilder columnNames = new StringBuilder();
            columnNames.Append(map.GetColumnsNamesList()); 

            foreach (var item in map.ManyRelations)
            {
                OrmMap subMap = pool.GetMap(item.SecondTable);
                columnNames.Append(',');
                columnNames.Append(subMap.GetColumnsNamesList());

                joins.Append(" LEFT OUTER JOIN ");
                joins.Append(item.SecondTable);
                joins.Append(" ON ");
                joins.AppendFormat("{0}.{1} = {2}.{3} ", 
                    map.TableInfo.DbTableName, map.Id.DbColumnName, item.SecondTable, item.ForeignKey);
            }

            string argCols = columnNames.ToString();

            string orderBy = String.Format(" ORDER BY {0}.{1}", map.TableInfo.DbTableName, map.Id.DbColumnName);


            string where = whereSection == string.Empty ? string.Empty : String.Format(" WHERE {0} ", whereSection);

            return String.Format(selectTemplate, argCols, map.TableInfo.DbTableName, joins.ToString(), where, orderBy);
        }


        public static string BuildSelectWhereQuery(this OrmMap map, string sqlWhereSection)
        {
            string selectTemplate = "SELECT {0} FROM {1} WHERE " + sqlWhereSection;
            string argCols = GetColumnsNamesList(map);
            return String.Format(selectTemplate, argCols, map.TableInfo.DbTableName);
        }

        public static string BuildInsertQuery(this OrmMap map, string argValues)
        {
            string insertTemplate = "INSERT INTO {0}({1}) VALUES(" + argValues + ")";
            string argCols = GetColumnsNamesList(map);
            return String.Format(insertTemplate, map.TableInfo.DbTableName, argCols);
        }

        public static string BuildDeleteQuery(this OrmMap map, string sqlWhereSection)
        {
            string deleteTemplate = "DELETE FROM {0} WHERE " + sqlWhereSection;
            return String.Format(deleteTemplate, map.TableInfo.DbTableName);
        }

        public static string BuildSubSelectQuery(this OrmMap map, string[] columns, string whereSection)
        { 
            StringBuilder builder = new StringBuilder();
            builder.Append("(SELECT ");
            foreach (string col in columns)
            {
                builder.Append(col);
                builder.Append(',');
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(" FROM ");
            builder.Append(map.TableInfo.DbTableName);
            builder.Append(" WHERE (");
            builder.Append(whereSection);
            builder.Append(" ))");
            return builder.ToString();
        }

        public static string BuildSubSelectQuery(this OrmMap map, string column, string whereSection)
        {
            return BuildSubSelectQuery(map, new string[] { column }, whereSection);
        }


        public static object GetId(this OrmMap map, object o)
        {
            return map.Id.PropertyInfo.GetValue(o);
        } 
    }
}
