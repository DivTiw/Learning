using Common_Components;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Task_Tracker_CommonLibrary.Utility;

namespace Task_tracker_WebAPI.Filters
{
    public class StrideAPIAuthValidator : AuthorizeAttribute
    {
        TokenServer ts = null;
        public StrideAPIAuthValidator()
        {
            ts = new TokenServer();
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Authorize(actionContext))
            {
                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
        private bool Authorize(HttpActionContext actionContext)
        {
            bool isAuthorized = false;
            try
            {
                if (actionContext.Request.Headers.Contains("Token"))
                {
                    string _token = actionContext.Request.Headers.GetValues("Token").First();
                    if (ts.ValidateToken(_token))
                    {
                        isAuthorized = true;
                    }
                    else
                    {
                        Log.LogError("Invalid Token. Token is: "+_token, "", null, "Authorize", "StrideAPIAuthValidator");
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogError(e.Message, "", null, "Authorize", "StrideAPIAuthValidator");
            }
            return isAuthorized;
        }
    }
}