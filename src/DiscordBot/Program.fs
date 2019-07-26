open DiscordBot

let readToken (argv: string[]) =
    try
      Some(argv.[0])
    with
      | :? System.IndexOutOfRangeException -> None

[<EntryPoint>]
let main argv =
  match readToken argv with
  | Some token -> DiscordBot.startBot token
  | None -> printfn "Access token is missing"

  0
