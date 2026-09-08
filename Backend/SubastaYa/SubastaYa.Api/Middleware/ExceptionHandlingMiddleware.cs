using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SubastaYa.Domain.Exceptions;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (statusCode, message) = Map(ex);

                if (statusCode == HttpStatusCode.InternalServerError)
                    _logger.LogError(ex, "Error no controlado en {Path}", context.Request.Path);
                else
                    _logger.LogWarning(ex, "{StatusCode} en {Path}: {Message}", (int)statusCode, context.Request.Path, message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
            }
        }

        private static (HttpStatusCode StatusCode, string Message) Map(Exception ex) => ex switch
        {

            ConcurrencyConflictException => (HttpStatusCode.Conflict, ex.Message),
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, ex.Message),
            EmailAlreadyRegisteredException => (HttpStatusCode.Conflict, ex.Message),
            AuctionNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            CategoryNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            WalletNotFoundException => (HttpStatusCode.InternalServerError, ex.Message),

            DomainException => (HttpStatusCode.BadRequest, ex.Message),
            AppException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocurrio un error inesperado."),

            
        };


    }
}