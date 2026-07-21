namespace MarketPulse.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseMarketPulse(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("Frontend");

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health");
        app.MapHealthChecks("/ready");

        return app;
    }
}