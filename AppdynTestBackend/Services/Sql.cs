using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDynTestBackend.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AppDynTestBackend
{
    public class Sql : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private static ILogger<Sql> _logger;
        private Sql _sql;
        
        public Sql(IServiceProvider serviceProvider, ILogger<Sql> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            _sql = scope.ServiceProvider.GetRequiredService<Sql>();

            SetupInfrastructure();

            return Task.CompletedTask;
        }

        private const string ConnectionString = "Data Source=appdyntest-sqlserver;Persist Security Info=True;User ID=sa;Password=yourStrong(!)Password;Connect Timeout=30";

        private static void SetupInfrastructure()
        {
            var isSetup = false;

            while (!isSetup)
            {
                try
                {
                    _logger.LogInformation("Setting up database and tables");

                    var con = new SqlConnection(ConnectionString);

                    var query = File.ReadAllText(@"Data/infrastructure.sql");

                    var cmdDatabase = new SqlCommand(
                        "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'AppdynamicsTest') DROP DATABASE AppdynamicsTest; CREATE DATABASE AppdynamicsTest", con);
                    var cmdTable = new SqlCommand(query, con);

                    con.Open();

                    cmdDatabase.ExecuteNonQuery();
                    cmdTable.ExecuteNonQuery();

                    isSetup = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error setting up database and tables: {ex.Message}");
                }

                Thread.Sleep(2000);
            }
        }
        public static string[] GetWeatherTypes()
        {
            try
            {
                var con = new SqlConnection(ConnectionString);
                const string query = "SELECT WeatherType FROM [AppdynamicsTest].[dbo].[WeatherTypes]";
                var cmd = new SqlCommand(query, con);

                con.Open();
                var dr = cmd.ExecuteReader();

                var typeList = new List<TestData>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var d = new TestData
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
                _logger.LogError($"Error getting WeatherTypes: {ex.Message}");
                return null;
            }
        }
        public static Status AddWeatherType(TestData body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);

                var con = new SqlConnection(ConnectionString);
                var query = $"INSERT [AppdynamicsTest].[dbo].[WeatherTypes] (WeatherType) VALUES('{body.WeatherType}')";

                var cmd = new SqlCommand(query, con);
                con.Open();

                var response = cmd.ExecuteNonQuery();

                con.Dispose();
                con.Close();

                _logger.LogInformation(response.ToString());

                var status = new Status
                {
                    Result = true,
                    Message = $"WeatherType {body.WeatherType} added successfully"
                };
                
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating new alert: {ex.Message}");

                var status = new Status
                {
                    Result = false,
                    Message = $"Error creating new alert: {ex.Message}"
                };

                return status;
            }

        }
    }
}
