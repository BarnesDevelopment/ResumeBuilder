using System.Data.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public class PostgreSqlContext
{
    protected readonly DbConnection Db;

    public PostgreSqlContext(IOptions<AWSSecrets> options)
    {
        var secrets = options.Value;
        Db = new MySqlConnection(secrets.ConnectionStrings_PostgreSql);
    }

   
}