# Open Lenovo Settings

Open Lenovo Settings is a utility created for Lenovo consumer (IdeaPad, Yoga, XiaoXin) laptops, that allows changing a couple of features that are only available in Lenovo Vantage or Lenovo PC Manager.

> ### ⚠️ **Disclaimer**
> **This tool comes with no warranty. Use at your own risk.**

## Compatibility

Most IdeaPad/Yoga/XiaoXin-branded models manufactured after 2020 should work fine. Unsupported features will be automatically hidden.

Limited support for ~~gayming~~ Legion-branded models, check out [Lenovo Legion Toolkit](https://github.com/BartoszCichecki/LenovoLegionToolkit) for more features.

For ThinkPad-branded models, search elsewhere 😜

### Lenovo softwares

It is recommended to uninstall Lenovo Vantage and disable/uninstall Lenovo System Interface Foundation Service.

## Features

* Battery charging mode*
* Always-on USB toggle
* Performance mode
* Built-in keyboard contrl (Fn lock, backlight*)
* Lenovo Utility service fine-tune
  - Caps Lock OSD control
  - Cortana shortcut (Fn-Ctrl) control

\* For settings that will lost after reboot or abnormal shutdown, an extra "apply on startup" option is available.

## But I'm using Linux!

Check `/sys/bus/platform/drivers/ideapad_acpi/VPC2004:00`, `/sys/firmware/acpi/platform_profile_choices` and `/sys/firmware/acpi/platform_profile`.

## Credits

* [Lenovo Controller](https://github.com/ViRb3/LenovoController)
* [Lenovo Legion Toolkit](https://github.com/BartoszCichecki/LenovoLegionToolkit)
* [LogoDIY](https://github.com/Coxxs/LogoDiy)
* App icon by <a href="https://www.flaticon.com/free-icons/business-and-finance" title="business and finance icons">manshagraphics on Flaticon</a>
