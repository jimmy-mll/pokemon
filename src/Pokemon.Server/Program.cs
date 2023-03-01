using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Protocol.Messages.Authentication;
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

var app = builder.Build();

var messageFactory = app.Services.GetRequiredService<IMessageFactory>();
var messageDispatcher = app.Services.GetRequiredService<IMessageDispatcher>();

messageFactory.Initialize(typeof(IdentificationSuccessMessage).Assembly);
messageDispatcher.InitializeServer(typeof(Program).Assembly);

await app.Services
	.GetRequiredService<PokemonServer>()
	.StartAsync();