open DiscordBot

let readToken (argv: string[]) =
    try
      Some(argv.[0])
    with
      | :? System.IndexOutOfRangeException -> None

[<EntryPoint>]
let main argv =
  match readToken argv with
  | Some token -> DiscordBot.start token
  | None -> printfn "Bot token is missing"

  0
