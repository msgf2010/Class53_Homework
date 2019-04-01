using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Class53.Data
{

    public class Manager
    {
        private string _connectionString;

        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddContributor(Contributor contributor)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors (FirstName, LastName, Cell, AlwaysInclude)
                                VALUES (@firstName, @lastName, @cell, @alwaysInclude) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cell", contributor.Cell);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            conn.Open();
            contributor.Id = (int)(decimal)cmd.ExecuteScalar();
        }

        public void AddSimcha(Simcha simcha)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas (Date, SimchaName)
                                VALUES (@date, @simchaName)";
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            cmd.Parameters.AddWithValue("@simchaName", simcha.SimchaName);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void AddDeposit(Deposit deposit)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits (Date, Amount, ContributorId)
                                VALUES (@date, @amount, @contributorId)";
            cmd.Parameters.AddWithValue("@date", deposit.Date);
            cmd.Parameters.AddWithValue("@amount", deposit.Amount);
            cmd.Parameters.AddWithValue("@contributorId", deposit.PersonId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void AddContribution(int cId, int sId, decimal amount)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO PersonSimcha (ContributorId, SimchaId, Amount)
                                VALUES (@contributorId, @simchaId, @amount)";
            cmd.Parameters.AddWithValue("@contributorId", cId);
            cmd.Parameters.AddWithValue("@simchaId", sId);
            cmd.Parameters.AddWithValue("@amount", amount);
            conn.Open();
            cmd.ExecuteNonQuery();

            conn.Close();
            conn.Dispose();
        }

        public void DeleteContribution(int cId, int sId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM PersonSimcha
                                WHERE ContributorId = @cId AND SimchaId = @sId";
            cmd.Parameters.AddWithValue("@cId", cId);
            cmd.Parameters.AddWithValue("@sId", sId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public IEnumerable<Contributor> GetContributors()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT c.Id, c.FirstName, c.LastName, c.Cell, c.AlwaysInclude FROM Contributors c";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contributor> contributors = new List<Contributor>();

            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                });
            }

            return contributors;
        }
        
        public IEnumerable<SimchaContributor> GetContributorsForSimcha(int simchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT c.Id, c.AlwaysInclude FROM Contributors c
                                JOIN PersonSimcha ps
                                ON ps.ContributorId = c.Id
                                WHERE ps.SimchaId = @simchId";
            cmd.Parameters.AddWithValue("@simchId", simchaId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<SimchaContributor> simchaContributor = new List<SimchaContributor>();

            while (reader.Read())
            {
                simchaContributor.Add(new SimchaContributor
                {
                    ContributorId = (int)reader["Id"]
                });
            }

            return simchaContributor;
        }

        public bool DidPersonContribute(int simchaId, int cid)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors c
                                LEFT JOIN PersonSimcha ps
                                ON ps.ContributorId = c.Id
                                WHERE ps.SimchaId = @simchId AND c.Id = @cid";
            cmd.Parameters.AddWithValue("@simchId", simchaId);
            cmd.Parameters.AddWithValue("@cid", cid);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return false;
            }

            return true;
        }

        public IEnumerable<Simcha> GetSimchas()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Simcha> simchas = new List<Simcha>();

            while (reader.Read())
            {
                simchas.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"]
                });
            }

            return simchas;
        }

        public decimal GetTotalAmountForSimcha(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT ISNULL(SUM(ps.Amount), 0) AS 'Amount' FROM PersonSimcha ps
                                JOIN Simchas s
                                ON s.Id = ps.SimchaId
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }
            return (decimal)reader["Amount"];
        }
        
        public decimal GetIndividualAmountForSimcha(int simchaId, int cId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Amount FROM PersonSimcha ps
                                WHERE ps.SimchaId = @simchaId AND ps.ContributorId = @cId";
            cmd.Parameters.AddWithValue("@cId", cId);
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }
            return (decimal)reader["Amount"];
        }

        public string GetSimchaName(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SimchaName FROM Simchas
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return (string)reader["SimchaName"];
        }

        public int GetTotalContributorsForSimcha(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT ISNULL(COUNT(ps.ContributorId), 0) AS 'Contributors' FROM PersonSimcha ps
                                JOIN Simchas s
                                ON s.Id = ps.SimchaId
                                WHERE ps.SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return 0;
            }

            return (int)reader["Contributors"];
        }

        public int GetAllContributors()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(Id) AS 'TotalContributors' FROM Contributors";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }

            return (int)reader["TotalContributors"];
        }

        public decimal GetDeposits(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT ISNULL(SUM(Amount), 0) as 'DepositAmount' FROM Deposits WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }

            return (decimal)reader["DepositAmount"];
        }

        public decimal GetContributions(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = @"SELECT ISNULL(SUM(ps.Amount), 0) AS 'ContributedAmount' FROM Contributors c
                                JOIN PersonSimcha ps
                                ON ps.ContributorId = c.Id
                                WHERE c.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }

            return (decimal)reader["ContributedAmount"];
        }

        public IEnumerable<Contributor> GetContributorHistory()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT c.Id, c.FirstName, c.LastName, c.Cell, c.AlwaysInclude FROM Contributors c";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contributor> contributors = new List<Contributor>();

            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                });
            }

            return contributors;
        }
    }
}
