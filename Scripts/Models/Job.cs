using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStationBuilder
{
	/// <summary>
	/// The data class for a queued-up job. This could include things like 
	/// placing furniture, construction walls, fighting enemies, moving
	/// around inventory, eating, sleeping, etc.
	/// </summary>
	public class Job
	{
		public Tile Tile { get; protected set; }
		float jobTime;

		public event Action<Job> cbJobComplete;
		public event Action<Job> cbJobCancelled;

		public Job(Tile tile, Action<Job> cbJobComplete, float jobTime = 1f)
		{
			Tile = tile;
			this.cbJobComplete += cbJobComplete;
			this.jobTime = jobTime;
		}

		public void DoWork(float workTime)
		{
			jobTime -= workTime;
			if (jobTime <= 0)
			{
				cbJobComplete?.Invoke(this);
			}
		}

		public void CancelJob()
		{
			cbJobCancelled?.Invoke(this);
		}
	}
}
