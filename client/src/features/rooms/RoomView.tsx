import { useEffect, useState } from "react";
import {
  HubConnection,
  HubConnectionState,
} from "@microsoft/signalr";

import { createWaveSyncConnection } from "../../realtime/wavesync";

interface RoomState {
  roomId: string;
  participants: string[];
}

interface RoomViewProps {
  roomId: string;
}

export function RoomView({ roomId }: RoomViewProps) {
  const [connection, setConnection] =
    useState<HubConnection | null>(null);

  const [participants, setParticipants] = useState<string[]>([]);

  useEffect(() => {
    const hubConnection = createWaveSyncConnection();

    hubConnection.on(
      "RoomState",
      (state: RoomState) => {
        setParticipants(state.participants);
      },
    );

    hubConnection.on(
      "ParticipantJoined",
      (connectionId: string) => {
        setParticipants((current) => {
          if (current.includes(connectionId)) {
            return current;
          }

          return [...current, connectionId];
        });
      },
    );

    hubConnection.on(
      "ParticipantLeft",
      (connectionId: string) => {
        setParticipants((current) =>
          current.filter((id) => id !== connectionId),
        );
      },
    );

    const start = async () => {
      try {
        await hubConnection.start();

        await hubConnection.invoke(
          "JoinRoom",
          roomId,
        );

        setConnection(hubConnection);
      } catch (error) {
        console.error(
          "Failed to connect to WaveSync:",
          error,
        );
      }
    };

    start();

    return () => {
      const stop = async () => {
        if (
          hubConnection.state ===
          HubConnectionState.Connected
        ) {
          try {
            await hubConnection.invoke(
              "LeaveRoom",
              roomId,
            );
          } catch {
            // Connection may already be closed.
          }
        }

        await hubConnection.stop();
      };

      stop();
    };
  }, [roomId]);

  return (
    <section>
      <h2>Room</h2>

      <p>
        Room ID:
        <br />
        <code>{roomId}</code>
      </p>

      <p>
        Status:{" "}
        {connection ? "Connected" : "Connecting..."}
      </p>

      <h3>
        Participants ({participants.length})
      </h3>

      {participants.length === 0 ? (
        <p>No participants.</p>
      ) : (
        <ul>
          {participants.map((participant) => (
            <li key={participant}>
              {participant}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}