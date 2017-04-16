cd ChatSample\Server
call  build.cmd
cd..
cd..
copy ChatSample\Server\Bin\Regulus.Project.Chat.Common.dll assets\project\plugins
copy ChatSample\Server\Bin\protobuf-net.dll assets\regulus\plugins
copy ChatSample\Server\Bin\RegulusLibrary.dll assets\regulus\plugins
copy ChatSample\Server\Bin\RegulusRemoting.dll assets\regulus\plugins
copy ChatSample\Regulus\Library\RemotingNativeGhost\bin\Debug\RegulusRemotingGhostNative.dll assets\regulus\plugins
copy ChatSample\Regulus\Library\RegulusProtocol\bin\Debug\Regulus.Protocol.dll assets\regulus\Plugins\Editor

copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\Regulus.Remoting.Unity.dll assets\regulus\Plugins


pause
