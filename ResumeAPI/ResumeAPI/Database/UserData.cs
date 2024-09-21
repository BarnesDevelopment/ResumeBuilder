using Dapper;
using Microsoft.Extensions.Options;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IUserData
{
  Task<User?> GetUser(Guid id);
  Task<User?> GetUser(string cookie);
  Task<Guid> CreateUser(Guid id);
  Task<string> CreateUser(Guid id, string cookie);
  Task<bool> DeleteUser(Guid id);
  Task UpdateAccessedDate(Guid id);
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
                    from ResumeDb.Users where id = @id", new { id }))
      .FirstOrDefault();
  }

  public async Task<User?> GetUser(string cookie)
  {
    return (await Db.QueryAsync<User>($@"select 
                    id {nameof(User.Id)},
                    created_date {nameof(User.CreatedDate)}
                    from ResumeDb.Users where demo_session_cookie = @cookie", new { cookie }))
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

  public async Task<string> CreateUser(Guid id, string cookie)
  {
    await Db.ExecuteAsync(
      @"insert into ResumeDb.Users (id, cookie) values(@id, @cookie)",
      new
      {
        id,
        cookie
      });

    return cookie;
  }

  public async Task<bool> DeleteUser(Guid id)
  {
    return await Db.ExecuteAsync("delete from ResumeDb.Users where id = @id", new { id }) > 0;
  }

  public async Task UpdateAccessedDate(Guid id)
  {
    await Db.ExecuteAsync("update ResumeDb.Users set last_accessed = now() where id = @id", new { id });
  }
}
