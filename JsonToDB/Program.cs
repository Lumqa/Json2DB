using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JsonToDB
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateDB gen = new GenerateDB();
            string tableName = "DataSet1";
            string fileName = "file.json";
            string fileName2 = "fill.json";
            string json = null;
            dynamic values = null;
            List<dynamic> res = new List<dynamic>();

            json = Read(fileName);
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            gen.Create(tableName, values); //Create table
            Console.WriteLine();

            json = Read(fileName2);
            values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            gen.AddData(tableName, values); //Insert into
            Console.WriteLine();

            res = gen.GetData(tableName); //Select

            Console.ReadKey();
        }
        static string Read(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                return r.ReadToEnd();
            }
        }
    }
}
