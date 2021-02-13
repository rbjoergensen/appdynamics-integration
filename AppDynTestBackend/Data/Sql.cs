using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using AppDynTestBackend.Helpers;
using AppDynTestBackend.Models;
using Newtonsoft.Json;

namespace AppDynTestBackend.Data
{
    public class Sql
    {
        public static string connectionString = "Data Source=appdyntest-sqlserver;Persist Security Info=True;User ID=sa;Password=yourStrong(!)Password;Connect Timeout=30";

        public static void SetupInfrastructure()
        {
            bool isSetup = false;

            while (!isSetup)
            {
                try
                {
                    Logging.log.Information("Setting up database and tables");

                    SqlConnection con = new SqlConnection(connectionString);

                    var query = File.ReadAllText(@"Data/infrastructure.sql");

                    SqlCommand cmdDatabase = new SqlCommand(
                        "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'AppdynamicsTest') DROP DATABASE AppdynamicsTest; CREATE DATABASE AppdynamicsTest", con);
                    SqlCommand cmdTable = new SqlCommand(query, con);

                    con.Open();

                    cmdDatabase.ExecuteNonQuery();
                    cmdTable.ExecuteNonQuery();

                    isSetup = true;
                }
                catch (Exception ex)
                {
                    Logging.log.Error($"Error setting up database and tables: {ex.Message}");
                }

                Thread.Sleep(2000);
            }
        }
        public static string[] GetWeatherTypes()
        {
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                var query = "SELECT WeatherType FROM [AppdynamicsTest].[dbo].[WeatherTypes]";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                List<TestData> typeList = new List<TestData>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TestData d = new TestData
                        {
                            WeatherType = Convert.ToString(dr["WeatherType"])
                        };
                        typeList.Add(d);
                    }

                    con.Dispose();
                    con.Close();

                    return typeList.Select(s => s.WeatherType).ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logging.log.Error($"Error getting WeatherTypes: {ex.Message}");
                return null;
            }
        }
        public static Status AddWeatherType(TestData body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);

                SqlConnection con = new SqlConnection(connectionString);
                string query = $"INSERT [AppdynamicsTest].[dbo].[WeatherTypes] (WeatherType) VALUES('{body.WeatherType}')";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                var response = cmd.ExecuteNonQuery();

                con.Dispose();
                con.Close();

                Logging.log.Information(response.ToString());

                Status status = new Status
                {
                    Result = true,
                    Message = $"WeatherType added successfully"
                };
                return status;
            }
            catch (Exception ex)
            {
                Logging.log.Error($"Error creating new alert: {ex.Message}");

                Status status = new Status
                {
                    Result = false,
                    Message = $"Error creating new alert: {ex.Message}"
                };

                return status;
            }

        }
    }
}
