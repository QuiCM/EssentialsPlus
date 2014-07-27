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
				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
				ServerApi.Hooks.NetSendData.Deregister(this, OnSendData);
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
			}
		}
		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
			ServerApi.Hooks.NetSendData.Register(this, OnSendData);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
		}
		
		private void OnGetData(GetDataEventArgs e)
		{
			if (e.Handled)
				return;

			TSPlayer tsplayer = TShock.Players[e.Msg.whoAmI];
			if (tsplayer == null)
				return;

			Player player = tsplayer.GetEssentialsPlayer();
			if (player == null)
				return;

			using (var reader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
			{
				switch (e.MsgID)
				{
					#region Packet 45 - PlayerKillMe
					case PacketTypes.PlayerKillMe:
						{
							var playerID = reader.ReadByte();
							if (playerID != e.Msg.whoAmI)
								return;

							if (player.HasPermission("essentials.tp.back") && (Config.EnableBackInPvp || !player.TPlayer.hostile))
							{
								player.AddBackPoint(player.TPlayer.position);
								player.SendInfoMessage("Use /back to return to your death point!");
							}
							return;
						}
					#endregion
				}
			}
		}
		private void OnGreetPlayer(GreetPlayerEventArgs e)
		{
			if (e.Handled)
				return;

			TSPlayer tsplayer = TShock.Players[e.Who];
			if (tsplayer != null)
				tsplayer.AttachEssentialsPlayer();
		}
		private void OnInitialize(EventArgs e)
		{
			#region Config
			var path = Path.Combine(TShock.SavePath, "essentials.json");
			(Config = Config.Read(path)).Write(path);
			#endregion
			#region Commands
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

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.sudo", Commands.Sudo, "sudo")
			{
				HelpText = "Allows you to execute a command as another user."
			});

			TShockAPI.Commands.ChatCommands.Add(new Command("essentials.find", Commands.Find, "find") {
				HelpText = "Finds an item and/or NPC with the specified name."
			});
			#endregion
		}
		private void OnLeave(LeaveEventArgs e)
		{
			TSPlayer tsplayer = TShock.Players[e.Who];
			if (tsplayer != null)
				tsplayer.DetachEssentialsPlayer();
		}
		private void OnSendData(SendDataEventArgs e)
		{
			if (e.Handled)
				return;
		}
	}
}
