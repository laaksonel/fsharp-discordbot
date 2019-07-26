namespace FooBotUtils

module WebSocketClient =
  open WebSocketSharp

  let withMessageHandler (client: WebSocket) messageHandler  =
    client.OnMessage.Add(messageHandler)
    client

  let initSocket url = 
      new WebSocket(url)

