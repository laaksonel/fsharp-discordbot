namespace DiscordBot.EventHandler

module HearbeatEventHandler =
  open DiscordBot.Utils.Logger
  open FSharp.Control.Reactive
  open Dto
  open DiscordRequestDto
  open WebSocketSharp // TODO: Get rid of web socket dependencies in event handlers, hide behind function
  open System

  let private heartbeatTimer interval =
    new System.Timers.Timer(float interval, AutoReset = true)

  let private sendHeartbeat optionalSeqNum (socket: WebSocket) =
    let seqNumber =
      Option.ofNullable optionalSeqNum
      |> Option.map (fun x -> Nullable<int>(x))
      |> Option.defaultValue(Nullable<int>())

    let logOutgoing data =
      log Info "Sending data %s" (data)
      data

    seqNumber
    |> fun seqNum -> { op = 1; d = seqNum }
    |> DtoMapping.dtoToJson
    |> logOutgoing
    |> socket.Send

  let initHeartbeat interval seqNumberO socket = 
    log Info "Initializing heartbeat with interval %d" interval

    let heartbeatTask = heartbeatTimer interval
    let sendWithSeqNumber seqNumber =
      sendHeartbeat seqNumber socket

    heartbeatTask.Elapsed
    |> Observable.withLatestFrom (fun _ y -> y) seqNumberO
    |> Observable.subscribe sendWithSeqNumber
    |> ignore

    heartbeatTask.Start()

