using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace EssentialsPlus.Extensions
{
	public static class TSPlayerExtensions
	{
		private static Dictionary<TSPlayer, Player> players = new Dictionary<TSPlayer, Player>();

		public static void AttachEssentialsPlayer(this TSPlayer tsplayer)
		{
			if (!players.ContainsKey(tsplayer))
				players.Add(tsplayer, new Player(tsplayer));
		}
		public static void DetachEssentialsPlayer(this TSPlayer tsplayer)
		{
			players.Remove(tsplayer);
		}
		public static Player GetEssentialsPlayer(this TSPlayer tsplayer)
		{
			if (!players.ContainsKey(tsplayer))
				return null;
			return players[tsplayer];
		}
		public static bool HasPermission(this TSPlayer tsplayer, string permission)
		{
			return tsplayer.Group != null && tsplayer.Group.HasPermission(permission);
		}
	}
}
