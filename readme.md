# Yet Another Minecraft Server (YAMS)

YAMS is a vanilla server admin tool designed to make running a Minecraft server easier without changing the overall experience of playing Survival Multiplayer.  As it is not a wrapper, you won't have to wait for YAMS to be updated when a new Minecraft server version comes out.

## Current Status

YAMS is currently in development but can run your server for you, there is an installer in the download section.  Source code contained here should always compile but may not do what you expect it to.  Local commits are made more frequently than they are pushed to this remote.

## Planned Features

* Installer, checks pre-requisites and installs 3rd party tools if requested
* Run as windows service
* Provide web interface for controlling all servers
* Keep itself and your Minecraft server up to date
* Integrate and automate various 3rd party tools such as c10t and Minecraft Overviewer
* Store all output in a database for properly persistent logs of server activity and chat

## Repository Guide

* __Binaries__ - Contains Release versions of all projects, used for auto updating
* __Source__ - The Visual Studio 2010 Solution and sub-projects
    * __YAMS-Library__ - Core DLL that contains any functions that actually do something
    * __YAMS-Service__ - The Windows service that keeps YAMS going 24/7
    * __YAMS-Setup__ - Visual Studio setup project for creating the MSI to distribute
    * __YAMS-Tester__ - A throw-away app for checking various functions before they go into the service
    * __YAMS-Updater__ - Small app to restart the service and apply updates to core files
    * __YAMS-Web__ - Apatana project for web files

## License

Commercial use of this software is strictly prohibited without contacting the owner for permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/3.0/"><img alt="Creative Commons Licence" style="border-width:0" src="http://i.creativecommons.org/l/by-nc-sa/3.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" property="dct:title">Yet Another Minecraft Server</span> by <a xmlns:cc="http://creativecommons.org/ns#" href="https://github.com/richardbenson/YAMS" property="cc:attributionName" rel="cc:attributionURL">Richard Benson</a> is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/3.0/">Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License</a>.