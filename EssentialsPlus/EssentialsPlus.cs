using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace EssentialsPlus
{
	[ApiVersion(1, 16)]
	public partial class EssentialsPlus : TerrariaPlugin
	{
		private Config config = new Config();

		public override string Author
		{
			get { return "White et al."; }
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
			}
		}
		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
		}

		private void OnInitialize(EventArgs e)
		{
			#region Config
			var path = Path.Combine(TShock.SavePath, "essentials.json");
			(config = Config.Read(path)).Write(path);
			#endregion
			#region Commands
			Commands.ChatCommands.Add(new Command("essentials.tp.back", BackCommand, "back", "b")
			{
				AllowServer = false,
				HelpText = "Teleports you back to your previous position after dying or teleporting."
			});
			Commands.ChatCommands.Add(new Command("essentials.tp.down", DownCommand, "down")
			{
				AllowServer = false,
				HelpText = "Teleports you down through a layer of blocks."
			});
			Commands.ChatCommands.Add(new Command("essentials.tp.left", LeftCommand, "left")
			{
				AllowServer = false,
				HelpText = "Teleports you left through a layer of blocks."
			});
			Commands.ChatCommands.Add(new Command("essentials.tp.right", RightCommand, "right")
			{
				AllowServer = false,
				HelpText = "Teleports you right through a layer of blocks."
			});
			Commands.ChatCommands.Add(new Command("essentials.tp.up", UpCommand, "up")
			{
				AllowServer = false,
				HelpText = "Teleports you up through a layer of blocks."
			});
			#endregion
		}
	}
}
