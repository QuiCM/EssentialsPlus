using System.Runtime.CompilerServices;
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
	}
}
