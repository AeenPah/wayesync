import { useState, type SubmitEvent } from "react";

interface JoinRoomPageProps {
  onJoin: (roomId: string) => void;
}

export function JoinRoomPage({ onJoin }: JoinRoomPageProps) {
  const [roomId, setRoomId] = useState("");

  function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();

    const id = roomId.trim();

    if (!id) {
      return;
    }

    onJoin(id);
  }

  return (
    <main>
      <h1>WaveSync</h1>

      <h2>Join a room</h2>

      <form onSubmit={handleSubmit}>
        <input
          value={roomId}
          onChange={(event) => setRoomId(event.target.value)}
          placeholder="Enter room ID"
        />

        <button type="submit">Join room</button>
      </form>
    </main>
  );
}
