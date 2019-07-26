namespace FooBotUtils

module Logger =
  let Error   = 3
  let Warning = 2
  let Info    = 1
  let Debug   = 0

  let private LevelToString level =
    match level with
      | 3 -> "Error"
      | 2 -> "Warning"
      | 1 -> "Info"
      | 0 -> "Debug"
      | _ -> "Unknown"
  
  let private consoleLog level msg = 
    let print = 
      level
      |> LevelToString
      |> printfn "[%A - %s] %s" System.DateTime.Now
      |> Printf.kprintf

    print msg

  let log level message =
    consoleLog level message
