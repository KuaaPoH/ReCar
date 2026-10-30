<!-- LOVABLE:BEGIN -->
> [!IMPORTANT]
> This project is connected to [Lovable](https://lovable.dev). Avoid rewriting
> published git history — force pushing, or rebasing/amending/squashing commits
> that are already pushed — as it rewrites history on Lovable's side and the
> user will likely lose their project history.
>
> Commits you push to the connected branch sync back to Lovable and show up in
> the editor, so keep the branch in a working state.
<!-- LOVABLE:END -->

- ReCar Host Pro is a frontend prototype for an ASP.NET Core MVC + MySQL backend: routes mirror `/CarOwner/{Controller}/{Action}`, demo data lives in `src/lib/host/mock-data.ts`, business rules in `src/lib/host/rules.ts` (tested), endpoints listed in `src/lib/host/api.ts`. Why: pages must port to Razor views and never imply data is persisted; do not add Lovable Cloud.
