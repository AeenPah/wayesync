import { api } from "./axios";

export interface HealthResponse {
  status: string;
  service: string;
}

export const getHealth = async (): Promise<HealthResponse> => {
  const { data } = await api.get<HealthResponse>("/api/health");

  return data;
};