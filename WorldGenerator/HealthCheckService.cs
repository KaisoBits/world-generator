using System.Net;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorldGenerator;

public class HealthCheckService : BackgroundService
{
    private readonly ILogger<HealthCheckService> _logger;
    private HttpListener? _listener;

    public HealthCheckService(ILogger<HealthCheckService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:1337/");
            _listener.Start();
            
            _logger.LogInformation("Health check service started on port 1337");

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => ProcessRequest(context), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in health check service");
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            if (request.Url?.AbsolutePath == "/_health")
            {
                // Simple health check - return 200 OK
                var responseString = "{\"status\":\"healthy\",\"timestamp\":\"" + DateTime.UtcNow.ToString("O") + "\"}";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                response.StatusCode = 200;
                
                await response.OutputStream.WriteAsync(buffer);
                response.OutputStream.Close();
            }
            else
            {
                response.StatusCode = 404;
                response.OutputStream.Close();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing health check request");
        }
    }

    public override void Dispose()
    {
        _listener?.Stop();
        _listener?.Close();
        base.Dispose();
    }
}