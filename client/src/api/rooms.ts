import { api } from "./axios";

export interface Room {
  id: string;
  name: string;
  createdAt: string;
}

export interface CreateRoomRequest {
  name: string;
}

export async function getRooms(): Promise<Room[]> {
  const { data } = await api.get<Room[]>("/api/rooms");

  return data;
}

export async function createRoom(
  request: CreateRoomRequest,
): Promise<Room> {
  const { data } = await api.post<Room>("/api/rooms", request);

  return data;
}