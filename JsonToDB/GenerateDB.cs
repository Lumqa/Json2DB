using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Linq;
using Dapper;

namespace JsonToDB
{
    class GenerateDB
    {
        private string connectionString = "Server=(localdb)\\mssqllocaldb;Database=JsonDB;Trusted_Connection=True;";

        private dynamic ParseValue(string type)
        {
            switch (type)
            {
                case "string": return typeof(string);
                case "decimal": return typeof(decimal);
                case "int": return typeof(int);
                default: throw new ArgumentException("DataType is not supported");
            }
        }

        private string TypeWithParam(SqlDbType type)
        {
            int n = 30;
            int p = 5;
            int s = 2;
            string size = "(" + n + ")";
            string size2 = "(" + p + "," + s + ")";
            switch (type)    
            {
                case SqlDbType.NVarChar: return "NVarChar"+size;
                case SqlDbType.Decimal: return "Decimal" + size2;
                default : return type.ToString();
            }
        }

        public void Create(string name, Dictionary<string, string> fields)
        {

            StringBuilder sql = new StringBuilder();
            string pk = " PRIMARY KEY";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                
                sql.Append("IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = '" + name + "') ");
                sql.Append("CREATE TABLE " + name + " (");

                foreach (var i in fields)
                {
                    sql.Append(i.Key + " " + TypeWithParam(SqlHelper.GetDbType(Type.GetType(ParseValue(i.Value).ToString()))) + " NOT NULL" + pk + ",");
                    pk = null;
                }

                sql.Remove(sql.Length - 1, 1);
                sql.Append(")");

                Console.WriteLine(sql.ToString());
                connection.Execute(sql.ToString());

                Console.WriteLine("Done CREATE");
            }
        }

        public void AddData(string name, Dictionary<string, dynamic> fields)
        {

            StringBuilder sql = new StringBuilder();
            var dynamicParameters = new DynamicParameters();

            foreach(var i in fields)
            {
                dynamicParameters.Add("@"+i.Value, i.Value);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                sql.Append("INSERT INTO " + name + " (");
                foreach(var i in fields)
                {
                    sql.Append(i.Key + ",");
                }

                sql.Remove(sql.Length - 1, 1);
                sql.Append(") VALUES (");

                foreach (var i in fields)
                {
                    sql.Append("@" + i.Value + ",");
                }

                sql.Remove(sql.Length - 1, 1);
                sql.Append(")");

                Console.WriteLine(sql.ToString());
                connection.Execute(sql.ToString(), dynamicParameters);

                Console.WriteLine("Done INSERT");
            }
        }

        public List<dynamic> GetData(string name)
        {
            string sql = "SELECT * FROM " + name;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                List<dynamic> list = new List<dynamic>();
                list = connection.Query<dynamic>(sql).ToList();

                foreach(var i in list)
                {
                    //Console.WriteLine(i);
                    foreach (var k in i)
                    {
                        Console.Write(k.Value + " ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Done SELECT");
                return list;
            }
        }
    }
}
