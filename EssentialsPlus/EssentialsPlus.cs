using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
				ServerApi.Hooks.NetSendData.Deregister(this, OnSendData);
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
			ServerApi.Hooks.NetSendData.Register(this, OnSendData);
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
			else if (e.Player.Group.HasPermission("essentials.lastcommand") && command.CommandDelegate != Commands.RepeatLast)
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
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.find", Commands.Find, "find")
			{
				HelpText = "Finds an item and/or NPC with the specified name."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.freezetime", Commands.FreezeTime, "freezetime")
			{
				HelpText = "Toggles freezing the time."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.home.delete", Commands.DeleteHome, "delhome")
			{
				AllowServer = false,
				HelpText = "Deletes one of your home points."
			});
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.home.set", Commands.SetHome, "sethome")
			{
				AllowServer = false,
				HelpText = "Sets you a home point."
			});
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.home.tp", Commands.MyHome, "myhome")
			{
				AllowServer = false,
				HelpText = "Teleports you to one of your home points."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.kickall", Commands.KickAll, "kickall")
			{
				HelpText = "Kicks everyone on the server."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.lastcommand", Commands.RepeatLast, "=")
			{
				HelpText = "Allows you to repeat your last command."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.ruler", Commands.Ruler, "ruler")
			{
				AllowServer = false,
				HelpText = "Allows you to measure the distances between two blocks."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.sudo", Commands.Sudo, "sudo")
			{
				HelpText = "Allows you to execute a command as another user."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.timecmd", Commands.TimeCmd, "timecmd")
			{
				HelpText = "Executes a command after a given time interval."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.tp.back", Commands.Back, "back", "b")
			{
				AllowServer = false,
				HelpText = "Teleports you back to your previous position after dying or teleporting."
			});
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.tp.down", Commands.Down, "down")
			{
				AllowServer = false,
				HelpText = "Teleports you down through a layer of blocks."
			});
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.tp.left", Commands.Left, "left")
			{
				AllowServer = false,
				HelpText = "Teleports you left through a layer of blocks."
			});
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.tp.right", Commands.Right, "right")
			{
				AllowServer = false,
				HelpText = "Teleports you right through a layer of blocks."
			});
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.tp.up", Commands.Up, "up")
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
		private void OnPostInitialize(EventArgs e)
		{
			Homes = new HomeManager(Db);
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
					if (tsplayer.Group.HasPermission("essentials.tp.back"))
						tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
					return;
				#endregion
			}
		}
		private void OnSendData(SendDataEventArgs e)
		{
			if (e.Handled)
				return;
		}
	}
}
