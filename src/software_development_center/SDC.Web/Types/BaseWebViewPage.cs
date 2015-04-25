using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.Web.Types
{
	public class BaseWebViewPage : WebViewPage
	{
		public virtual new BasePrincipal User
		{
			get { return HttpContext.Current.User as BasePrincipal; }
		}

		public override void Execute()
		{
		}
	}

	public class BaseWebViewPage<TModel> : WebViewPage<TModel>
	{
		public virtual new BasePrincipal User
		{
			get { return HttpContext.Current.User as BasePrincipal; }
		}

		public override void Execute()
		{
		}
	}
}