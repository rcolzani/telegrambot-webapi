using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace server.Middlewares
{
    public class MiddlewareTeste
    {
        private readonly RequestDelegate _next;

        public MiddlewareTeste(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("\n -------- Antes do Middleware ---------- \n");
            await _next(context);
            Console.WriteLine("\n -------- Depois do Middleware ---------- \n");
        }
    }
}