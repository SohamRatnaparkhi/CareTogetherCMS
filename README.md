# CareTogetherCMS
CareTogether is an open-source case management system (CMS) for nonprofits connecting families to caring communities. [The wiki provides much more detail about the design of CareTogether.](https://github.com/CareTogether/CareTogetherCMS/wiki) If you prefer to jump straight into the code, keep reading!

![License](https://badgen.net/github/license/CareTogether/CareTogetherCMS)
![Alpha Release Project](https://badgen.net/github/milestones/CareTogether/CareTogetherCMS/1)
![Open Help-Wanted Issues](https://badgen.net/github/label-issues/CareTogether/CareTogetherCMS/help%20wanted/open)

## Prerequisites
You can build and run CareTogether on any operating system supported by Node.js and .NET Core, including Windows, MacOS, and supported Linux distros. CareTogether requires currently supported versions of Node.js and the .NET 6 SDK to be installed on your system.

You will also need to install [the Azurite emulator for Azure Storage](https://github.com/Azure/Azurite) and [the Yarn package manager](https://yarnpkg.com/getting-started/install):
```
npm install -g azurite yarn
```

Finally, to run locally you will need a set of environment configuration files. This may be fixed in the future but for now, please contact [Lars Kemmann](https://github.com/LarsKemmann) to obtain the required information.

## Development
1. Clone the repository into any local directory on your device, e.g. `D:\Code\CareTogetherCMS`.
2. Start Azurite from the command line with the `--loose` flag:
   ```
   azurite-blob --loose
   ```
   This will run Azurite with the default Blob service endpoint (we don't use the Queue or Table storage endpoints currently). Add a `--silent` parameter if you don't want to see individual requests logged to the terminal. The `--loose` parameter is currently required to support valet key access from the browser.
3. Run the _CareTogether.Api_ project:
   a. If using **Visual Studio**, you can open the _CareTogetherCMS.sln_ solution and hit F5 to start debugging.
   b. If using **Visual Studio Code**, you may first need to perform a `dotnet build` before you can debug.
   c. If you just want to run the API **from the command line**, run the following from the _CareTogether.Api_ directory:
   ```
   dotnet run SolutionDir=..
   ```
4. To run the _caretogether-pwa_ web application, run the following from the _caretogether-pwa_ directory:
   ```
   yarn install
   yarn start
   ```

## Contributing
Thank you for your interest in helping build this vital tool! We are using Microsoft Teams to coordinate the design, development, and support efforts for CareTogether. Please contact [Lars Kemmann](https://github.com/LarsKemmann) to set up an introductory call and request an invite.

## Licensing Notice
CareTogether is licensed under the GNU Affero General Public License v3.0 (AGPL-3.0) which, crucially, **only permits hosting this software (including derivatives) if** you also make the source code of the software and any of your modifications available to your users under this same license. This effectively ensures that CareTogether CMS remains forever open-source and doesn't simply become the base code for a proprietary derivative at some point. We value collaboration and openness, and we believe that the best way to accomplish this is to ensure the software remains open to everyone.
