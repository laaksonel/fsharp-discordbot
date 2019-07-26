namespace DiscordBot.EventHandler

module HeartbeatHandler =
  open DiscordBot.Utils.Logger
  open FSharp.Control.Reactive
  open DiscordBot.Dto
  open DiscordRequestDto
  open System

  let private heartbeatTimer interval =
    new System.Timers.Timer(float interval, AutoReset = true)

  let private sendHeartbeat optionalSeqNum sendData =
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
    |> sendData

  let initHeartbeat interval seqNumberO sendData = 
    log Info "Initializing heartbeat with interval %d" interval

    let heartbeatTask = heartbeatTimer interval
    let sendWithSeqNumber seqNumber =
      sendHeartbeat seqNumber sendData

    heartbeatTask.Elapsed
    |> Observable.withLatestFrom (fun _ y -> y) seqNumberO
    |> Observable.subscribe sendWithSeqNumber
    |> ignore

    heartbeatTask.Start()

