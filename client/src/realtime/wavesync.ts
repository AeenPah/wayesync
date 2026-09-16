import {
  HubConnectionBuilder,
  HubConnection,
  LogLevel,
} from "@microsoft/signalr";

const API_URL = import.meta.env.VITE_API_URL;

export function createWaveSyncConnection(): HubConnection {
  return new HubConnectionBuilder()
    .withUrl(`${API_URL}/hubs/wavesync`)
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build();
}