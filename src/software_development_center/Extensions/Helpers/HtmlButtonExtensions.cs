using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Extensions.Helpers
{
	public static class HtmlButtonExtensions
	{
		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName);
			return ActionButtonInternal(buttonText, a, null);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName, Object routeValues)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, routeValues);
			return ActionButtonInternal(buttonText, a, null);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName, string controllerName)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, controllerName);
			return ActionButtonInternal(buttonText, a, null);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				RouteValueDictionary routeValues)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, routeValues);
			return ActionButtonInternal(buttonText, a, null);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				Object routeValues, Object htmlAttributes)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, routeValues);
			return ActionButtonInternal(buttonText, a, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				RouteValueDictionary routeValues, IDictionary<string, Object> htmlAttributes)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, routeValues);
			return ActionButtonInternal(buttonText, a, htmlAttributes);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				string controllerName, Object routeValues, Object htmlAttributes)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, controllerName, routeValues);
			return ActionButtonInternal(buttonText, a, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				string controllerName, RouteValueDictionary routeValues, IDictionary<string, Object> htmlAttributes)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, controllerName, routeValues);
			return ActionButtonInternal(buttonText, a, htmlAttributes);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				string controllerName, string protocol, string hostName, string fragment,
				Object routeValues, Object htmlAttributes)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, controllerName, new RouteValueDictionary(routeValues), protocol, hostName) + "#" + fragment;
			return ActionButtonInternal(buttonText, a, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				string controllerName, string protocol, string hostName, string fragment,
				RouteValueDictionary routeValues, IDictionary<string, Object> htmlAttributes)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, controllerName, routeValues, protocol, hostName) + "#" + fragment;
			return ActionButtonInternal(buttonText, a, htmlAttributes);
		}

		public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper,
				string buttonText, string actionName,
				string controller, object routeValues)
		{
			string a = (new UrlHelper(htmlHelper.ViewContext.RequestContext)).Action(actionName, controller, routeValues);
			return ActionButtonInternal(buttonText, a, null);
		}

		private static MvcHtmlString ActionButtonInternal(string buttonText,
				string url, IDictionary<string, Object> htmlAttributes)
		{
			string queryParamString = null;
			if (url.Contains("?"))
			{
				queryParamString = url.Substring(url.IndexOf("?") + 1);
				url = url.Substring(0, url.IndexOf("?"));
			}

			TagBuilder formBuilder = new TagBuilder("form");
			formBuilder.Attributes.Add("method", "get");
			formBuilder.Attributes.Add("action", url);

			TagBuilder buttonBuilder = new TagBuilder("input");
			buttonBuilder.MergeAttributes(htmlAttributes);
			buttonBuilder.MergeAttribute("type", "submit");
			buttonBuilder.MergeAttribute("value", buttonText);

			if (null != queryParamString)
			{
				string[] queryParameters = queryParamString.Split('&');

				foreach (string queryParameter in queryParameters)
				{
					string[] queryParamParts = queryParameter.Split('=');

					TagBuilder hiddenBuilder = new TagBuilder("input");
					hiddenBuilder.Attributes.Add("type", "hidden");
					hiddenBuilder.Attributes.Add("name", queryParamParts[0]);
					hiddenBuilder.Attributes.Add("value", queryParamParts[1]);

					formBuilder.InnerHtml += hiddenBuilder.ToString(TagRenderMode.SelfClosing);
				}

			}

			formBuilder.InnerHtml += buttonBuilder.ToString(TagRenderMode.SelfClosing);
			return MvcHtmlString.Create(formBuilder.ToString(TagRenderMode.Normal));
		}
	}
}