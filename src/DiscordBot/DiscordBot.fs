namespace DiscordBot

open DiscordBot.Utils.Logger
open DiscordBot.EventHandler

module DiscordBot =
  // Keep the main thread alive with the event which never fires
  let private waitForever = 
    new Event<unit>()
    |> fun e -> Async.AwaitEvent e.Publish
    |> Async.Ignore
    |> Async.RunSynchronously

  let start botToken =
    // TODO: Bind service actions to ws events here
    let eventHandlers = []
    EventHandler.connectGateway eventHandlers botToken

    log Info "Discord bot up and ready"
    waitForever
