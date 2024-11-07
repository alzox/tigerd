## tiger-daemon

I got an internship this summer so I wanted to understand the C# system and what better way to do it than writing a daemon (*I really wanted to write a daemon*). Microsoft has wrote most of it for you, they give you configurable factories and have already created their own opinionated frameworks plus interfaces with respective OS systems. In my case I used the Worker blueprint to create a daemon that can be integrated with systemd or windows scm. It has **SMTP** capability as well which is the most fun part of it 

## Installation

### Prerequisite

- .NET 8.0
- ASP Run-Time

### Set-Up

1. Clone the repository:
```
https://github.com/You-Gao/tiger-daemon.git
```

## Usage
1. cd tiger 
2. add config with SMTP login info:
   
```
{
  "SmtpSettings": {
    "Server": "smtp.gmail.com",
    "Port": 587,
    "Email": "email",
    "Password": "app_password or password"
  }
}

```

3. edit processes.txt and goodprocesses.txt
4. dotnet build
5. dotnet run (-- chaotic)

x. There are online guides to integrate with SCM or systemd

## To-do
* Add an animation tiger?
* Make logo
* Clean code
* Write tests
* Add more message flags

## License
This project is licensed under the [Creative Commons CC0 1.0 Universal License](https://creativecommons.org/publicdomain/zero/1.0/). See the [LICENSE](LICENSE) file for more details.


