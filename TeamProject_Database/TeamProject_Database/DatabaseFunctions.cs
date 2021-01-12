using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using TeamProject_Database.Models;
using System.Collections.Generic;

namespace TeamProject_Database
{
    public static class DatabaseFunctions
    {
        [FunctionName("GetLeaderBoardList")]
        public static async Task<IActionResult> GetLeaderBoardList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard")] HttpRequest req,
            ILogger log)
        {
            try
            {
                List<Trappenspel> leaderboard = new List<Trappenspel>();

                string connectionString = Environment.GetEnvironmentVariable("connectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandText = "SELECT * FROM tbLeaderboard ORDER BY score DESC";

                        SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            Trappenspel newLeaderboard = new Trappenspel();

                            newLeaderboard.PlayerID = int.Parse(reader["playerID"].ToString());
                            newLeaderboard.Playername = reader["playername"].ToString();
                            newLeaderboard.Score = int.Parse(reader["score"].ToString());
                            newLeaderboard.Difficulty = reader["difficulty"].ToString();
                            newLeaderboard.Date = DateTime.Parse(reader["date"].ToString());
                            newLeaderboard.Steps = int.Parse(reader["steps"].ToString());
                            leaderboard.Add(newLeaderboard);
                        }
                    }
                }
                return new ObjectResult(leaderboard);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("GetLeaderBoardDifficulty")]
        public static async Task<IActionResult> GetLeaderBoardDifficulty(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/{difficulty}")] HttpRequest req, string difficulty,
            ILogger log)
        {
            try
            {
                List<Trappenspel> leaderboard = new List<Trappenspel>();

                string connectionString = Environment.GetEnvironmentVariable("connectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandText = "SELECT * FROM tbLeaderboard WHERE difficulty = @difficulty ORDER BY score DESC";

                        sqlCommand.Parameters.AddWithValue("@difficulty", difficulty);

                        SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            Trappenspel newLeaderboard = new Trappenspel();

                            newLeaderboard.PlayerID = int.Parse(reader["playerID"].ToString());
                            newLeaderboard.Playername = reader["playername"].ToString();
                            newLeaderboard.Score = int.Parse(reader["score"].ToString());
                            newLeaderboard.Difficulty = reader["difficulty"].ToString();
                            newLeaderboard.Date = DateTime.Parse(reader["date"].ToString());
                            newLeaderboard.Steps = int.Parse(reader["steps"].ToString());
                            leaderboard.Add(newLeaderboard);
                        }
                    }
                }
                return new ObjectResult(leaderboard);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("GetLeaderBoardListSinglePerson")]
        public static async Task<IActionResult> GetLeaderBoardListSinglePerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/{playername}")] HttpRequest req, string playername, 
            ILogger log)
        {
            try
            {
                List<Trappenspel> leaderboard = new List<Trappenspel>();

                string connectionString = Environment.GetEnvironmentVariable("connectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandText = "SELECT * FROM tbLeaderboard WHERE playername = @playername ORDER BY score DESC";

                        sqlCommand.Parameters.AddWithValue("@playername", playername);

                        SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            Trappenspel newLeaderboard = new Trappenspel();

                            newLeaderboard.PlayerID = int.Parse(reader["playerID"].ToString());
                            newLeaderboard.Playername = reader["playername"].ToString();
                            newLeaderboard.Score = int.Parse(reader["score"].ToString());
                            newLeaderboard.Difficulty = reader["difficulty"].ToString();
                            newLeaderboard.Date = DateTime.Parse(reader["date"].ToString());
                            newLeaderboard.Steps = int.Parse(reader["steps"].ToString());
                            leaderboard.Add(newLeaderboard);
                        }
                    }
                }
                return new ObjectResult(leaderboard);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("PostLeaderboard")]
        public static async Task<IActionResult> PostLeaderboard(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "postleaderboard")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Trappenspel TrappenspelObj = JsonConvert.DeserializeObject<Trappenspel>(requestBody);

                string connectionString = Environment.GetEnvironmentVariable("connectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;

                        // insert statement
                        //command.CommandText = "INSERT INTO tbLeaderboard (playerid, playername, score, date, difficulty, steps) SELECT @playerid, @playername, @score, @date, @difficulty, @steps WHERE NOT EXISTS (SELECT playerid, playername, score, date, difficulty, steps FROM tblTrappenspel WHERE playerid = @playerid)";
                        command.CommandText = "INSERT INTO tbLeaderboard VALUES (@playername, @score, @date, @difficulty, @steps)";

                        command.Parameters.AddWithValue("@playerid", TrappenspelObj.PlayerID);
                        command.Parameters.AddWithValue("@playername", TrappenspelObj.Playername);
                        command.Parameters.AddWithValue("@score", TrappenspelObj.Score);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@difficulty", TrappenspelObj.Difficulty);
                        command.Parameters.AddWithValue("@steps", TrappenspelObj.Steps);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return new OkObjectResult(TrappenspelObj);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }
    }
}
