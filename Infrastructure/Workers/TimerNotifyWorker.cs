using System;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Workers;

public class TimerNotifyWorker(
IServiceProvider serviceProvider
) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {


    }
}
