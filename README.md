# Discord bot template written in FSharp

- This bot opens a websocket connection to Discord server and listen the incoming evens from the stream
- You can easily implement own event handlers for each event
- Heartbeat event handling is already implemented so the connection will stay open after initial handshake

## Requirements
- .NET Core 2.2
- FAKE 5

## Build

Release build
```
dotnet publish -c Release
```
or with FAKE
```
fake build
```

## Dockerize
```
docker build -t discordbot .
```

## How to run
```
dotnet DiscordBot.dll <YOUR DISCORD TOKEN>
```
With Docker
```
docker run discordbot <YOUR DISCORD TOKEN>