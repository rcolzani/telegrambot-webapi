﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.WebAPI.Application.Interfaces;
using Telegram.WebAPI.Domain.DTO;

namespace Telegram.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private IStatisticsApplication _statisticsApp;
    private readonly ILogger<TelegramBotController> _logger;
    public StatisticsController(ILogger<TelegramBotController> logger, IStatisticsApplication statisticsApp)
    {
        _statisticsApp = statisticsApp;
        _logger = logger;
    }

    [HttpGet]
    public async Task<StatisticsDto> Get()
    {
        try
        {
            var statistics = await _statisticsApp.GetStatistics();

            var quantity = new StatisticsDto(statistics.ActiveUsersQuantity,
                statistics.UsersQuantity,
                statistics.MessagesReceivedQuantity,
                statistics.MessagesSentQuantity);

            return quantity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro:");
            return new StatisticsDto(0, 0, 0, 0);
        }
    }
}
