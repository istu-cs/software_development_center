using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities.Enum
{
	public enum TeamProgressStatus
	{
		Unoccupied = 0,
		Assigned = 1,
		InProgress = 2,
		PartiallyDone = 3,
		Done = 4,
		ToVerify = 5
	}
}