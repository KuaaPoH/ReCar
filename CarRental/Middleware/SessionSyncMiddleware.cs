using CarRental.Utilities;
using Microsoft.AspNetCore.Http;

namespace CarRental.Middleware
{
    public class SessionSyncMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionSyncMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Đồng bộ Session với static variables mỗi request
            if (context.Session != null)
            {
                Function._AccountId = Function.GetAccountId(context.Session);
                Function._CarOwnerAccountId = Function.GetCarOwnerAccountId(context.Session);
                Function._UserName = Function.GetUserName(context.Session);
                Function._Email = Function.GetEmail(context.Session);
                Function._address = Function.GetAddress(context.Session);
                Function._Phone = Function.GetPhone(context.Session);
                Function._Avatar = Function.GetAvatar(context.Session);
                Function._Message = Function.GetMessage(context.Session);
            }

            await _next(context);
        }
    }
}

