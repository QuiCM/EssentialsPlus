using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EssentialsPlus.Extensions;
using TShockAPI;

namespace EssentialsPlus
{
	public class Player
	{
		private List<Vector2> backPoints = new List<Vector2>();

		public ReadOnlyCollection<Vector2> BackPoints
		{
			get { return backPoints.AsReadOnly(); }
		}
		public int Index
		{
			get { return TSPlayer.Index; }
		}
		public Terraria.Player TPlayer
		{
			get { return TSPlayer.TPlayer; }
		}
		public TSPlayer TSPlayer { get; private set; }

		public Player(TSPlayer tsplayer)
		{
			TSPlayer = tsplayer;
		}

		public void AddBackPoint(Vector2 vector)
		{
			backPoints.Insert(0, vector);
			if (backPoints.Count > EssentialsPlus.Config.BackPointHistory)
				backPoints.RemoveAt(backPoints.Count - 1);
		}
		public bool HasPermission(string permission)
		{
			return TSPlayer != null && TSPlayer.HasPermission(permission);
		}
		public void SendErrorMessage(string format, params object[] args)
		{
			if (TSPlayer != null)
				TSPlayer.SendErrorMessage(format, args);
		}
		public void SendInfoMessage(string format, params object[] args)
		{
			if (TSPlayer != null)
				TSPlayer.SendInfoMessage(format, args);
		}
		public void SendSuccessMessage(string format, params object[] args)
		{
			if (TSPlayer != null)
				TSPlayer.SendSuccessMessage(format, args);
		}
		public void SendWarningMessage(string format, params object[] args)
		{
			if (TSPlayer != null)
				TSPlayer.SendWarningMessage(format, args);
		}
		public void Teleport(Vector2 vector)
		{
			if (TSPlayer != null)
				TSPlayer.Teleport(vector.X, vector.Y);
		}
	}
}
