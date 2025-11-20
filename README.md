# AlreadyEdge
Added backdrop effects to Microsoft Edge browser on Windows.

(Compatible with [DWMBlurGlass](https://github.com/Maplespe/DWMBlurGlass))




[![license](https://img.shields.io/github/license/JustKanade/AlreadyEdge.svg)](https://opensource.org/licenses/MIT)
[![GitHub release](https://img.shields.io/github/release/JustKanade/AlreadyEdge.svg)](https://github.com/JustKanade/AlreadyEdge/releases/latest)
<img src="https://img.shields.io/badge/language-C%23-239120.svg"/>
<img src="https://img.shields.io/badge/.NET-9.0-512BD4.svg"/>
<img src="https://img.shields.io/github/last-commit/JustKanade/AlreadyEdge.svg"/>

This project uses [MIT license](LICENSE).



Compatible with all Chromium-based Microsoft Edge versions.

Uses the official **Desktop Window Manager (DWM) API** - `DWMWA_SYSTEMBACKDROP_TYPE` attribute for effect application.

**Requires GPU to be disabled** for transparency to work properly. This may impact browser performance but ensures the Acrylic effect is visible.

We do not modify Edge's internal rendering logic, only apply DWM effects to window handles, ensuring maximum compatibility.

Not tested with third-party Edge modifications or extensions that affect window rendering.



Acrylic effect applied to Microsoft Edge browser window.

## Material Effects

### Acrylic
> The acrylic recipe: background, blur, exclusion blend, saturation, color/tint overlay and noise.

Acrylic provides a frosted glass appearance with visible transparency and blur effect. This is the only reliable backdrop effect that works with Edge's rendering pipeline.

**Why not Mica?**
Mica and Mica Alt effects rely on subtle wallpaper sampling and render as semi-transparent materials. However, Edge's Chromium rendering engine draws non-transparent background layers that completely obscure the Mica effect, resulting in opaque solid colors instead of the intended subtle transparency.

## How to use


### Installation
1. Download the latest release from the [Releases](https://github.com/JustKanade/AlreadyEdge/releases) page.
2. Extract the archive to a location such as `C:\Program Files\AlreadyEdge`.
3. Run `AlreadyEdge.exe`.



Configuration files are stored in `%AppData%\AlreadyEdge` and can be deleted manually if needed.




## Technical Details



### Code Conventions
- **Naming**: camelCase for private fields, PascalCase for public members
- **Comments**: Concise English documentation
- **P/Invoke**: Centralized DWM API calls in `DwmManager`
- **Resource Management**: Proper disposal patterns

### How It Works
1. **Window Detection**: Uses `EnumWindows` to find Edge windows by class name `Chrome_WidgetWin_1`
2. **Effect Application**: Applies `DWMWA_SYSTEMBACKDROP_TYPE` attribute via `DwmSetWindowAttribute`
3. **Frame Extension**: Uses `DwmExtendFrameIntoClientArea` to extend effect to title bar
4. **Monitoring**: Timer-based polling to detect new Edge windows and apply effects automatically

## Known Limitations
2. **Mica Not Supported**: Mica effects render as opaque due to Edge's rendering pipeline.
4. **Edge Only**: Currently only supports Microsoft Edge browser.


**Application won't start:**
- Install .NET 9.0 Runtime
- Run as Administrator if necessary

**Effects disappear after Windows update:**
- Simply restart AlreadyEdge - no additional configuration needed

## Acknowledgments
Inspired by:
- [DWMBlurGlass](https://github.com/Maplespe/DWMBlurGlass) - For UI design and project structure inspiration



## Author
Created by **[@JustKanade](https://github.com/JustKanade)**

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Enjoy your transparent Edge **
