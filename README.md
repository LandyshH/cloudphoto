# cloudphoto
Команда, чтобы сбилдить приложение
dotnet publish --runtime {runtime} --configuration Release -p:PublishSingleFile=true --self-contained true -p:AssemblyName=cloudphoto
(Например для windows: dotnet publish --runtime win-x64 --configuration Release -p:PublishSingleFile=true --self-contained true -p:AssemblyName=cloudphoto)
Так же не забудьте добавить в переменные среды путь до приложения cloudphoto
