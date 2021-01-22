# CRY crypto-chat
<p align="justify"><i>Cryptography and Computer Security</i> course project, as taught at the Faculty of Electrical Engineering Banja Luka. <b>CRY</b> is a console <a href="https://en.wikipedia.org/wiki/End-to-end_encryption">E2EE</a> chat application. I created this project a year ago, but I never got around to opensourcing it.</p>

## Usage
### Register
<p align="justify">To chat with other user you first need to create an account. You do that by registering using an <i>Username</i>, <i>Password</i> and a <i><a href="https://en.wikipedia.org/wiki/X.509">X.509</a> Public Certificate</i> file which has been issued by the appropriate CA.</p>

### Login
<p align="justify">Once user has succesfully started the chat application, <b>CRY</b> will prompt user to enter a new command. To use the chat application user must first login. At login, user needs to provide <i>Username</i>, <i>Password</i> and a valid copy of his <i>Private RSA Key</i>. RSA key is valid if it matches a Public Key from users Public Certificate he used to register. The application will only login if the private key matches the public key stored in the certificate inside of the applications database <i>Users.db</i>.</p>

## Commands
Detailed explanation of the individual commands and their flags.
COMMAND | NAME | SYNOPSIS | DESCRIPTION
| --- | --- | --- | ---
LOGIN | login - allows access into the chat application | **log** -name *username* -pass *password* -key *key_path* | Command allows user to login into the chat application. User needs username, password and it's private RSA key.
REGISTER | register - creates a new user | **reg** -name *username* -pass *password* -cert *certificate_path* | Command creates a new user in database *Users.db*. User needs to provide username, password and a valid certificate that has been issued by the appropriate certificate authority which is denoted here as *rootCA.pem* file.
USER #1 | usr - show all users that have an account | **usr** -a | Command shows every user that has an registered account.
USER #2 | usr - shows online users | **usr** -o | Command shows every user that is online.
CHAT | chat - starts E2EE chat with other user. | **char** -user *username* -encalgo *[aes \| 3ds \| 2fh]* -hashalgo *[sh1 \| sh2 \| md5]* | Command initializes a three-way CRY handshake and if it's successful E2EE chat will be stared.
LOGOFF | logoff - allows users to logout | **logoff** | Command performs log out on the currently logged in user which in turn allows same/different user to login using the same instance of the application.

## Algorithms
List of symmetric encryption algorithms that are implemented in <b>CRY</b>.
ALGORITHM<br>NAME | BLOCK CIPHER<br>MODE OF OPERATION | KEY<br>SIZE | BLOCK<br>SIZE
| --- | --- | --- | ---
<a href="https://en.wikipedia.org/wiki/Advanced_Encryption_Standard">AES</a> | <a href="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation#Cipher_block_chaining_(CBC)">CBC</a> | 256 bits | 128 bits
<a href="https://en.wikipedia.org/wiki/Triple_DES">3DES</a> | CBC | 192 bits | 64 bits
<a href="https://www.schneier.com/academic/archives/1998/12/the_twofish_encrypti.html">Twofish</a> | <a href="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation#Electronic_codebook_(ECB)">ECB</a> | 256 bits | 128 bits

Hashing algorithms that are implemented in <b>CRY</b>:
 * <a href="https://en.wikipedia.org/wiki/MD5">MD5</a>
 * <a href="https://en.wikipedia.org/wiki/SHA-1">SHA1</a>
 * <a href="https://en.wikipedia.org/wiki/SHA-2">SHA256</a>

## Technologies
<p align="justify"><b>CRY</b> is written in C# and requires <a href="https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-web-installer">.NET Framework 4.7.2</a> to run. It was developed in Visual Studio Community 2019 and application uses <a href="https://www.sqlite.org/index.html">SQLite</a> with <a href="https://docs.microsoft.com/en-us/ef/ef6/">Entity Framework 6</a> to store data. For the implementation of Twofish algorithm I used <a href="https://en.wikipedia.org/wiki/Bouncy_Castle_(cryptography)">BouncyCastle</a> Nuget package.</p>

## To-Do List
- [ ] Remove pepper from code and store it as a random value in a config file instead of predefined deterministic value stored in code.
- [ ] Replace PRNG with CSPRNG.
- [ ] Change default block cipher mode of operation from an unsafe ECB to CBC mode for Twofish algorithm.
- [ ] Expand the project - create a LAN chat.
