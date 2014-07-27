using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace EssentialsPlus.Extensions
{
	public static class TSPlayerExtensions
	{
		private static ConditionalWeakTable<TSPlayer, Player> players = new ConditionalWeakTable<TSPlayer, Player>();

		public static Player GetEssentials(this TSPlayer tsplayer)
		{
			return players.GetValue(tsplayer, tsp => new Player(tsp));
		}
		public static bool HasPermission(this TSPlayer tsplayer, string permission)
		{
			return tsplayer != null && tsplayer.Group != null && tsplayer.Group.HasPermission(permission);
		}
	}
}
