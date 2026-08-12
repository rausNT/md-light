# Privacy policy

Last updated: 2026-08-12

MdLight is an offline Markdown viewer and editor. It does not collect, store, sell, or
transmit personal data.

## Data handled by the application

MdLight reads only files that the user explicitly opens, drops onto the window,
or passes on the command line. It writes a file only when the user selects Save
or Save As. File contents and paths are kept in process memory while the
document is open. The application does not upload them or add them to a
database.

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
also connect those extensions to Windows' built-in text preview handler so that
the Explorer Preview pane can read the selected local file. Previewing is done
locally by Windows; MdLight is not launched and no data is transmitted. The
settings remain on the user's computer and can be removed by uninstalling the
program or changing Windows Default Apps settings.

## Language preference

MdLight stores the language selected in its toolbar in the current user's local
Windows registry profile. The value contains only a language code, is not sent
anywhere, and can be removed by deleting MdLight's per-user settings.

## Contact

General privacy questions can be filed in the
[public issue tracker](https://github.com/rausNT/md-light/issues). Security
reports should follow the private process in [SECURITY.md](SECURITY.md).
