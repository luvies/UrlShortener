using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace UrlShortener
{
    /// <summary>
    /// Flags an Action Method valid for any incoming request only if all, any or none of the given HTTP parameter(s) are set,
    /// enabling the use of multiple Action Methods with the same name (and different signatures) within the same MVC Controller.
    /// </summary>
    /// <remarks>
    /// Derived from
    /// https://www.ryadel.com/en/asp-net-mvc-fix-ambiguous-action-methods-errors-multiple-action-methods-action-name-c-sharp-core/
    /// </remarks>
    public class RequireParameterAttribute : ActionMethodSelectorAttribute
    {
        public RequireParameterAttribute(string parameterName) : this(new[] { parameterName })
        {
        }

        public RequireParameterAttribute(params string[] parameterNames)
        {
            ParameterNames = parameterNames;
            IncludeGET = true;
            IncludePOST = true;
            IncludeCookies = false;
            Mode = MatchMode.All;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var req = routeContext.HttpContext.Request;
            bool getPredicate(string p) => req.Query.Select(q => q.Key).Contains(p);
            bool postPredicate(string p) => req.HasFormContentType && req.Form.Select(f => f.Key).Contains(p);
            bool cookiePredicate(string p) => req.Cookies.Select(c => c.Key).Contains(p);
            switch (Mode)
            {
                default:
                    return (
                        (IncludeGET && ParameterNames.All(getPredicate))
                        || (IncludePOST && ParameterNames.All(postPredicate))
                        || (IncludeCookies && ParameterNames.All(cookiePredicate))
                        );
                case MatchMode.Any:
                    return (
                        (IncludeGET && ParameterNames.Any(getPredicate))
                        || (IncludePOST && ParameterNames.Any(postPredicate))
                        || (IncludeCookies && ParameterNames.Any(cookiePredicate))
                        );
                case MatchMode.None:
                    return (
                        (!IncludeGET || !ParameterNames.Any(getPredicate))
                        && (!IncludePOST || !ParameterNames.Any(postPredicate))
                        && (!IncludeCookies || !ParameterNames.Any(cookiePredicate))
                        );
            }
        }

        public string[] ParameterNames { get; private set; }

        /// <summary>
        /// Set it to TRUE to include GET (QueryStirng) parameters, FALSE to exclude them:
        /// default is TRUE.
        /// </summary>
        public bool IncludeGET { get; set; }

        /// <summary>
        /// Set it to TRUE to include POST (Form) parameters, FALSE to exclude them:
        /// default is TRUE.
        /// </summary>
        public bool IncludePOST { get; set; }

        /// <summary>
        /// Set it to TRUE to include parameters from Cookies, FALSE to exclude them:
        /// default is FALSE.
        /// </summary>
        public bool IncludeCookies { get; set; }

        /// <summary>
        /// Use MatchMode.All to invalidate the method unless all the given parameters are set (default).
        /// Use MatchMode.Any to invalidate the method unless any of the given parameters is set.
        /// Use MatchMode.None to invalidate the method unless none of the given parameters is set.
        /// </summary>
        public MatchMode Mode { get; set; }

        /// <summary>
        /// The match modes that are available.
        /// </summary>
        public enum MatchMode
        {
            /// <summary>
            /// Only valid if all given parameters are set.
            /// </summary>
            All,
            /// <summary>
            /// Only valid if any of the given are set.
            /// </summary>
            Any,
            /// <summary>
            /// Only valid if none of the given parameters are set.
            /// </summary>
            None
        }
    }
}
