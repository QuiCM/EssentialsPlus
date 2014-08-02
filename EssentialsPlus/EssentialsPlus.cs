using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Extensions;
using TShockAPI.Hooks;

namespace EssentialsPlus
{
	[ApiVersion(1, 16)]
	public class EssentialsPlus : TerrariaPlugin
	{
		public static Config Config { get; private set; }
		public static IDbConnection Db { get; private set; }
		public static HomeManager Homes { get; private set; }
		public static MuteManager Mutes { get; private set; }

		public override string Author
		{
			get { return "WhiteX et al."; }
		}
		public override string Description
		{
			get { return "Essentials, but better"; }
		}
		public override string Name
		{
			get { return "EssentialsPlus"; }
		}
		public override Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public EssentialsPlus(Main game)
			: base(game)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				GeneralHooks.ReloadEvent -= OnReload;
				PlayerHooks.PlayerCommand -= OnPlayerCommand;

				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
				ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
			}
			base.Dispose(disposing);
		}
		public override void Initialize()
		{
			GeneralHooks.ReloadEvent += OnReload;
			PlayerHooks.PlayerCommand += OnPlayerCommand;

			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
		}

		private async void OnReload(ReloadEventArgs e)
		{
			string path = Path.Combine(TShock.SavePath, "essentials.json");
			Config = Config.Read(path);
			if (!File.Exists(path))
				Config.Write(path);
			await Homes.ReloadAsync();
			e.Player.SendSuccessMessage("[EssentialsPlus] Reloaded config and homes!");
		}
		private void OnPlayerCommand(PlayerCommandEventArgs e)
		{
			if (e.Handled || e.Player == null)
				return;

			Command command = e.CommandList.FirstOrDefault();
			if (command == null || (command.Permissions.Any() && !command.Permissions.Any(s => e.Player.Group.HasPermission(s))))
				return;

			if (e.Player.TPlayer.hostile && command.Names.Select(s => s.ToLowerInvariant()).Intersect(Config.DisabledCommandsInPvp.Select(s => s.ToLowerInvariant())).Any())
			{
				e.Player.SendErrorMessage("This command is blocked while in PvP!");
				e.Handled = true;
			}
			else if (e.Player.Group.HasPermission(Permissions.LastCommand) && command.CommandDelegate != Commands.RepeatLast)
				e.Player.GetPlayerInfo().LastCommand = e.CommandText;
		}

		private void OnInitialize(EventArgs e)
		{
			#region Config
			string path = Path.Combine(TShock.SavePath, "essentials.json");
			Config = Config.Read(path);
			if (!File.Exists(path))
				Config.Write(path);
			#endregion
			#region Commands
			Action<Command> Add = c =>
			{
				TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Select(s => s.ToLowerInvariant()).Intersect(c.Names.Select(s => s.ToLowerInvariant())).Any());
				TShockAPI.Commands.ChatCommands.Add(c);
			};

			Add(new Command(Permissions.Find, Commands.Find, "find")
			{
				HelpText = "Finds an item and/or NPC with the specified name."
			});

			Add(new Command(Permissions.FreezeTime, Commands.FreezeTime, "freezetime")
			{
				HelpText = "Toggles freezing the time."
			});

			Add(new Command(Permissions.HomeDelete, Commands.DeleteHome, "delhome")
			{
				AllowServer = false,
				HelpText = "Deletes one of your home points."
			});
			Add(new Command(Permissions.HomeSet, Commands.SetHome, "sethome")
			{
				AllowServer = false,
				HelpText = "Sets you a home point."
			});
			Add(new Command(Permissions.HomeTp, Commands.MyHome, "myhome")
			{
				AllowServer = false,
				HelpText = "Teleports you to one of your home points."
			});

			Add(new Command(Permissions.KickAll, Commands.KickAll, "kickall")
			{
				HelpText = "Kicks everyone on the server."
			});

			Add(new Command(Permissions.LastCommand, Commands.RepeatLast, "=")
			{
				HelpText = "Allows you to repeat your last command."
			});

			Add(new Command(Permissions.Mute, Commands.Mute, "mute")
			{
				HelpText = "Manages mutes."
			});

			Add(new Command(Permissions.Ruler, Commands.Ruler, "ruler")
			{
				AllowServer = false,
				HelpText = "Allows you to measure the distances between two blocks."
			});

			Add(new Command(Permissions.Sudo, Commands.Sudo, "sudo")
			{
				HelpText = "Allows you to execute a command as another user."
			});

			Add(new Command(Permissions.TimeCmd, Commands.TimeCmd, "timecmd")
			{
				HelpText = "Executes a command after a given time interval."
			});

			Add(new Command(Permissions.TpBack, Commands.Back, "back", "b")
			{
				AllowServer = false,
				HelpText = "Teleports you back to your previous position after dying or teleporting."
			});
			Add(new Command(Permissions.TpDown, Commands.Down, "down")
			{
				AllowServer = false,
				HelpText = "Teleports you down through a layer of blocks."
			});
			Add(new Command(Permissions.TpLeft, Commands.Left, "left")
			{
				AllowServer = false,
				HelpText = "Teleports you left through a layer of blocks."
			});
			Add(new Command(Permissions.TpRight, Commands.Right, "right")
			{
				AllowServer = false,
				HelpText = "Teleports you right through a layer of blocks."
			});
			Add(new Command(Permissions.TpUp, Commands.Up, "up")
			{
				AllowServer = false,
				HelpText = "Teleports you up through a layer of blocks."
			});
			#endregion
			#region Database
			if (TShock.Config.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
			{
				string[] host = Config.MySqlHost.Split(':');
				Db = new MySqlConnection()
				{
					ConnectionString = String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
						host[0],
						host.Length == 1 ? "3306" : host[1],
						Config.MySqlDbName,
						Config.MySqlUsername,
						Config.MySqlPassword)
				};
			}
			else if (TShock.Config.StorageType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
				Db = new SqliteConnection("uri=file://" + Path.Combine(TShock.SavePath, "essentials.sqlite") + ",Version=3");
			else
				throw new InvalidOperationException("Invalid storage type!");
			#endregion
		}
		private async void OnJoin(JoinEventArgs e)
		{
			if (e.Handled)
				return;

			TSPlayer player = TShock.Players[e.Who];
			DateTime muteExpiration = await Mutes.GetExpirationAsync(player);

			if (DateTime.UtcNow < muteExpiration)
			{
				player.mute = true;
				try
				{
					await Task.Delay(muteExpiration - DateTime.UtcNow, player.GetPlayerInfo().MuteToken);
					player.mute = false;
					player.SendInfoMessage("You have been unmuted.");
				}
				catch (TaskCanceledException)
				{
				}
			}
		}
		private void OnPostInitialize(EventArgs e)
		{
			Homes = new HomeManager(Db);
			Mutes = new MuteManager(Db);
		}
		private void OnGetData(GetDataEventArgs e)
		{
			if (e.Handled)
				return;

			TSPlayer tsplayer = TShock.Players[e.Msg.whoAmI];
			if (tsplayer == null)
				return;

			switch (e.MsgID)
			{
				#region Packet 45 - PlayerKillMe
				case PacketTypes.PlayerKillMe:
					if (tsplayer.Group.HasPermission(Permissions.TpBack))
						tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
					return;
				#endregion
			}
		}
	}
}
