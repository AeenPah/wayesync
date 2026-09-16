import { useQuery } from "@tanstack/react-query";
import { getHealth } from "./api/health";

function App() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["health"],
    queryFn: getHealth,
  });

  if (isLoading) {
    return <p>Checking API...</p>;
  }

  if (isError) {
    return <p>API connection failed</p>;
  }

  return (
    <main>
      <h1>WaveSync</h1>
      <p>
        {data?.service}: {data?.status}
      </p>
    </main>
  );
}

export default App;