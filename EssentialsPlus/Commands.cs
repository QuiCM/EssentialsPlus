using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EssentialsPlus.Extensions;
using TShockAPI;

namespace EssentialsPlus
{
	public static class Commands
	{
		public static void Back(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}back [steps]", TShock.Config.CommandSpecifier);
				return;
			}
			if (e.TPlayer.hostile && !EssentialsPlus.Config.EnableBackInPvp)
			{
				e.Player.SendErrorMessage("You cannot teleport back in PvP!");
				return;
			}

			Player player = e.Player.GetEssentialsPlayer();
			int steps = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out steps) || steps < 0 || steps > player.BackPoints.Count))
			{
				e.Player.SendErrorMessage("Invalid number of steps '{0}'!", e.Parameters[0]);
				return;
			}
			player.Teleport(player.BackPoints[--steps]);
		}
		public static void Down(CommandArgs e)
		{
		}
		public static void Left(CommandArgs e)
		{
		}
		public static void Right(CommandArgs e)
		{
		}
		public static void Up(CommandArgs e)
		{
		}

		public static async void Sudo(CommandArgs e)
		{
			if (e.Parameters.Count < 2)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}sudo <player> <command...>", TShock.Config.CommandSpecifier);
				return;
			}

			List<TSPlayer> players = TShock.Utils.FindPlayer(e.Parameters[0]);
			if (players.Count == 0)
				e.Player.SendErrorMessage("Invalid player '{0}'!", e.Parameters[0]);
			else if (players.Count > 1)
				e.Player.SendErrorMessage("More than one player matched: {0}", String.Join(", ", players.Select(p => p.Name)));
			else
			{
				string message = String.Join(" ", e.Parameters.Skip(1));
				List<string> args = e.Parameters.Skip(2).ToList();

				Command command = TShockAPI.Commands.ChatCommands.Where(c => c.HasAlias(e.Parameters[1].ToLowerInvariant())).FirstOrDefault();
				if (command == null)
				{
					e.Player.SendErrorMessage("Invalid command '{0}{1}'!", TShock.Config.CommandSpecifier, e.Parameters[1]);
					return;
				}

				if (!e.Player.Group.HasPermission("essentials.sudo.super"))
				{
					if (!command.Permissions.Any(s => players[0].Group.HasPermission(s)))
					{
						e.Player.SendErrorMessage("{0} cannot execute {1}{2}!", players[0].Name, TShock.Config.CommandSpecifier, message);
						return;
					}
					else if (!command.Permissions.Any(s => e.Player.Group.HasPermission(s)))
					{
						e.Player.SendErrorMessage("You cannot execute {0}{1}!", TShock.Config.CommandSpecifier, message);
						return;
					}
				}

				await Task.Run(() => command.CommandDelegate(new CommandArgs(message, players[0], args)));

				e.Player.SendSuccessMessage("Forced {0} to execute {1}{2}.", players[0].Name, TShock.Config.CommandSpecifier, message);
				if (!e.Player.Group.HasPermission("essentials.sudo.invisible"))
					players[0].SendInfoMessage("{0} forced you to execute {1}{2}.", e.Player.Name, TShock.Config.CommandSpecifier, message);
			}
		}
	}
}
