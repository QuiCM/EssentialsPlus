using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace EssentialsPlus
{
	public static class Commands
	{
		public static async void Find(CommandArgs e)
		{
			var regex = new Regex(@"^\w+ -(\w+) (.+?) ?(\d*)$");
			Match match = regex.Match(e.Message);
			if (!match.Success)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}find <-switch> <name...> [page]", TShock.Config.CommandSpecifier);
				e.Player.SendSuccessMessage("Valid {0}find switches:", TShock.Config.CommandSpecifier);
				e.Player.SendInfoMessage("-command: Finds a command.");
				e.Player.SendInfoMessage("-item: Finds an item.");
				e.Player.SendInfoMessage("-npc: Finds an NPC.");
				e.Player.SendInfoMessage("-tile: Finds a tile.");
				e.Player.SendInfoMessage("-wall: Finds a wall.");
				return;
			}

			int page = 1;
			if (!String.IsNullOrEmpty(match.Groups[3].Value) && (!int.TryParse(match.Groups[3].Value, out page) || page <= 0))
			{
				e.Player.SendErrorMessage("Invalid page '{0}'!", match.Groups[3].Value);
				return;
			}

			switch (match.Groups[1].Value.ToLowerInvariant())
			{
				#region Command
				case "command":
					var commands = new List<string>();

					await Task.Run(() =>
					{
						foreach (Command command in TShockAPI.Commands.ChatCommands.Where(c => c.Names.Any(s => s.ContainsInsensitive(match.Groups[2].Value))))
							commands.Add(String.Format("{0} (Permission: {1})", command.Name, command.Permissions.FirstOrDefault()));
					});

					PaginationTools.SendPage(e.Player, page, commands,
						new PaginationTools.Settings
						{
							HeaderFormat = "Found Commands ({0}/{1}):",
							FooterFormat = String.Format("Type /find -command {0} {{0}} for more", match.Groups[2].Value),
							NothingToDisplayString = "No commands were found."
						});
					return;
				#endregion
				#region Item
				case "item":
					var items = new List<string>();

					await Task.Run(() =>
					{
						for (int i = -48; i < 0; i++)
						{
							var item = new Item();
							item.netDefaults(i);
							if (item.name.ContainsInsensitive(match.Groups[2].Value))
								items.Add(String.Format("{0} (ID: {1})", item.name, i));
						}
						for (int i = 0; i < Main.itemName.Length; i++)
						{
							if (Main.itemName[i].ContainsInsensitive(match.Groups[2].Value))
								items.Add(String.Format("{0} (ID: {1})", Main.itemName[i], i));
						}
					});

					PaginationTools.SendPage(e.Player, page, items,
						new PaginationTools.Settings
						{
							HeaderFormat = "Found Items ({0}/{1}):",
							FooterFormat = String.Format("Type /find -item {0} {{0}} for more", match.Groups[2].Value),
							NothingToDisplayString = "No items were found."
						});
					return;
				#endregion
				#region NPC
				case "npc":
					var npcs = new List<string>();

					await Task.Run(() =>
					{
						for (int i = -65; i < 0; i++)
						{
							var npc = new NPC();
							npc.netDefaults(i);
							if (npc.name.ContainsInsensitive(match.Groups[2].Value))
								npcs.Add(String.Format("{0} (ID: {1})", npc.name, i));
						}
						for (int i = 0; i < Terraria.Main.npcName.Count(); i++)
						{
							if (Main.npcName[i].ContainsInsensitive(match.Groups[2].Value))
								npcs.Add(String.Format("{0} (ID: {1})", Main.npcName[i], i));
						}
					});

					PaginationTools.SendPage(e.Player, page, npcs,
						new PaginationTools.Settings
						{
							HeaderFormat = "Found NPCs ({0}/{1}):",
							FooterFormat = String.Format("Type /find -npc {0} {{0}} for more", match.Groups[2].Value),
							NothingToDisplayString = "No NPCs were found.",
						});
					return;
				#endregion
				#region Tile
				case "tile":
					var tiles = new List<string>();

					await Task.Run(() =>
					{
						foreach (FieldInfo fi in typeof(TileID).GetFields())
						{
							var sb = new StringBuilder();
							for (int i = 0; i < fi.Name.Length; i++)
							{
								if (Char.IsUpper(fi.Name[i]) && i > 0)
									sb.Append(" ").Append(fi.Name[i]);
								else
									sb.Append(fi.Name[i]);
							}

							string name = sb.ToString();
							if (name.ContainsInsensitive(match.Groups[2].Value))
								tiles.Add(String.Format("{0} (ID: {1})", name, fi.GetValue(null)));
						}
					});

					PaginationTools.SendPage(e.Player, page, tiles,
						new PaginationTools.Settings
						{
							HeaderFormat = "Found Tiles ({0}/{1}):",
							FooterFormat = String.Format("Type /find -tile {0} {{0}} for more", match.Groups[2].Value),
							NothingToDisplayString = "No tiles were found.",
						});
					return;
				#endregion
				#region Wall
				case "wall":
					var walls = new List<string>();

					await Task.Run(() =>
					{
						foreach (FieldInfo fi in typeof(WallID).GetFields())
						{
							var sb = new StringBuilder();
							for (int i = 0; i < fi.Name.Length; i++)
							{
								if (Char.IsUpper(fi.Name[i]) && i > 0)
									sb.Append(" ").Append(fi.Name[i]);
								else
									sb.Append(fi.Name[i]);
							}

							string name = sb.ToString();
							if (name.ContainsInsensitive(match.Groups[2].Value))
								walls.Add(String.Format("{0} (ID: {1})", name, fi.GetValue(null)));
						}
					});

					PaginationTools.SendPage(e.Player, page, walls,
						new PaginationTools.Settings
						{
							HeaderFormat = "Found Walls ({0}/{1}):",
							FooterFormat = String.Format("Type /find -wall {0} {{0}} for more", match.Groups[2].Value),
							NothingToDisplayString = "No walls were found.",
						});
					return;
				#endregion
				default:
					e.Player.SendSuccessMessage("Valid {0}find switches:", TShock.Config.CommandSpecifier);
					e.Player.SendInfoMessage("-command: Finds a command.");
					e.Player.SendInfoMessage("-item: Finds an item.");
					e.Player.SendInfoMessage("-npc: Finds an NPC.");
					e.Player.SendInfoMessage("-tile: Finds a tile.");
					e.Player.SendInfoMessage("-wall: Finds a wall.");
					return;
			}
		}

		private static System.Timers.Timer FreezeTimer = new System.Timers.Timer(1000);
		public static void FreezeTime(CommandArgs e)
		{
			if (FreezeTimer.Enabled)
			{
				FreezeTimer.Stop();
				TSPlayer.All.SendInfoMessage("{0} unfroze the time.", e.Player.Name);
			}
			else
			{
				bool dayTime = Main.dayTime;
				double time = Main.time;

				FreezeTimer.Dispose();
				FreezeTimer = new System.Timers.Timer(1000);
				FreezeTimer.Elapsed += (o, ee) =>
				{
					Main.dayTime = dayTime;
					Main.time = time;
					TSPlayer.All.SendData(PacketTypes.TimeSet);
				};
				FreezeTimer.Start();
				TSPlayer.All.SendInfoMessage("{0} froze the time.", e.Player.Name);
			}
		}

		public static async void DeleteHome(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}delhome <home name>", TShock.Config.CommandSpecifier);
				return;
			}

			string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
			Home home = await EssentialsPlus.Homes.Get(e.Player, homeName);
			if (home != null)
			{
				if (await EssentialsPlus.Homes.DeleteAsync(e.Player, homeName))
					e.Player.SendSuccessMessage("Deleted your home '{0}'.", homeName);
				else
					e.Player.SendErrorMessage("Could not delete home, check logs for more details.");
			}
			else
				e.Player.SendErrorMessage("Invalid home '{0}'!", homeName);
		}
		public static async void MyHome(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}myhome <home name>", TShock.Config.CommandSpecifier);
				return;
			}

			string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
			Home home = await EssentialsPlus.Homes.Get(e.Player, homeName);
			if (home != null)
			{
				e.Player.Teleport(home.X, home.Y);
				e.Player.SendSuccessMessage("Teleported you to your home '{0}'.", homeName);
			}
			else
				e.Player.SendErrorMessage("Invalid home '{0}'!", homeName);
		}
		public static async void SetHome(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}sethome <home name>", TShock.Config.CommandSpecifier);
				return;
			}

			string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
			if (EssentialsPlus.Homes.Get(e.Player, homeName) != null)
			{
				if (await EssentialsPlus.Homes.UpdateAsync(e.Player, homeName, e.Player.X, e.Player.Y))
					e.Player.SendSuccessMessage("Updated your home '{0}'.", homeName);
				else
					e.Player.SendErrorMessage("Could not update home, check logs for more details.");
				return;
			}

			if ((await EssentialsPlus.Homes.GetAll(e.Player)).Count + 1 >= e.Player.Group.GetDynamicPermission("essentials.home.set"))
			{
				e.Player.SendErrorMessage("You have reached your home limit!");
				return;
			}

			if (await EssentialsPlus.Homes.AddAsync(e.Player, homeName, e.Player.X, e.Player.Y))
				e.Player.SendSuccessMessage("Set your home '{0}'.", homeName);
			else
				e.Player.SendErrorMessage("Could not set home, check logs for more details.");
		}

		public static async void KickAll(CommandArgs e)
		{
			var regex = new Regex(@"^\w+(?: -(\w+))* ?(.*)$");
			Match match = regex.Match(e.Message);
			bool noSave = false;
			foreach (Capture capture in match.Groups[1].Captures)
			{
				switch (capture.Value.ToLowerInvariant())
				{
					case "nosave":
						noSave = true;
						continue;
					default:
						e.Player.SendSuccessMessage("Valid {0}kickall switches:", TShock.Config.CommandSpecifier);
						e.Player.SendInfoMessage("-nosave: Kicks without saving SSC data.");
						return;
				}
			}

			int kickLevel = e.Player.Group.GetDynamicPermission("essentials.kickall");
			string reason = String.IsNullOrEmpty(match.Groups[2].Value) ? "No reason." : match.Groups[2].Value;
			await Task.WhenAll(TShock.Players.Where(p => p != null && p.Group.GetDynamicPermission("essentials.kickall") < kickLevel).Select(p => Task.Run(() =>
			{
				if (!noSave && p.IsLoggedIn)
					p.SaveServerCharacter();
				p.Disconnect("Kicked: " + reason);
			})));
			e.Player.SendSuccessMessage("Kicked everyone for '{0}'.", reason);
		}

		public static async void RepeatLast(CommandArgs e)
		{
			string lastCommand = e.Player.GetPlayerInfo().LastCommand;
			if (String.IsNullOrEmpty(lastCommand))
			{
				e.Player.SendErrorMessage("You don't have a last command!");
				return;
			}

			e.Player.SendSuccessMessage("Repeated last command '{0}{1}'!", TShock.Config.CommandSpecifier, lastCommand);
			await Task.Run(() => TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.CommandSpecifier + lastCommand));
		}

		public static void Ruler(CommandArgs e)
		{
			if (e.Parameters.Count == 0)
			{
				if (e.Player.TempPoints.Any(p => p == Point.Zero))
				{
					e.Player.SendErrorMessage("Ruler points are not set up!");
					return;
				}

				Point p1 = e.Player.TempPoints[0];
				Point p2 = e.Player.TempPoints[1];
				int x = Math.Abs(p1.X - p2.X);
				int y = Math.Abs(p1.Y - p2.Y);
				double cartesian = Math.Sqrt(x * x + y * y);
				e.Player.SendInfoMessage("Distances: X: {0}, Y: {1}, Cartesian: {2:N3}", x, y, cartesian);
			}
			else if (e.Parameters.Count == 1)
			{
				if (e.Parameters[0] == "1")
				{
					e.Player.AwaitingTempPoint = 1;
					e.Player.SendInfoMessage("Modify a block to set the first ruler point.");
				}
				else if (e.Parameters[0] == "2")
				{
					e.Player.AwaitingTempPoint = 2;
					e.Player.SendInfoMessage("Modify a block to set the second ruler point.");
				}
				else
					e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}ruler [1/2]", TShock.Config.CommandSpecifier);
			}
			else
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}ruler [1/2]", TShock.Config.CommandSpecifier);
		}

		public static async void Sudo(CommandArgs e)
		{
			var regex = new Regex(String.Format(@"^\w+(?: -(\w+))* (?:""(.+?)""|([^\s]*?)) (?:{0})?(.+)$", TShock.Config.CommandSpecifier));
			Match match = regex.Match(e.Message);
			if (!match.Success)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}sudo [-switches...] <player> <command...>", TShock.Config.CommandSpecifier);
				e.Player.SendSuccessMessage("Valid {0}sudo switches:", TShock.Config.CommandSpecifier);
				e.Player.SendInfoMessage("-f, -force: Force sudo, ignoring permissions.");
				return;
			}

			bool force = false;
			foreach (Capture capture in match.Groups[1].Captures)
			{
				switch (capture.Value.ToLowerInvariant())
				{
					case "f":
					case "force":
						if (!e.Player.Group.HasPermission("essentials.sudo.force"))
						{
							e.Player.SendErrorMessage("You do not have access to the switch '-{0}'!", capture.Value);
							return;
						}
						force = true;
						continue;
					default:
						e.Player.SendSuccessMessage("Valid {0}sudo switches:", TShock.Config.CommandSpecifier);
						e.Player.SendInfoMessage("-f, -force: Force sudo, ignoring permissions.");
						return;
				}
			}

			string playerName = String.IsNullOrEmpty(match.Groups[3].Value) ? match.Groups[2].Value : match.Groups[3].Value;
			string command = match.Groups[4].Value;

			List<TSPlayer> players = TShock.Utils.FindPlayer(playerName);
			if (players.Count == 0)
				e.Player.SendErrorMessage("Invalid player '{0}'!", playerName);
			else if (players.Count > 1)
				e.Player.SendErrorMessage("More than one player matched: {0}", String.Join(", ", players.Select(p => p.Name)));
			else
			{
				if (e.Player.Group.GetDynamicPermission("essentials.sudo") <= players[0].Group.GetDynamicPermission("essentials.sudo"))
				{
					e.Player.SendErrorMessage("You cannot force {0} to execute {1}{2}!", players[0].Name, TShock.Config.CommandSpecifier, command);
					return;
				}

				e.Player.SendSuccessMessage("Forced {0} to execute {1}{2}.", players[0].Name, TShock.Config.CommandSpecifier, command);
				if (!e.Player.Group.HasPermission("essentials.sudo.invisible"))
					players[0].SendInfoMessage("{0} forced you to execute {1}{2}.", e.Player.Name, TShock.Config.CommandSpecifier, command);

				var fakePlayer = new TSPlayer(players[0].Index);
				fakePlayer.AwaitingName = players[0].AwaitingName;
				fakePlayer.AwaitingNameParameters = players[0].AwaitingNameParameters;
				fakePlayer.AwaitingTempPoint = players[0].AwaitingTempPoint;
				fakePlayer.Group = force ? new SuperAdminGroup() : players[0].Group;
				fakePlayer.TempPoints = players[0].TempPoints;

				await Task.Run(() => TShockAPI.Commands.HandleCommand(fakePlayer, TShock.Config.CommandSpecifier + command));

				players[0].AwaitingName = fakePlayer.AwaitingName;
				players[0].AwaitingNameParameters = fakePlayer.AwaitingNameParameters;
				players[0].AwaitingTempPoint = fakePlayer.AwaitingTempPoint;
				players[0].TempPoints = fakePlayer.TempPoints;
			}
		}

		public static async void TimeCmd(CommandArgs e)
		{
			var regex = new Regex(String.Format(@"^\w+(?: -(\w+))* (\w+) (?:{0})?(.+)$", TShock.Config.CommandSpecifier));
			Match match = regex.Match(e.Message);
			if (!match.Success)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}timecmd [-switches...] <time> <command...>", TShock.Config.CommandSpecifier);
				e.Player.SendSuccessMessage("Valid {0}timecmd switches:", TShock.Config.CommandSpecifier);
				e.Player.SendInfoMessage("-r, -repeat: Repeats the time command indefinitely.");
				return;
			}

			bool repeat = false;
			foreach (Capture capture in match.Groups[1].Captures)
			{
				switch (capture.Value.ToLowerInvariant())
				{
					case "r":
					case "repeat":
						repeat = true;
						break;
					default:
						e.Player.SendSuccessMessage("Valid {0}timecmd switches:", TShock.Config.CommandSpecifier);
						e.Player.SendInfoMessage("-r, -repeat: Repeats the time command indefinitely.");
						return;
				}
			}

			int seconds;
			if (!TShock.Utils.TryParseTime(match.Groups[2].Value, out seconds) || seconds <= 0)
			{
				e.Player.SendErrorMessage("Invalid time '{0}'!", match.Groups[2].Value);
				return;
			}

			if (repeat)
				e.Player.SendSuccessMessage("Queued command '{0}{1}' indefinitely. Use /cancel to cancel!", TShock.Config.CommandSpecifier, match.Groups[3].Value);
			else
				e.Player.SendSuccessMessage("Queued command '{0}{1}'. Use /cancel to cancel!", TShock.Config.CommandSpecifier, match.Groups[3].Value);
			e.Player.AddResponse("cancel", o =>
			{
				e.Player.GetPlayerInfo().Cancel();
				e.Player.SendSuccessMessage("Cancelled all time commands!");
			});

			CancellationToken token = e.Player.GetPlayerInfo().CancellationToken;
			try
			{
				await Task.Run(async () =>
				{
					do
					{
						await Task.Delay(1000 * seconds, token);
						TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.CommandSpecifier + match.Groups[3].Value);
					}
					while (repeat);
				}, token);
			}
			catch (TaskCanceledException)
			{
			}
		}

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

			PlayerInfo info = e.Player.GetPlayerInfo();
			if (info.BackHistoryCount == 0)
			{
				e.Player.SendErrorMessage("Could not teleport back!");
				return;
			}

			steps = Math.Min(steps, info.BackHistoryCount);
			e.Player.SendSuccessMessage("Teleported back {0} step{1}.", steps, steps == 1 ? "" : "s");
			Vector2 vector = info.PopBackHistory(steps);
			e.Player.Teleport(vector.X, vector.Y);
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

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport down!");
			else
			{
				if (e.Player.Group.HasPermission("essentials.tp.back"))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
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

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport left!");
			else
			{
				if (e.Player.Group.HasPermission("essentials.tp.back"))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
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

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport right!");
			else
			{
				if (e.Player.Group.HasPermission("essentials.tp.back"))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
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

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport up!");
			else
			{
				if (e.Player.Group.HasPermission("essentials.tp.back"))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y + 6);
				e.Player.SendSuccessMessage("Teleported up {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
	}
}
