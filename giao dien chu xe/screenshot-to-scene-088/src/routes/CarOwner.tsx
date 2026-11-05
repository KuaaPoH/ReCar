import { createFileRoute, Outlet } from "@tanstack/react-router";
import { HostShell } from "@/components/host/HostShell";
import { HostProvider } from "@/lib/host/store";
import { HostExtensionProvider } from "@/lib/host/extension-store";

// Mirrors Areas/CarOwner/Views/Shared/_Layout.cshtml
export const Route = createFileRoute("/CarOwner")({
  component: () => (
    <HostProvider>
      <HostExtensionProvider>
        <HostShell>
          <Outlet />
        </HostShell>
      </HostExtensionProvider>
    </HostProvider>
  ),
});
