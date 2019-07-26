namespace DiscordBot

module DiscordBot =
  open WebSocketSharp
  open DiscordBot.Utils.WebSocketClient
  open DiscordBot.Utils.Logger
  open DiscordBot.EventHandler
  open DiscordBot.Dto.DtoMapping
  open Dto.DiscordRequestDto

  // TODO: Move to configuration file
  let private discordGatewayUrl = "wss://gateway.discord.gg/"

  // Keep the main thread alive forever
  let private waitForever = 
    new Event<unit>()
    |> fun e -> Async.AwaitEvent e.Publish
    |> Async.Ignore
    |> Async.RunSynchronously

  let private identifyClient botToken sendData =
    let identityProps = {
      ``$os`` = "io.dontcare"
      ``$browser`` = "io.dontcare"
      ``$device`` = "io.dontrace"
    }

    let identityDto = {
      token = botToken
      properties = identityProps
      compress = false
    }

    let dto: RequestDto<IdentityDto> = {
      op = 2
      d = identityDto
    } 

    dtoToJson dto
    |> fun data -> log Debug "Sending %s"  data; data
    |> sendData

  let private connect () =
    let socket = initSocket discordGatewayUrl
    let sendData = fun (data: string) -> socket.Send(data)
    let onReceiveMessage  = fun (msg: MessageEventArgs) ->
      EventHandler.onReceiveMessage sendData msg.Data

    withMessageHandler socket onReceiveMessage
    |> fun s -> s.Connect()

    sendData

  let startBot botToken =
    connect ()
    |> identifyClient botToken
    |> fun _ -> waitForever
    
