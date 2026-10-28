import { createFileRoute, Outlet } from "@tanstack/react-router";
import { HostShell } from "@/components/host/HostShell";
import { HostProvider } from "@/lib/host/store";

// Mirrors Areas/CarOwner/Views/Shared/_Layout.cshtml
export const Route = createFileRoute("/CarOwner")({
  component: () => (
    <HostProvider>
      <HostShell>
        <Outlet />
      </HostShell>
    </HostProvider>
  ),
});
