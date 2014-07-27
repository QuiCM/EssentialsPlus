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
		    if (e.Parameters.Count == 0)
		    {
		        e.Parameters.Add("1");
		    }
            else if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}down [levels]", TShock.Config.CommandSpecifier);
                return;
            }

		    int levels = 0;
            bool tile = false;
            int levelsDetected = 0;

		    if (int.TryParse(e.Parameters[0], out levels))
		    {
		        for (int i = e.Player.TileY; i < Main.maxTilesY; i++)
		        {
		            if (Main.tile[e.Player.TileX, i].active() == false)
		            {
		                if (tile)
		                {
		                    tile = false;
		                    levelsDetected++;
		                    if (levelsDetected == levels)
		                    {
                                e.Player.GetEssentialsPlayer().AddBackPoint(new Vector2(e.Player.X, e.Player.Y));
		                        e.Player.Teleport(e.Player.X, (i*16));
                                e.Player.SendInfoMessage("Teleported down {0} levels.", levelsDetected);
		                        return;
		                    }
		                }
		            }
		            else
		            {
		                tile = true;
		            }
		        }
		        e.Player.SendInfoMessage("Could not teleport {0} levels. Only {1} levels detected.", levels, levelsDetected);
		    }
		    else
		    {
                e.Player.SendErrorMessage("Invalid distance '{0}'!", e.Parameters[0]);    
		    }
		}

		public static void Left(CommandArgs e)
		{
            if (e.Parameters.Count == 0)
            {
                e.Parameters.Add("1");
            }
            else if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}left [distance]", TShock.Config.CommandSpecifier);
                return;
            }

            int levels = 0;
            bool tile = false;
            int levelsDetected = 0;

            if (int.TryParse(e.Parameters[0], out levels))
            {
                for (int i = e.Player.TileX; i > 1; i--)
                {
                    if (Main.tile[i, e.Player.TileY].active() == false)
                    {
                        if (tile)
                        {
                            tile = false;
                            levelsDetected++;
                            if (levelsDetected == levels)
                            {
                                e.Player.GetEssentialsPlayer().AddBackPoint(new Vector2(e.Player.X, e.Player.Y));
                                e.Player.Teleport(((i-1) * 16), e.Player.Y);
                                e.Player.SendInfoMessage("Teleported left {0} levels.", levelsDetected);
                                return;
                            }
                        }
                    }
                    else
                    {
                        tile = true;
                    }
                }
                e.Player.SendInfoMessage("Could not teleport {0} levels. Only {1} levels detected.", levels, levelsDetected);
            }
            else
            {
                e.Player.SendErrorMessage("Invalid distance '{0}'!", e.Parameters[0]);
            }
		}

		public static void Right(CommandArgs e)
        {
            if (e.Parameters.Count == 0)
            {
                e.Parameters.Add("1");
            }
            else if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}right [distance]", TShock.Config.CommandSpecifier);
                return;
            }

            int levels = 0;
            bool tile = false;
            int levelsDetected = 0;

            if (int.TryParse(e.Parameters[0], out levels))
            {
                for (int i = e.Player.TileX; i < Main.maxTilesX; i++)
                {
                    if (Main.tile[i, e.Player.TileY].active() == false)
                    {
                        if (tile)
                        {
                            tile = false;
                            levelsDetected++;
                            if (levelsDetected == levels)
                            {
                                e.Player.GetEssentialsPlayer().AddBackPoint(new Vector2(e.Player.X, e.Player.Y));
                                e.Player.Teleport((i * 16), e.Player.Y);
                                e.Player.SendInfoMessage("Teleported right {0} levels.", levelsDetected);
                                return;
                            }
                        }
                    }
                    else
                    {
                        tile = true;
                    }
                }
                e.Player.SendInfoMessage("Could not teleport {0} levels. Only {1} levels detected.", levels, levelsDetected);
            }
            else
            {
                e.Player.SendErrorMessage("Invalid distance '{0}'!", e.Parameters[0]);
            }
		}

		public static void Up(CommandArgs e)
		{
            if (e.Parameters.Count == 0)
            {
                e.Parameters.Add("1");
            }
            else if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}down [distance]", TShock.Config.CommandSpecifier);
                return;
            }

            int levels = 0;
            bool tile = false;
            int levelsDetected = 0;

            if (int.TryParse(e.Parameters[0], out levels))
            {
                for (int i = e.Player.TileY; i > 1; i--)
                {
                    if (Main.tile[e.Player.TileX, i].active() == false)
                    {
                        if (tile)
                        {
                            tile = false;
                            levelsDetected++;
                            if (levelsDetected == levels)
                            {
                                e.Player.GetEssentialsPlayer().AddBackPoint(new Vector2(e.Player.X, e.Player.Y));
                                e.Player.Teleport(e.Player.X, ((i-3) * 16));
                                e.Player.SendInfoMessage("Teleported up {0} levels.", levelsDetected);
                                return;
                            }
                        }
                    }
                    else
                    {
                        tile = true;
                    }
                }
                e.Player.SendInfoMessage("Could not teleport {0} levels. Only {1} levels detected.", levels, levelsDetected);
            }
            else
            {
                e.Player.SendErrorMessage("Invalid distance '{0}'!", e.Parameters[0]);
            }
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
