namespace FooBot

open WebSocketSharp
open FooBotUtils.WebSocketClient

module EventHandler =
  open FooBotUtils.Logger
  open Dto.DtoMapping
  open Dto.DiscordResponseDto
  open Dto.DiscordRequestDto
  open HearbeatEventHandler // TODO: Remove this, inject instead
  open FSharp.Control.Reactive

  let private discordGatewayUrl = "wss://gateway.discord.gg/"

  let private seqNumberO = Subject.replay

  let private handleEvent (data: DtoData) eventHandlers socket =
    // TODO: Call methods from eventHandlers
    match data with
    | H heartbeat -> initHeartbeat heartbeat.heartbeat_interval seqNumberO socket
    | _ -> ()

  let private readEvent processMessage (msg: MessageEventArgs) =
    log Debug "%s" msg.Data

    let updateSeqNumber = fun x -> seqNumberO.OnNext x

    match jsonToDto msg.Data updateSeqNumber with
    | Ok dto -> processMessage dto
    | Result.Error e -> log Error "Failed to read message, skipping actions, %s" e.Message

    ()

  let private identifyClient botToken (socket: WebSocket) =
    let identityProps = {
        ``$os`` = "FOO"
        ``$browser`` = "FOO"
        ``$device`` = "FOO"
        ``$referrer`` = ""
        ``$referring_domain`` = ""
      }

    let identityDto = {
      token = botToken
      properties = identityProps
      compress = false
    }

    let dto: RequestDto<IdentityDto> =
      { op = 2; d = identityDto } 

    dtoToJson dto
    |> fun data -> log Debug "Sending %s"  data; data
    |> socket.Send

  let connectGateway eventHandlers token = 
    log Info "Connecting to discord gateway..."

    let processMessage =
      fun socket ->
        fun dto ->
          handleEvent dto eventHandlers socket

    let socket =
      initSocket discordGatewayUrl
      |> fun socket ->
        processMessage socket
        |> readEvent
        |> withMessageHandler socket
    
    socket.Connect()
    identifyClient token socket


