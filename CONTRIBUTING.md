# Contributing to MdLight

Thank you for helping improve MdLight.

1. Fork the repository and create a focused branch.
2. Keep changes small and do not add telemetry, advertising, remote-content
   loading, or proprietary runtime dependencies.
3. Build with `dotnet build MdLight.sln -c Release` and run
   `src\MdLight\bin\Release\net48\MdLight.exe --smoke-test`.
4. Open a pull request describing the change and its validation.

Changes from contributors who are not committers require maintainer review.
Build scripts, release workflows, installer definitions, and signing policy
files are reviewed with the same care as application code.

Report security issues privately as described in [SECURITY.md](SECURITY.md).
By contributing, you agree that your contribution is provided under the
project's [MIT License](LICENSE).
