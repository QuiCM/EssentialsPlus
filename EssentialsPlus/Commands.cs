using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EssentialsPlus.Extensions;
using Terraria;
using TShockAPI;
using System.Text.RegularExpressions;

namespace EssentialsPlus
{
	public static class Commands
	{
		private static readonly Regex findPattern = new Regex("(\\/?\\w+) (-(\\w+) )?\"*(.+?)\"*$");

		public static void Back(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}back [steps]", TShock.Config.CommandSpecifier);
				return;
			}

			int steps = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out steps) || steps <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of steps '{0}'!", e.Parameters[0]);
				return;
			}

			Player player = e.Player.GetEssentials();
			if (player.BackHistoryCount == 0)
			{
				e.Player.SendErrorMessage("Could not teleport back!");
				return;
			}

			steps = Math.Min(steps, player.BackHistoryCount);
			e.Player.SendSuccessMessage("Teleported back {0} step{1}.", steps, steps == 1 ? "" : "s");
			player.Teleport(player.PopBackHistory(steps));
		}
		public static async void Down(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}down [levels]", TShock.Config.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool empty = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY + 3, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int j = y; currentLevel < levels && j < Main.maxTilesY - 2; j++)
				{
					if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
						Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
						Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
					{
						empty = true;
					}
					else if (empty)
					{
						empty = false;
						currentLevel++;
						y = j;
					}
				}
			});

			if (currentLevel <= 0)
				e.Player.SendErrorMessage("Could not teleport down!");
			else
			{
				if (e.Player.HasPermission("essentials.tp.back"))
					e.Player.GetEssentials().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y - 10);
				e.Player.SendSuccessMessage("Teleported down {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
		public static async void Left(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}left [levels]", TShock.Config.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool solid = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int i = x; currentLevel < levels && i >= 0; i--)
				{
					if (Main.tile[i, y].IsEmpty() && Main.tile[i + 1, y].IsEmpty() &&
						Main.tile[i, y + 1].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
						Main.tile[i, y + 2].IsEmpty() && Main.tile[i + 1, y + 2].IsEmpty())
					{
						if (solid)
						{
							solid = false;
							currentLevel++;
							x = i;
						}
					}
					else
						solid = true;
				}
			});

			if (currentLevel <= 0)
				e.Player.SendErrorMessage("Could not teleport left!");
			else
			{
				if (e.Player.HasPermission("essentials.tp.back"))
					e.Player.GetEssentials().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x + 12, 16 * y);
				e.Player.SendSuccessMessage("Teleported left {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
		public static async void Right(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}right [levels]", TShock.Config.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool solid = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX + 1, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int i = x; currentLevel < levels && i < Main.maxTilesX - 1; i++)
				{
					if (Main.tile[i, y].IsEmpty() && Main.tile[i + 1, y].IsEmpty() &&
						Main.tile[i, y + 1].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
						Main.tile[i, y + 2].IsEmpty() && Main.tile[i + 1, y + 2].IsEmpty())
					{
						if (solid)
						{
							solid = false;
							currentLevel++;
							x = i;
						}
					}
					else
						solid = true;
				}
			});

			if (currentLevel <= 0)
				e.Player.SendErrorMessage("Could not teleport right!");
			else
			{
				if (e.Player.HasPermission("essentials.tp.back"))
					e.Player.GetEssentials().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y);
				e.Player.SendSuccessMessage("Teleported right {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
		public static async void Up(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}up [levels]", TShock.Config.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool solid = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int j = y; currentLevel < levels && j >= 0; j--)
				{
					if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
						Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
						Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
					{
						if (solid)
						{
							solid = false;
							currentLevel++;
							y = j;
						}
					}
					else
						solid = true;
				}
			});

			if (currentLevel <= 0)
				e.Player.SendErrorMessage("Could not teleport up!");
			else
			{
				if (e.Player.HasPermission("essentials.tp.back"))
					e.Player.GetEssentials().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y + 6);
				e.Player.SendSuccessMessage("Teleported up {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}

		public static async void RepeatLast(CommandArgs e)
		{
			string lastCommand = e.Player.GetEssentials().LastCommand;
			if (String.IsNullOrEmpty(lastCommand))
			{
				e.Player.SendErrorMessage("You don't have a last command!");
				return;
			}

			e.Player.SendSuccessMessage("Repeated last command '{0}{1}'!", TShock.Config.CommandSpecifier, lastCommand);
			await Task.Run(() => TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.CommandSpecifier + lastCommand));
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

				e.Player.SendSuccessMessage("Forced {0} to execute {1}{2}.", players[0].Name, TShock.Config.CommandSpecifier, message);
				if (!e.Player.Group.HasPermission("essentials.sudo.invisible"))
					players[0].SendInfoMessage("{0} forced you to execute {1}{2}.", e.Player.Name, TShock.Config.CommandSpecifier, message);

				try
				{
					await Task.Run(() => command.CommandDelegate(new CommandArgs(message, players[0], args)));
				}
				catch (Exception ex)
				{
					e.Player.SendErrorMessage("Command failed, check logs for more details.");
					Log.Error(ex.ToString());
				}
			}
		}

		public static async void Find(CommandArgs e)
		{
			Match match = null;
			string switchText = null;
			string findText = null;
			List<KeyValuePair<int, string>> itemList = new List<KeyValuePair<int,string>>();
			List<KeyValuePair<int, string>> npcList = new List<KeyValuePair<int,string>>();
			StringBuilder sb = new StringBuilder();
			/*
			 * Output per item:  Eye of Chtulhu (NPC ID 4)
			 */
			string itemTemplate = "{1} ({0} ID {2})";
			int itemsPerLine = 3;
			int totalItemCount = 0;
			int totalLineCount = 0;
			int maxLineCount = 7;

			/*
			 * Match groups:
			 *	0: The string, the whole string and nothing but the string
			 *	3: The switch text, eg "npc", or "item" (optional)
			 *	4: The search paramaters. May include spaces and never quotes
			 */
			if (findPattern.IsMatch(e.Message) == false
				|| (match = findPattern.Match(e.Message)) == null
				|| string.IsNullOrEmpty((findText = match.Groups[4].Value)) == true) {
				e.Player.SendErrorMessage("/find: Invalid syntax. Usage: /find -[npc|item] <name>");
				return;
			}

			if (string.IsNullOrEmpty((switchText = match.Groups[3].Value)) == true) {
				switchText = "all";
			}

			if (switchText.Equals("all", StringComparison.InvariantCultureIgnoreCase)) {
				itemList.AddRange(await FindItemByNameAsync(findText));
				npcList.AddRange(await FindNPCByNameAsync(findText));
			} else if (switchText.Equals("npc", StringComparison.InvariantCultureIgnoreCase)) {
				npcList.AddRange(await FindNPCByNameAsync(findText));
			} else if (switchText.Equals("item", StringComparison.InvariantCultureIgnoreCase)) {
				itemList.AddRange(await FindItemByNameAsync(findText));
			}

			totalItemCount = npcList.Count + itemList.Count;
			totalLineCount = totalItemCount / itemsPerLine;

			if (totalItemCount == 0) {
				e.Player.SendInfoMessage("/find: nothing was found for that search criteria.");
				return;
			}

			if (totalLineCount > maxLineCount) {
				e.Player.SendInfoMessage("/find: too many results were returned, please try narrowing your search.");
				return;
			}

			/*
			 * Format items
			 */
			for (int i = 0; i < itemList.Count; i++) {
				KeyValuePair<int, string> item = itemList[i];
				sb.AppendFormat(itemTemplate, "Item", item.Value, item.Key);
				if (i % itemsPerLine == 0) {
					e.Player.SendInfoMessage(sb.ToString());
					sb.Clear();
				} else if (i <= itemList.Count - 1) {
					sb.Append(", ");
				}
			}

			if (sb.Length > 0) {
				e.Player.SendInfoMessage(sb.ToString());
				sb.Clear();
			}

			/*
			 * Format NPCs
			 */
			for (int i = 0; i < npcList.Count; i++) {
				KeyValuePair<int, string> item = npcList[i];
				sb.AppendFormat(itemTemplate, "NPC", item.Value, item.Key);
				if (i % itemsPerLine == 0) {
					e.Player.SendInfoMessage(sb.ToString());
					sb.Clear();
				} else if (i <= npcList.Count - 1) {
					sb.Append(", ");
				}
			}

			if (sb.Length > 0) {
				e.Player.SendInfoMessage(sb.ToString());
				sb.Clear();
			}
		}

		private static async Task<List<KeyValuePair<int, string>>> FindItemByNameAsync(string findText)
		{
			List<KeyValuePair<int, string>> itemList = new List<KeyValuePair<int, string>>();

			await Task.Run(() => {
				for (int i = 0; i < Terraria.Main.itemName.Count(); i++) {
					Terraria.Item item = new Terraria.Item();
					item.SetDefaults(Terraria.Main.itemName[i]);

					if (item.name.ContainsInsensitive(findText) == false) {
						continue;
					}
					itemList.Add(new KeyValuePair<int, string>(item.netID, item.name));
				}
			});

			return itemList;
		}
		private static async Task<List<KeyValuePair<int, string>>> FindNPCByNameAsync(string findText)
		{
			List<KeyValuePair<int, string>> npcList = new List<KeyValuePair<int, string>>();

			await Task.Run(() => {
				for (int i = 0; i < Terraria.Main.npcName.Count(); i++) {
					Terraria.NPC npc = new Terraria.NPC();
					npc.SetDefaults(Terraria.Main.npcName[i]);

					if (npc.name.ContainsInsensitive(findText) == false) {
						continue;
					}

					npcList.Add(new KeyValuePair<int, string>(npc.netID, npc.name));
				}
			});

			return npcList;
		}
	}
}
