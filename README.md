# CRY crypto-chat
*Cryptography and Computer Security* course project, as taught at the Faculty of Electrical Engineering Banja Luka. **CRY** is a console enripted chat application written in **C#**.

## Usage
### Register
To chat with other user you first need to create an account. You do that by registering using an *Username*, *Password* and a *X.509 Public Certificate* file which has been issued by the appropriate CA.
### Login
Once user has succesfully started the chat application, **CRY** will prompt user to enter a new command. To use the chat application user must first login.
At login, user needs to provide *Username* and *Password* and a valid copy of his *Private RSA Key*. RSA key is valid if it matches a Public Key included in his Public Certificate he used to register. The application will only login if the private key matches the public key stored in the certificate inside of the applications database *Users.db*.

## Commands
Detailed explanation of the individual commands and their flags.
COMMAND | NAME | SYNOPSIS | DESCRIPTION |
| --- | --- | --- | --- |
LOG | login - allows access into the chat application | **log** -name *user_name* -pass *user_pass* -key *key_location* | Command allows user to login into the chat application. User needs username, password and it's private RSA key. |
REG | register - creates a new user | **reg** -name *user_name* -pass *user_pass* -cert *certificate_location* | Command creates a new user in database *Users.db*. User needs to provide username, password and a valid certificate that has been issued by the appropriate certificate authority which is denoted here as *rootCA.pem* file. |
LOGOFF | logoff - allows users to logout | **logoff** | Command performs log out on the currently logged in user which in turn allows same/different user to login using the same instance of the application.

## Technologies
**CRY** is written in C# and requires [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-web-installer) to run. It was developed in Visual Studio Community 2019 and application uses [SQLite](https://www.sqlite.org/index.html) with [Entity Framework 6](https://docs.microsoft.com/en-us/ef/ef6/) to store data. For the implementation of [Twofish algorithm](https://en.wikipedia.org/wiki/Twofish) I used [BouncyCastle](https://en.wikipedia.org/wiki/Bouncy_Castle_(cryptography)) Nuget package

## To-Do List
- [ ] Remove pepper from code and store it as a random value in a cofig file.
- [ ] Expand the project - create a LAN chat.
