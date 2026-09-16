const PARTICIPANT_ID_KEY = "wavesync-participant-id";

export function getParticipantId(): string {
  const existingId = localStorage.getItem(PARTICIPANT_ID_KEY);

  if (existingId) {
    return existingId;
  }

  const newId = crypto.randomUUID();

  localStorage.setItem(PARTICIPANT_ID_KEY, newId);

  return newId;
}
