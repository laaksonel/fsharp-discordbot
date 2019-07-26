namespace DiscordBot.EventHandler

module EventHandler =
  open DiscordBot.Utils.Logger
  open DiscordBot.Dto.DtoMapping
  open DiscordBot.Dto.DiscordResponseDto
  open FSharp.Control.Reactive

  let private seqNumberO = Subject.replay

  let private handleEvent (data: DtoData) sendData =
    match data with
    | H heartbeat ->
      HeartbeatHandler.initHeartbeat
        heartbeat.heartbeat_interval
        seqNumberO
        sendData
    // Add other event handlers here
    | _ -> ()

  let private updateSeqNumber =
    fun x -> seqNumberO.OnNext x

  let onReceiveMessage sendData =
    fun rawData ->
      log Debug "Received message: %s" rawData
      match jsonToDto rawData with
      | Ok (dto, seqNumber) ->
        updateSeqNumber seqNumber
        handleEvent dto sendData
      | Result.Error e ->
        log Error "Failed to read message, skipping actions, %s" e.Message
