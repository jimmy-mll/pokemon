using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Server.Handlers.Authentication;
using Pokemon.Server.Network;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.ClearProviders()
	.AddSerilog(dispose: true);

builder.Services
	.AddSingleton<IMessageParser, MessageParser>()
	.AddSingleton<IMessageFactory, MessageFactory>()
	.AddSingleton<IMessageDispatcher, MessageDispatcher>()
	.AddSingleton<PokemonServer>()
	.AddSingleton<AuthenticationHandler>()
	.AddSingleton<PokemonServer>()
	.Configure<ServerOptions>(builder.Configuration.GetRequiredSection("Network"));

await builder
	.Build()
	.Services.GetRequiredService<PokemonServer>()
	.StartAsync();