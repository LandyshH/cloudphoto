# cloudphoto
Команда, чтобы сбилдить приложение

dotnet publish --runtime {runtime} --configuration Release -p:PublishSingleFile=true --self-contained true -p:AssemblyName=cloudphoto

(Например для windows: dotnet publish --runtime win-x64 --configuration Release -p:PublishSingleFile=true --self-contained true -p:AssemblyName=cloudphoto)
(приложение будет лежать в папке \CloudPhoto\CloudPhoto\bin\Release\net7.0\win-x64\cloudphoto.exe)

Так же не забудьте добавить в переменные среды путь до приложения cloudphoto

# приложения для windows и linux лежат в архиве (applications)[CloudPhoto/applications.rar]#
