using System.Data.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Npgsql;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public class PostgreSqlContext
{
    protected readonly DbConnection Db;

    public PostgreSqlContext(AppSettings appSettings)
    {
        Db = new NpgsqlConnection(appSettings.ConnectionStrings.Postgres);
    }

   
}