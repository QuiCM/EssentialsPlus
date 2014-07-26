using System;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Essentials
{
    [ApiVersion(1, 16)]
    public class EMain : TerrariaPlugin
    {
        private static EConfig _config = new EConfig();  //private unless needed

        public override string Author
        {
            get { return "White, MarioE, Enerdy, Ijwu"; }
        }

        public override string Description
        {
            get { return "Essentials, but better"; }
        }

        public override string Name
        {
            get { return "Essentials+"; }
        }

        public override Version Version
        {
            get { return new Version(0, 1); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        private static void OnInitialize(EventArgs args)
        {
            //Do we need a whole folder for Essentials, like last time?
            var path = Path.Combine(TShock.SavePath, "Essentials.json"); 
            (_config = EConfig.Read(path)).Write(path);

            Commands.ChatCommands.Add(new Command("essentials.position.back", ECommands.BackCommand, "back", "b")
            {
                AllowServer = false, 
                HelpText = "Moves you back to your previous position after teleporting, dying or warping"
            });
        }

        public EMain(Main game)
            : base(game)
        {
        }
    }
}
