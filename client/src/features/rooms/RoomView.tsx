import { useEffect, useState } from "react";
import { HubConnection, HubConnectionState } from "@microsoft/signalr";

import { createWaveSyncConnection } from "../../realtime/wavesync";
import { getParticipantId } from "../../lib/participant";

interface Participant {
  id: string;
  connectionId: string;
}

interface RoomState {
  roomId: string;
  participants: Participant[];
}

interface RoomViewProps {
  roomId: string;
}

export function RoomView({ roomId }: RoomViewProps) {
  const participantId = getParticipantId();

  const [connection, setConnection] = useState<HubConnection | null>(null);

  const [participants, setParticipants] = useState<Participant[]>([]);

  useEffect(() => {
    const hubConnection = createWaveSyncConnection();

    hubConnection.on("RoomState", (state: RoomState) => {
      setParticipants(state.participants);
    });

    hubConnection.on("ParticipantJoined", (participant: Participant) => {
      setParticipants((current) => {
        if (current.some((item) => item.id === participant.id)) {
          return current;
        }

        return [...current, participant];
      });
    });

    hubConnection.on("ParticipantLeft", (participantId: string) => {
      setParticipants((current) =>
        current.filter((participant) => participant.id !== participantId),
      );
    });

    const start = async () => {
      try {
        await hubConnection.start();

        await hubConnection.invoke("JoinRoom", roomId, participantId);

        setConnection(hubConnection);
      } catch (error) {
        console.error("Failed to connect to WaveSync:", error);
      }
    };

    start();

    return () => {
      const stop = async () => {
        if (hubConnection.state === HubConnectionState.Connected) {
          try {
            await hubConnection.invoke("LeaveRoom", roomId, participantId);
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

      <p>Status: {connection ? "Connected" : "Connecting..."}</p>

      <h3>Participants ({participants.length})</h3>

      {participants.length === 0 ? (
        <p>No participants.</p>
      ) : (
        <ul>
          {participants.map((participant) => (
            <li key={participant.id}>
              {participant.id} {participant.connectionId}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
