# Privacy policy

Last updated: 2026-08-11

MdLight is an offline Markdown viewer. It does not collect, store, sell, or
transmit personal data.

## Data handled by the application

MdLight reads only files that the user explicitly opens, drops onto the window,
or passes on the command line. File contents and paths are kept in process
memory while the document is open. The application does not upload them or add
them to a database.

MdLight has no accounts, analytics, advertising, telemetry, automatic updater,
crash-report uploader, or background network service. Markdown images are not
downloaded automatically. HTML embedded in Markdown is displayed as plain text.

## User-requested network access

When the user explicitly clicks an `http`, `https`, or `mailto` link, MdLight
asks Windows to open that target in the system's associated application. Any
data handling after that point is governed by that application and the target
service, not by MdLight.

This program will not transfer any information to other networked systems
unless specifically requested by the user or the person installing or
operating it.

## Installation data

The installer copies application files to the selected local directory and can
register MdLight as a handler for `.md` and `.markdown` files. These settings
remain on the user's computer and can be removed by uninstalling the program or
changing Windows Default Apps settings.

## Contact

General privacy questions can be filed in the
[public issue tracker](https://github.com/rausNT/md-light/issues). Security
reports should follow the private process in [SECURITY.md](SECURITY.md).
