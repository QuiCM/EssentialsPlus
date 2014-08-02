using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;
using System.Threading;

namespace EssentialsPlus.Db
{
	public class MuteManager
	{
		private IDbConnection db;
		private ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();

		public MuteManager(IDbConnection db)
		{
			this.db = db;

			var sqlCreator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
			sqlCreator.EnsureExists(new SqlTable("Mutes",
				new SqlColumn("ID", MySqlDbType.Int32) { AutoIncrement = true, Primary = true },
				new SqlColumn("Name", MySqlDbType.Text),
				new SqlColumn("UUID", MySqlDbType.Text),
				new SqlColumn("IP", MySqlDbType.Text),

				new SqlColumn("Date", MySqlDbType.Text),
				new SqlColumn("Expiration", MySqlDbType.Text)));
		}

		public async Task<bool> AddAsync(TSPlayer player, DateTime expiration)
		{
			try
			{
				return await Task.Run(() =>
				{
					syncLock.EnterWriteLock();
					return db.Query("INSERT INTO Mutes VALUES (@0, @1, @2, @3, @4, @5)",
						null,
						player.Name,
						player.UUID,
						player.IP,
						DateTime.UtcNow.ToString("s"),
						expiration.ToString("s")) > 0;
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
			finally
			{
				syncLock.ExitWriteLock();
			}
		}
		public async Task<bool> AddAsync(User user, DateTime expiration)
		{
			try
			{
				return await Task.Run(() =>
				{
					syncLock.EnterWriteLock();
					return db.Query("INSERT INTO Mutes VALUES (@0, @1, @2, @3, @4, @5)",
						null,
						user.Name,
						user.UUID,
						JsonConvert.DeserializeObject<List<string>>(user.KnownIps)[0],
						DateTime.UtcNow.ToString("s"),
						expiration.ToString("s")) > 0;
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
			finally
			{
				syncLock.ExitWriteLock();
			}
		}
		public async Task<bool> DeleteAsync(TSPlayer player)
		{
			try
			{
				return await Task.Run(() =>
				{
					syncLock.EnterWriteLock();
					return db.Query("DELETE FROM Mutes WHERE ID IN (SELECT ID FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1)",
						player.UUID,
						player.IP) > 0;
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
			finally
			{
				syncLock.ExitWriteLock();
			}
		}
		public async Task<bool> DeleteAsync(User user)
		{
			try
			{
				return await Task.Run(() =>
				{
					syncLock.EnterWriteLock();
					return db.Query("DELETE FROM Mutes WHERE ID IN (SELECT ID FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1)",
						user.UUID,
						JsonConvert.DeserializeObject<List<string>>(user.KnownIps)[0]) > 0;
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
			finally
			{
				syncLock.ExitWriteLock();
			}
		}
		public async Task<DateTime> GetExpirationAsync(TSPlayer player)
		{
			try
			{
				return await Task.Run(() =>
				{
					syncLock.EnterReadLock();

					DateTime dateTime = DateTime.MinValue;
					using (QueryResult result = db.QueryReader("SELECT Expiration FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC", player.UUID, player.IP))
					{
						if (result.Read())
							dateTime = DateTime.Parse(result.Get<string>("Expiration"));
					}

					syncLock.ExitReadLock();
					return dateTime;
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				syncLock.ExitReadLock();
				return DateTime.MinValue;
			}
		}
	}
}
