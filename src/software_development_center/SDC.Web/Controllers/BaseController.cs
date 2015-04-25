using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Database.Common;
using SDC.Web.Types;

namespace SDC.Web.Controllers
{
	public abstract class BaseController : Controller
	{
		protected readonly SdcDbContext db = SdcDbContext.Create();

		protected virtual new BasePrincipal User
		{
			get { return HttpContext.User as BasePrincipal; }
		}
	}
}