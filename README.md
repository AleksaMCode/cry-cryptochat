# CRY crypto-chat
<p align="justify"><i>Cryptography and Computer Security</i> course project, as taught at the Faculty of Electrical Engineering Banja Luka. <b>CRY</b> is a console <a href="https://en.wikipedia.org/wiki/End-to-end_encryption">E2EE</a> chat application written in <b><i>C#</i></b>.</p>

## Usage
### Register
<p align="justify">To chat with other user you first need to create an account. You do that by registering using an <i>Username</i>, <i>Password</i> and a <i>X.509 Public Certificate</i> file which has been issued by the appropriate CA.</p>

### Login
<p align="justify">Once user has succesfully started the chat application, <b>CRY</b> will prompt user to enter a new command. To use the chat application user must first login.<br>
At login, user needs to provide <i>Username</i>, <i>Password</i> and a valid copy of his <i>Private RSA Key</i>. RSA key is valid if it matches a Public Key from users Public Certificate he used to register. The application will only login if the private key matches the public key stored in the certificate inside of the applications database <i>Users.db</i>.</p>

## Commands
Detailed explanation of the individual commands and their flags.
COMMAND | NAME | SYNOPSIS | DESCRIPTION |
| --- | --- | --- | --- |
LOG | login - allows access into the chat application | **log** -name *user_name* -pass *user_pass* -key *key_location* | Command allows user to login into the chat application. User needs username, password and it's private RSA key. |
REG | register - creates a new user | **reg** -name *user_name* -pass *user_pass* -cert *certificate_location* | Command creates a new user in database *Users.db*. User needs to provide username, password and a valid certificate that has been issued by the appropriate certificate authority which is denoted here as *rootCA.pem* file. |
LOGOFF | logoff - allows users to logout | **logoff** | Command performs log out on the currently logged in user which in turn allows same/different user to login using the same instance of the application.

## Technologies
<p align="justify"><b>CRY</b> is written in C# and requires <a href="https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-web-installer">.NET Framework 4.7.2</a> to run. It was developed in Visual Studio Community 2019 and application uses <a href="https://www.sqlite.org/index.html">SQLite</a> with <a href="https://docs.microsoft.com/en-us/ef/ef6/">Entity Framework 6</a> to store data. For the implementation of <a href="https://en.wikipedia.org/wiki/Twofish">Twofish algorithm</a> I used <a href="https://en.wikipedia.org/wiki/Bouncy_Castle_(cryptography)">BouncyCastle</a> Nuget package.</p>

## To-Do List
- [ ] Remove pepper from code and store it as a random value in a config file instead of predefined deterministic value stored in code.
- [ ] Replace PRNG with CSPRNG.
- [ ] Expand the project - create a LAN chat.
