# TrayAppControl 
[![MIT license](https://img.shields.io/github/license/tushev/trayappcontrol)](https://github.com/tushev/trayappcontrol/blob/main/LICENSE.txt) 
![Language: C#](https://img.shields.io/badge/lang-C%23-brightgreen) ![.NET Framework: 4.7.2](https://img.shields.io/badge/.NET%20Framework-4.7.2-blueviolet)
![OS: Windows](https://img.shields.io/badge/OS-Windows-blue)
                                                                           [![CodeFactor](https://www.codefactor.io/repository/github/tushev/trayappcontrol/badge)](https://www.codefactor.io/repository/github/tushev/trayappcontrol)

A handy tool that allows to simultaneously start and stop a set of processes from system tray[^2].

**Key features:**
* Allows to **hide console windows** 🖥 👍
* Allows to control  **one** or **multiple** processes  
* Allows to use different sets of commands for **Start** and **Stop** actions.

![ui](/docs/ui.png?raw=true)

It is very handy in case you work with apps that cannot hide their console window, i.e. Apache[^4].

## ✔ Usage
* [Download](https://github.com/tushev/trayappcontrol/releases) the app, extract the archive somewhere _(or compile the code)_.
* Create an YAML file that describes your service (see [below](#example-configuration))
* Create a shortcut (in `Startup Apps` if desired) or a `.cmd` that launches the app:<br>`TrayAppControl.exe C:\full\path\to\service.yaml`

ℹ Tested on Windows 10 21H1. Not tested on other versions.

If necessary, you can launch multiple instances of TrayAppControl with different configuration files:
```cmd
TrayAppControl.exe C:\full\path\to\web-service.yaml
TrayAppControl.exe C:\full\path\to\ml-service.yaml
```

## Example configuration

The following example `.yaml` file illustrates the full range of capabilities of TrayAppControl.

It defines a problem-specific set of `nginx + Apache + MariaDB + PHP7/PHP8` used in a local dev environment.

```yaml
# This file defines a set of executables (a "service") that will be controlled by TrayAppControl

service-name: WWW Server
icon-running: C:\TrayAppControl\sample\running.ico
icon-stopped: C:\TrayAppControl\sample\stopped.ico

# if true, the service will be started immediately on TrayAppControl start
# if false, you have to manually start it via context menu
start-immediately: true

# if true, the service will be stopped when TrayAppControl app is being closed
#    (either per user request or on shutdown/reboot/logout)
# if false, the service will be left "as is"
stop-on-shutdown: true
# NOTE: do not rely on this feature, it is not guaranteed that it will work in 100% cases
# NOTE: no error messages will be displayed ever on shutdown


# these apps will be "managed" by TrayAppControl, i.e.
# on "Start" action these app will be started and their process handles will be "remembered", 
# so on "Stop" action their processes will be terminated
managed-apps:
  - command:     C:\httpd\bin\httpd.exe
    arguments:   -D php7
    workdir:     C:\httpd\bin\
    hide-window: true

  - command:     C:\httpd\bin\httpd.exe
    arguments:   -D php8
    workdir:     C:\httpd\bin\
    hide-window: true

# these commands will be executed on "Start" action "as is", without remembering process handles
on-start:
  - command:   C:\nginx\nginx.exe
    workdir:   C:\nginx
    hide-window: true
  
  - command:   C:\mysql\bin\mysqld.exe
    arguments: --skip-grant-tables --log_syslog=0
    hide-window: true

# these commands will be executed on "Stop" action "as is"
on-stop:
  - command:   C:\mysql\bin\mysqladmin.exe
    arguments: -u root shutdown
    hide-window: true
    
  - command:   C:\nginx\nginx.exe
    arguments: -s stop
    workdir:   C:\nginx
    hide-window: true
```

With this example configuration, TrayAppControl:
* On **Start service**: 
  * Launches two[^3] instances of `C:\httpd\bin\httpd.exe`, hides their windows, remembers their process handles
  * Launches `nginx` process _(without remembering its process handle)_
  * Launches `mysqld` process _(without remembering its process handle)_
* On **Stop service**: 
  * Terminates "remembered" Apache processes
  * Executes `nginx -s stop` command
  * Executes `mysqladmin -u root shutdown` command

## 🔐 Privacy

✔ This software neither collects any data nor phones home.

## ⚖ License: MIT

```
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
* [Full license text ](LICENSE.txt)
* [Acknowledgements and licenses for 3rd party components](3rdparty_licensing/3RDPARTY.txt)

[^2]: Officially known as **Taskbar Notification Area**.
[^3]: With different params (i.e. `-D php8`) to be used with the corresponding `<IfDefine>` sections of `httpd.conf`.
[^4]: While Apache can be installed as Windows service (`httpd -k install`), it is not always convenient to run it as a service.
