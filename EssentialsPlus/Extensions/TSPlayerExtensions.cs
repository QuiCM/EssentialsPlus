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
		private static ConditionalWeakTable<TSPlayer, PlayerInfo> players = new ConditionalWeakTable<TSPlayer, PlayerInfo>();

		public static PlayerInfo GetPlayerInfo(this TSPlayer tsplayer)
		{
			return players.GetOrCreateValue(tsplayer);
		}
		public static bool HasPermission(this TSPlayer tsplayer, string permission)
		{
			return tsplayer != null && tsplayer.Group != null && tsplayer.Group.HasPermission(permission);
		}
	}
}
