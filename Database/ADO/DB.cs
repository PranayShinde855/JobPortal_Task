using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.ADO
{
    public static class DB<T> where T : class
    {
        public static string source = "Data Source=DESKTOP-HKULI1B;Initial Catalog=JobPortalDB;user id=sa;password=spark";

        public async static Task<int> ExecuteData(string str)
        {
            int rows = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(source))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(str, con);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rows;
        }

        public async static Task<int> ExecuteData(string str, params IDataParameter[] parameters)
        {
            int rows = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(source))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(str, con);
                    if (parameters != null)
                    {
                        foreach (IDataParameter p in parameters)
                        {
                            cmd.Parameters.Add(p);
                        }
                        rows = cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rows;
        }

        public async static Task<T> GetSingleRecord(string sql)
        {
            var tcs = new TaskCompletionSource<List<T>>();
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(source))
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                    }
                    con.Close();
                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(ds.Tables[0]);
                    IEnumerable<T> list = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<T>>(JSONString));
                    return list.FirstOrDefault();
                }
            }
        }

        public async static Task<IEnumerable<T>> GetList(string sql)
        {
            var tcs = new TaskCompletionSource<List<T>>();
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(source))
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                    }
                    con.Close();
                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(ds.Tables[0]);
                    IEnumerable<T> list = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<T>>(JSONString));
                    return list;
                }
            }
        }
    }
}
