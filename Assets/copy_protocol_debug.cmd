call ChatSample\Server\build.cmd
cd..
copy ChatSample\Server\Bin\protobuf-net.dll assets\project\plugins
copy ChatSample\Server\Bin\Regulus.Project.Chat.Common.dll assets\project\plugins
copy ChatSample\Server\Bin\Regulus.project.Chat.Protocol.dll assets\project\plugins
copy ChatSample\Server\Bin\RegulusLibrary.dll assets\project\plugins
copy ChatSample\Server\Bin\RegulusRemoting.dll assets\project\plugins

copy ChatSample\Regulus\Library\RemotingNativeGhost\bin\Debug\RegulusRemotingGhostNative.dll assets\project\plugins

pause
