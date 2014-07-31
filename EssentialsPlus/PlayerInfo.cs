using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EssentialsPlus.Extensions;
using TShockAPI;

namespace EssentialsPlus
{
	public class PlayerInfo
	{
		private List<Vector2> backHistory = new List<Vector2>();
		private CancellationTokenSource cancel = new CancellationTokenSource();

		public int BackHistoryCount
		{
			get { return backHistory.Count; }
		}
		public CancellationToken CancellationToken
		{
			get { return cancel.Token; }
		}
		public string LastCommand { get; set; }

		~PlayerInfo()
		{
			cancel.Cancel();
		}

		public void Cancel()
		{
			cancel.Cancel();
		}
		public Vector2 PopBackHistory(int steps)
		{
			Vector2 vector = backHistory[steps - 1];
			backHistory.RemoveRange(0, steps);
			return vector;
		}
		public void PushBackHistory(Vector2 vector)
		{
			backHistory.Insert(0, vector);
			if (backHistory.Count > EssentialsPlus.Config.BackPositionHistory)
				backHistory.RemoveAt(backHistory.Count - 1);
		}
	}
}
