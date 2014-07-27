using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EssentialsPlus.Extensions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace EssentialsPlus
{
	[ApiVersion(1, 16)]
	public class EssentialsPlus : TerrariaPlugin
	{
		public static Config Config = new Config();

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
				PlayerHooks.PlayerCommand -= OnPlayerCommand;

				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				ServerApi.Hooks.NetSendData.Deregister(this, OnSendData);
			}
		}
		public override void Initialize()
		{
			PlayerHooks.PlayerCommand += OnPlayerCommand;

			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			ServerApi.Hooks.NetSendData.Register(this, OnSendData);
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
					if (tsplayer.HasPermission("essentials.tp.back"))
						tsplayer.GetEssentials().PushBackHistory(tsplayer.TPlayer.position);
					return;
				#endregion
			}
		}
		private void OnInitialize(EventArgs e)
		{
			#region Config
			var path = Path.Combine(TShock.SavePath, "essentials.json");
			(Config = Config.Read(path)).Write(path);
			#endregion
			#region Commands
			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.find", Commands.Find, "find") {
				HelpText = "Finds an item and/or NPC with the specified name."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.lastcommand", Commands.RepeatLast, "=")
			{
				HelpText = "Allows you to repeat your last command."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.sudo", Commands.Sudo, "sudo")
			{
				HelpText = "Allows you to execute a command as another user."
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
		}
		private void OnPlayerCommand(PlayerCommandEventArgs e)
		{
			if (e.Handled || e.Player == null)
				return;

			Command command = e.CommandList.FirstOrDefault();
			if (command == null || (command.Permissions.Any() && !command.Permissions.Any(s => e.Player.HasPermission(s))))
				return;

			if (e.Player.TPlayer.hostile && command.Names.Select(s => s.ToLowerInvariant()).Intersect(Config.DisabledCommandsInPvp.Select(s => s.ToLowerInvariant())).Any())
			{
				e.Player.SendErrorMessage("This command is blocked while in PvP!");
				e.Handled = true;
			}
			else if (e.Player.HasPermission("essentials.lastcommand") && command.CommandDelegate != Commands.RepeatLast)
				e.Player.GetEssentials().LastCommand = e.CommandText;
		}
		private void OnSendData(SendDataEventArgs e)
		{
			if (e.Handled)
				return;
		}
	}
}
