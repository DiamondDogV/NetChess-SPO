using Chess.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Chess.Service.Implementations
{
    public class RoomCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public RoomCleanupService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var svc = scope.ServiceProvider.GetRequiredService<IActiveRoomService>();
                    await svc.CleanupStaleAsync(TimeSpan.FromSeconds(30));
                }
                catch { }
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
