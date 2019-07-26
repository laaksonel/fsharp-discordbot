namespace DiscordBot.Dto

module DiscordResponseDto =
  open System
  // Wrapper for data field so we can deserialize json properly
  type ResponseData<'a> = {
    d: 'a
  }

  type HeartbeatAckData = {
    d: obj
  }

  type HandshakeData = {
    heartbeat_interval: int
  }

  type Channel = {
    id: int64
    name: string
  }

  type GuildCreateData = {
    channels: Channel[]
  }

  // All the response types
  type DtoData =
  | H of HandshakeData
  | A of HeartbeatAckData
  | G of GuildCreateData

  // Every response contains the metadata fields
  type DiscordMetaData = {
    t: string
    s: Nullable<int>
    op: int
  }

module DiscordRequestDto =
  open System

  type RequestDto<'T> = {
    op: int
    d: 'T
  }

  type IdentityConstants = {
    ``$os``: string
    ``$browser``: string
    ``$device``: string
  }

  type IdentityDto = {
    token: string
    properties: IdentityConstants
    compress: bool
  }

  type HeartbeatDto = RequestDto<Nullable<int>>

module DtoMapping =
  open DiscordResponseDto
  open DiscordBot.Utils.Json

  let private mapToDto<'a> dtoConstructor (data: ResponseData<'a>) =
    data.d
    |> dtoConstructor
    |> Ok

  let private toDto<'T> dtoConstructor json =
    let dtoMapper = mapToDto<'T> dtoConstructor

    json
    |> deserialize<ResponseData<'T>>
    |> Result.bind dtoMapper
    
  let private readPayload json metaData =
    let extractData =
      match metaData.t with
      | "GUILD_CREATE" -> toDto<GuildCreateData> G
      | _ when metaData.op = 10 -> toDto<HandshakeData> H
      | _ when metaData.op = 11 -> toDto<HeartbeatAckData> A
      | _ -> fun _ -> Error (exn "Unknown message type")

    extractData json

  let jsonToDto json =
    let readMetadata json = 
      deserialize<DiscordMetaData> json

    match readMetadata json with
    | Ok metadata ->
      readPayload json metadata
      |> Result.map(fun dto -> (dto, metadata.s))
    | Error ex -> Error ex

  let dtoToJson dto = serialize dto
