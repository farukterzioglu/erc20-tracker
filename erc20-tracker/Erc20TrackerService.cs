using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace erc20_tracker
{
    public class Erc20TrackerService : BackgroundService
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly Settings _settings;

        public Erc20TrackerService(
            IApplicationLifetime applicationLifetime,
            IOptions<Settings> settings)
        {
            _settings = (settings ?? throw new ArgumentNullException(nameof(applicationLifetime))).Value;
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_applicationLifetime.ApplicationStopping.IsCancellationRequested)
            {

                await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
            }
        }
    }
}