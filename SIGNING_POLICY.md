# Code signing policy

MdLight uses code signing to let users verify that released Windows binaries
were produced from this repository and were not modified after publication.

Free code signing provided by [SignPath.io](https://signpath.io/), certificate
by [SignPath Foundation](https://signpath.org/).

## Signed artifacts

The release signing scope covers:

- `MdLight.exe`;
- `MdLight-Setup.exe`, including its uninstaller;
- executable files contained in the portable release archive.

Archives, documentation, and source-code files are not executable and do not
require an Authenticode signature.

## Build and signing process

1. Source code, installer definitions, and CI configuration are stored in the
   public [rausNT/md-light](https://github.com/rausNT/md-light) repository.
2. Release artifacts are built only by GitHub-hosted runners from the default
   branch or an annotated version tag.
3. The unsigned workflow artifact is submitted to SignPath through its GitHub
   trusted-build-system integration with origin verification enabled.
4. SignPath verifies the artifact origin, applies the approved signing policy,
   signs the configured executables, and returns a signed artifact.
5. After SignPath Foundation onboarding, only the signed artifact is attached
   to a public GitHub release.

An initial pre-release may be explicitly labeled as unsigned when SignPath
Foundation needs an already released artifact to evaluate the project. Such an
artifact is an onboarding candidate only and is never represented as signed.

Signing credentials and private keys are never stored in this repository or
made available to the build process.

## Team roles

- Committers and reviewers: [rausNT](https://github.com/rausNT)
- Approvers: [rausNT](https://github.com/rausNT)

Changes submitted by people who are not committers must be reviewed by a
maintainer before they are merged. Changes to build, release, or signing files
receive the same review as application source code.

## Verification

On Windows, a downloaded executable can be checked with:

```powershell
Get-AuthenticodeSignature .\MdLight-Setup.exe | Format-List Status,SignerCertificate,TimeStamperCertificate
```

The expected status for a signed release is `Valid`. The signer displayed by
Windows will be the identity supplied by SignPath Foundation.

## Privacy

See the [privacy policy](PRIVACY.md). This program will not transfer any
information to other networked systems unless specifically requested by the
user or the person installing or operating it.
