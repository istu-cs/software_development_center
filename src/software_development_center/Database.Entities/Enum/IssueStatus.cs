using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities.Enum
{
	public enum IssueStatus
	{
		Unoccupied,
		Assigned,
		InProcess,
		PartiallyDone,
		Done,
		ToVerify
	}
}