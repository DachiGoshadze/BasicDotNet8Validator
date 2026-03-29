using Microsoft.AspNetCore.Builder;

namespace BasicValidatorLibrary;
public static class MiddlewareInjection
{
    public static void UseBasicValidatorMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidatorMiddleware>();
    }
}