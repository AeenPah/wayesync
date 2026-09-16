import { useState } from "react";

import { JoinRoomPage } from "./features/rooms/JoinRoomPage";
import { RoomView } from "./features/rooms/RoomView";

function App() {
  const [roomId, setRoomId] = useState<string | null>(null);

  if (roomId) {
    return <RoomView roomId={roomId} />;
  }

  return <JoinRoomPage onJoin={setRoomId} />;
}

export default App;
