# Releasing FluentTelegramUI

## Version bump

1. Update `Version` in `FluentTelegramUI/FluentTelegramUI.csproj`
2. Add a section to `CHANGELOG.md`
3. Commit on `main`

## NuGet publish (automated)

The [Release workflow](../.github/workflows/release.yml) runs on tags matching `v*.*.*`.

### One-time setup

1. Create a NuGet API key at [nuget.org](https://www.nuget.org/account/apikeys)
2. Add repository secret **`NUGET_API_KEY`** in GitHub → Settings → Secrets and variables → Actions

### Publish

```bash
git tag v0.2.0
git push origin v0.2.0
```

The workflow will build, test, pack, push to NuGet, and create a GitHub Release with the `.nupkg` attached.

### Local pack (verify before tagging)

```bash
dotnet pack FluentTelegramUI/FluentTelegramUI.csproj -c Release
ls FluentTelegramUI/bin/Release/*.nupkg
```

## Documentation site (GitHub Pages)

The [Docs workflow](../.github/workflows/docs.yml) validates Jekyll builds on every push and PR.

### One-time setup

1. GitHub → **Settings** → **Pages** → Source: **GitHub Actions**
2. Run **Docs** workflow manually via **Actions → Docs → Run workflow** (deploy job runs only on `workflow_dispatch`)
3. Site URL: `https://golovin-igor.github.io/fluent-telegram-ui/`

Ensure `docs/_config.yml` `baseurl` matches the repository name.

## CI

Every push and PR runs build, tests (with coverage artifact), sample builds, and pack validation via [ci.yml](../.github/workflows/ci.yml).
