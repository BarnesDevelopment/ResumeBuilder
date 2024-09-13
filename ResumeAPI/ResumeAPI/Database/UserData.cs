using Dapper;
using Microsoft.Extensions.Options;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IUserData
{
  Task<User?> GetUser(Guid id);
  Task<Guid> CreateUser(Guid id);
  Task<bool> DeleteUser(Guid id);
}

public class UserData : PostgreSqlContext, IUserData
{
  public UserData(IOptions<AWSSecrets> options) : base(options)
  {
  }

  public async Task<User?> GetUser(Guid id)
  {
    return (await Db.QueryAsync<User>($@"select 
                    id {nameof(User.Id)},
                    created_date {nameof(User.CreatedDate)}
                    from ResumeDb.Users where id = @id", new { id = id }))
      .FirstOrDefault();
  }

  public async Task<Guid> CreateUser(Guid id)
  {
    await Db.ExecuteAsync(
      @"insert into ResumeDb.Users (id) values(@id)",
      new
      {
        id
      });

    return id;
  }

  public async Task<bool> DeleteUser(Guid id)
  {
    return await Db.ExecuteAsync("delete from ResumeDb.Users where id = @id", new { id = id }) > 0;
  }
}
