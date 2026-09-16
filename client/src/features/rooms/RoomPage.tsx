import {  useState, type SubmitEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { createRoom, getRooms } from "../../api/rooms";

export function RoomPage() {
  const [roomName, setRoomName] = useState("");

  const queryClient = useQueryClient();
  
  const roomsQuery = useQuery({
    queryKey: ["rooms"],
    queryFn: getRooms,
  });

  const createRoomMutation = useMutation({
  mutationFn: createRoom,
  onSuccess: () => {
    queryClient.invalidateQueries({
      queryKey: ["rooms"],
    });

    setRoomName("");
  },
});

  function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();

    const name = roomName.trim();

    if (!name) {
      return;
    }

    createRoomMutation.mutate({
      name,
    });
  }

  return (
    <main>
      <h1>WaveSync</h1>

      <section>
        <h2>Create a room</h2>

        <form onSubmit={(e)=> handleSubmit(e)}>
          <input
            type="text"
            placeholder="Room name"
            value={roomName}
            onChange={(event) => setRoomName(event.target.value)}
          />

          <button
            type="submit"
            disabled={createRoomMutation.isPending}
          >
            {createRoomMutation.isPending
              ? "Creating..."
              : "Create room"}
          </button>
        </form>
      </section>

      <section>
        <h2>Rooms</h2>

        {roomsQuery.isLoading && <p>Loading rooms...</p>}

        {roomsQuery.isError && (
          <p>Failed to load rooms.</p>
        )}

        {roomsQuery.data?.map((room) => (
          <div key={room.id}>
            <strong>{room.name}</strong>

            <small>
              <br />
              {room.id}
            </small>
          </div>
        ))}
      </section>
    </main>
  );
}